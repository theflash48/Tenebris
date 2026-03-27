using UnityEngine;

/// <summary>
/// Controlador Kinematico
/// </summary>
[RequireComponent(typeof(CapsuleCollider))]
public class KinematicMover : MonoBehaviour
{
    [SerializeField] CapsuleCollider capsule;
    [SerializeField] PlayerStats stats;

    public bool IsGrounded { get; set; }
    public Vector3 GroundNormal { get; set; } = Vector3.up;
    
    //Para Debugear
    public bool Debug_LastGroundHitValid { get; private set; }
    public RaycastHit Debug_LastGroundHit { get; private set; }

    public bool Debug_LastWallHitValid { get; private set; }
    public RaycastHit Debug_LastWallHit { get; private set; }

    public float Debug_LastSnapApplied { get; private set; }
    public bool Debug_LastStepAttempted { get; private set; }
    public bool Debug_LastStepSucceeded { get; private set; }
    public float Debug_LastStepForward { get; private set; }

    void Reset()
    {
        capsule = GetComponent<CapsuleCollider>();
    }

    /// <summary>
    /// Paso Principal del Movimiento
    /// </summary>
    public void TickMove(ref Vector3 velocity, float dt)
    {
        if (capsule == null || stats == null) return;

        bool wasGrounded = IsGrounded;

        ResolveOverlaps();

        //Mover con colisiones y CapsuleCast
        Vector3 displacement = velocity * dt;
        MoveWithCollisions(ref displacement, ref velocity);

        ProbeGround(ref velocity);

        //StepDown suave si estabas en el suelo
        SnapDownIfNeeded(wasGrounded, ref velocity, dt);

        //Para micro huecos
        StickyGroundSnap(wasGrounded, ref velocity, dt);
    }
    
    /// <summary>
    /// Mantiene el personaje pegado al suelo
    ///     - No actua si estas saltando
    /// </summary>
    void StickyGroundSnap(bool wasGrounded, ref Vector3 velocity, float dt)
    {
        if (IsGrounded) return;
        if (!wasGrounded) return;
        
        //Si estamos subiendo
        if (velocity.y > 0.01f) return;

        float maxSnapDown = Mathf.Max(0.01f, stats.collision.stationarySnapDownDistance);

        GetWorldCapsule(out Vector3 p1, out Vector3 p2, out float radius);

        bool hit = Physics.CapsuleCast(
            p1, p2, radius,
            Vector3.down, out RaycastHit h,
            maxSnapDown + stats.collision.skinWidth,
            stats.collision.collisionMask,
            QueryTriggerInteraction.Ignore
        );

        if (!hit) return;

        //Comprobacion Pendiente
        float slopeAngle = Vector3.Angle(h.normal, Vector3.up);
        if (slopeAngle > stats.collision.maxSlopeAngle) return;

        float desiredDrop = Mathf.Max(h.distance - stats.collision.skinWidth, 0f);
        if (desiredDrop <= 0.0001f) return;

        //Bajada Suave por frame
        float maxDropThisFrame = Mathf.Max(0.01f, stats.collision.maxSnapDownSpeed * dt);
        float appliedDrop = Mathf.Min(desiredDrop, maxDropThisFrame);

        transform.position += Vector3.down * appliedDrop;

        IsGrounded = true;
        GroundNormal = h.normal;

        if (velocity.y < 0f) velocity.y = 0f;
    }
    
    /// <summary>
    /// StepDown al bajar escalones/bordes
    ///
    /// Sin esto:
    /// - Al perder suelo un frame, entra gravedad
    /// - En pendientes puede generar aceleracion o “rebotes” raros
    /// </summary>
    void SnapDownIfNeeded(bool wasGrounded, ref Vector3 velocity, float dt)
    {
        if (IsGrounded) return;
        if (!wasGrounded) return;

        if (velocity.y > 0.01f) return;

        //Si no hay Movimiento Horizontal, no hace falta enganchar escalones
        Vector3 horiz = new Vector3(velocity.x, 0f, velocity.z);
        if (horiz.sqrMagnitude < 0.0004f) return;

        float maxSnapDown = stats.collision.stepHeight + stats.collision.groundSnapDistance + 0.05f;

        GetWorldCapsule(out Vector3 p1, out Vector3 p2, out float radius);

        bool hit = Physics.CapsuleCast(
            p1, p2, radius,
            Vector3.down, out RaycastHit h,
            maxSnapDown + stats.collision.skinWidth,
            stats.collision.collisionMask,
            QueryTriggerInteraction.Ignore
        );

        if (!hit) return;

        float slopeAngle = Vector3.Angle(h.normal, Vector3.up);
        if (slopeAngle > stats.collision.maxSlopeAngle) return;

        float desiredDrop = Mathf.Max(h.distance - stats.collision.skinWidth, 0f);
        if (desiredDrop <= 0.0001f) return;

        float maxDropThisFrame = Mathf.Max(0.01f, stats.collision.maxSnapDownSpeed * dt);
        float appliedDrop = Mathf.Min(desiredDrop, maxDropThisFrame);

        transform.position += Vector3.down * appliedDrop;

        IsGrounded = true;
        GroundNormal = h.normal;

        if (velocity.y < 0f) velocity.y = 0f;
    }
    
    /// <summary>
    /// Aplica el desplazamiento con colisiones
    /// El solver repite hasta maxBounces para resolver esquinas y colisiones multiples
    /// </summary>
    /// <param name="displacement"></param>
    /// <param name="velocity"></param>
    void MoveWithCollisions(ref Vector3 displacement, ref Vector3 velocity)
    {
        float remaining = displacement.magnitude;
        if (remaining <= 0.000001f) return;

        Vector3 dir = displacement / remaining;

        for (int i = 0; i < stats.collision.maxBounces; i++)
        {
            if (remaining <= 0.000001f) break;

            GetWorldCapsule(out Vector3 p1, out Vector3 p2, out float radius);

            float castDist = remaining + stats.collision.skinWidth;
            bool hitSomething = Physics.CapsuleCast(
                p1, p2, radius,
                dir, out RaycastHit hit,
                castDist,
                stats.collision.collisionMask,
                QueryTriggerInteraction.Ignore
            );

            if (!hitSomething)
            {
                transform.position += dir * remaining;
                return;
            }

            bool canTryStep =
                IsGrounded &&
                velocity.y <= 0.01f &&
                stats.collision.stepHeight > 0.001f &&
                hit.normal.y < 0.2f;

            if (canTryStep)
            {
                //Direcion Horizontal para Evitar Escalones
                Vector3 stepDir = Vector3.ProjectOnPlane(dir, Vector3.up);
                if (stepDir.sqrMagnitude > 0.0001f)
                {
                    stepDir.Normalize();

                    if (TryStepUp(stepDir, remaining, out float movedForward))
                    {
                        remaining = Mathf.Max(0f, remaining - movedForward);

                        if (remaining <= 0.000001f) return;

                        continue;
                    }
                }
            }

            //Movimiento hasta el impacto
            float moveDist = Mathf.Max(hit.distance - stats.collision.skinWidth, 0f);
            if (moveDist > 0f)
                transform.position += dir * moveDist;

            remaining -= moveDist;

            // Slide
            Vector3 n = hit.normal;
            Vector3 remainingVec = dir * remaining;
            remainingVec = Vector3.ProjectOnPlane(remainingVec, n);
            velocity = Vector3.ProjectOnPlane(velocity, n);

            remaining = remainingVec.magnitude;
            if (remaining <= 0.000001f) break;
            dir = remainingVec / remaining;
        }
    }

    /// <summary>
    /// Intenta subir un escalon
    ///     - Devuelve TRUE si tuvo exito
    /// </summary>
    bool TryStepUp(Vector3 moveDir, float moveDist, out float movedForward)
    {
        movedForward = 0f;
        
        float stepH = stats.collision.stepHeight;
        if (stepH <= 0.001f) return false;
        if (moveDist <= 0.001f) return false;

        Vector3 startPos = transform.position;

        //Comprobacion si puedes subir sin chocar
        if (CapsuleCast(Vector3.up, stepH, out _))
            return false;

        transform.position += Vector3.up * stepH;

        //Intenta avanzar
        float forwardAllowed = moveDist;

        if (CapsuleCast(moveDir, moveDist, out RaycastHit fHit))
        {
            forwardAllowed = Mathf.Max(fHit.distance - stats.collision.skinWidth, 0f);
            if (forwardAllowed <= 0.001f)
            {
                transform.position = startPos;
                return false;
            }
        }

        transform.position += moveDir * forwardAllowed;
        movedForward = forwardAllowed;

        //Busca SUELO hacia abajo
        float downProbe = stepH + stats.collision.groundSnapDistance + stats.collision.skinWidth + 0.05f;

        if (!CapsuleCast(Vector3.down, downProbe, out RaycastHit dHit))
        {
            transform.position = startPos;
            movedForward = 0f;
            return false;
        }

        float slopeAngle = Vector3.Angle(dHit.normal, Vector3.up);
        if (slopeAngle > stats.collision.maxSlopeAngle)
        {
            transform.position = startPos;
            movedForward = 0f;
            return false;
        }

        //Bajar hasta tocar el suelo
        float drop = Mathf.Max(dHit.distance - stats.collision.skinWidth, 0f);
        transform.position += Vector3.down * drop;

        return true;
    }
    
    /// <summary>
    /// Wrapper de CapsuleCast usando geometria del collider actual
    /// </summary>
    bool CapsuleCast(Vector3 dir, float dist, out RaycastHit hit)
    {
        GetWorldCapsule(out Vector3 p1, out Vector3 p2, out float radius);

        return Physics.CapsuleCast(
            p1, p2, radius,
            dir, out hit,
            dist + stats.collision.skinWidth,
            stats.collision.collisionMask,
            QueryTriggerInteraction.Ignore
        );
    }

    /// <summary>
    /// Devuelve true si no chocaria al moverte "dist" en "dir"
    /// </summary>
    bool CanMove(Vector3 dir, float dist)
    {
        GetWorldCapsule(out Vector3 p1, out Vector3 p2, out float radius);
        return !Physics.CapsuleCast(
            p1, p2, radius,
            dir, dist + stats.collision.skinWidth,
            stats.collision.collisionMask,
            QueryTriggerInteraction.Ignore
        );
    }
    
    /// <summary>
    /// Comprueba si hay suelo debajo
    /// </summary>
    void ProbeGround(ref Vector3 velocity)
    {
        //Si estamos saltando, no hay snap
        if (velocity.y > 0.01f)
        {
            IsGrounded = false;
            GroundNormal = Vector3.up;
            return;
        }

        GetWorldCapsule(out Vector3 p1, out Vector3 p2, out float radius);

        float castDist = stats.collision.groundProbeDistance + stats.collision.skinWidth;

        bool hitGround = Physics.CapsuleCast(
            p1, p2, radius,
            Vector3.down, out RaycastHit hit,
            castDist,
            stats.collision.collisionMask,
            QueryTriggerInteraction.Ignore
        );

        if (!hitGround)
        {
            IsGrounded = false;
            GroundNormal = Vector3.up;
            return;
        }

        float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
        bool validSlope = slopeAngle <= stats.collision.maxSlopeAngle;

        GroundNormal = hit.normal;
        IsGrounded = validSlope;

        //Snap corto al suelo si estamos cerca
        float snap = Mathf.Max(hit.distance - stats.collision.skinWidth, 0f);
        if (IsGrounded && velocity.y <= 0f && snap > 0f && snap <= stats.collision.groundSnapDistance)
            transform.position += Vector3.down * snap;

        //No resbalarse en pendientes mientras esta parado
        if (IsGrounded && velocity.y < 0f)
            velocity.y = 0f;
    }
    
    /// <summary>
    /// Si el player aparece o esta buggeado en un collider
    /// - OverlapCapsule para encontrar overlaps
    /// - ComputePenetration para saber hacia donde empujar
    /// - Empuja varias iteraciones hasta salir o llegar al limite
    /// </summary>
    void ResolveOverlaps()
    {
        for (int iter = 0; iter < stats.collision.maxDepenetrationIterations; iter++)
        {
            GetWorldCapsule(out Vector3 p1, out Vector3 p2, out float radius);

            Collider[] hits = Physics.OverlapCapsule(
                p1, p2, radius,
                stats.collision.collisionMask,
                QueryTriggerInteraction.Ignore
            );

            Vector3 bestDir = Vector3.zero;
            float bestDist = 0f;

            for (int i = 0; i < hits.Length; i++)
            {
                Collider other = hits[i];
                if (other == null || other == capsule) continue;

                bool overlapped = Physics.ComputePenetration(
                    capsule, transform.position, transform.rotation,
                    other, other.transform.position, other.transform.rotation,
                    out Vector3 direction, out float distance
                );

                if (!overlapped || distance <= 0f) continue;

                //Se elige el mas profundo para empujar primero
                if (distance > bestDist)
                {
                    bestDist = distance;
                    bestDir = direction;
                }
            }

            if (bestDist <= 0f) break;

            float push = Mathf.Min(bestDist + stats.collision.skinWidth, stats.collision.maxDepenetrationDistance);
            transform.position += bestDir * push;
        }
    }
    
    /// <summary>
    /// Convierte el CapsuleCollider a puntos en el mundo y radio
    /// </summary>
    void GetWorldCapsule(out Vector3 p1, out Vector3 p2, out float radius)
    {
        Vector3 center = transform.TransformPoint(capsule.center);
        radius = capsule.radius;

        float height = Mathf.Max(capsule.height, radius * 2f);
        float half = Mathf.Max(0f, (height * 0.5f) - radius);

        Vector3 up = transform.up;
        p1 = center + up * half;
        p2 = center - up * half;
    }
}
