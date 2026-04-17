using UnityEngine;

/// <summary>
/// Motor de Movimiento que convierte el input en velocidad
///
///     - Motor "Kinematic": NO ES NECESARIO RIGIBODY NI EL CHARACTERCONTROLLER DE UNITY
///     - Se ejecuta en FixedUpdate para sincronizar con la fisica
/// </summary>
public class ThirdPersonMotor : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] CharacterInputHandler input;
    [SerializeField] Transform cameraTransform;
    [SerializeField] PlayerStats stats;
    [SerializeField] KinematicMover mover;
    [SerializeField] CapsuleResizer resizer;

    [Header("Runtime")]
    [SerializeField] Vector3 velocity;

    //Toggle (Interruptor) de Crouch (Agacharse)
    bool crouchState;
    bool prevCrouchHeld;
    bool prevJumpHeld;

    //Timers
    float coyoteCounter;
    float jumpBufferCounter;
    
    //Referencia para Debugear
    public Vector3 DebugVelocity => velocity;
    
    void Reset()
    {
        input = GetComponent<CharacterInputHandler>();
        mover = GetComponent<KinematicMover>();
        resizer = GetComponent<CapsuleResizer>();
    }
    
    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        if (stats == null || mover == null || input == null) return;

        bool grounded = mover.IsGrounded;

        //Actualizacion de Timers
        if (grounded) coyoteCounter = stats.jump.coyoteTime;
        else coyoteCounter = Mathf.Max(0f, coyoteCounter - dt);

        jumpBufferCounter = Mathf.Max(0f, jumpBufferCounter - dt);

        //Detecta subida en CrouchHeld
        bool crouchHeld = input.CrouchHeld;
        if (crouchHeld && !prevCrouchHeld)
        {
            crouchState = !crouchState;
            if (resizer != null) resizer.SetCrouch(crouchState);
        }
        prevCrouchHeld = crouchHeld;

        //Guardar una "ventana" para saltar
        bool jumpHeld = input.Jump;
        if (jumpHeld && !prevJumpHeld)
            jumpBufferCounter = stats.jump.bufferTime;
        prevJumpHeld = jumpHeld;

        //Calculo de Direcion de Movimiento
        Vector3 wishDir = GetMoveWorld(input.Move);
        if (grounded && wishDir.sqrMagnitude > 0.0001f)
            wishDir = Vector3.ProjectOnPlane(wishDir, mover.GroundNormal).normalized;

        float speed = stats.movement.maxSpeed;
        if (resizer != null && resizer.IsCrouching)
            speed *= stats.movement.crouchSpeedMultiplier;

        Vector3 targetHorizVel = wishDir * (speed * Mathf.Clamp01(input.Move.magnitude));

        //Aplicamos Aceleracion y Deceleracion (+ control en el aire)
        float accel = grounded ? stats.movement.acceleration : (stats.movement.acceleration * stats.movement.airControl);
        float decel = grounded ? stats.movement.deceleration : (stats.movement.deceleration * stats.movement.airControl);

        Vector3 horiz = Vector3.ProjectOnPlane(velocity, Vector3.up);
        
            //Si hay input == accel; si no == decel para frenar
        float use = (targetHorizVel.sqrMagnitude > 0.0001f) ? accel : decel;
        horiz = Vector3.MoveTowards(horiz, targetHorizVel, use * dt);

        velocity.x = horiz.x;
        velocity.z = horiz.z;

        //Jump (buffer + coyote)
        if (jumpBufferCounter > 0f && (grounded || coyoteCounter > 0f))
        {
            // v = sqrt(2 * g * h)
            velocity.y = Mathf.Sqrt(2f * stats.gravity.gravity * stats.jump.jumpHeight);
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
        }

        //Gravedad
        if (!grounded || velocity.y > 0f)
        {
            velocity.y -= stats.gravity.gravity * dt;
            
            //Limitar Velocidad de Caida
            float maxFall = -stats.gravity.terminalSpeed;
            if (velocity.y < maxFall) velocity.y = maxFall;
        }

        //Rotacion
        RotateTowards(wishDir, dt);

        //Movimiento con colisiones
        mover.TickMove(ref velocity, dt);
    }

    /// <summary>
    /// Convierte input 2D en direccion 3D
    /// SI hay CAMARA, movimiento relativo en la orientacion de la camara
    /// </summary>
    Vector3 GetMoveWorld(Vector2 move)
    {
        Vector3 dir = new Vector3(move.x, 0f, move.y);
        if (dir.sqrMagnitude < 0.0001f) return Vector3.zero;

        //Sin camara
        if (cameraTransform == null) return dir.normalized;

        //Con camara
        Vector3 f = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 r = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
        Vector3 world = (r * dir.x + f * dir.z);
        return world.sqrMagnitude > 0.0001f ? world.normalized : Vector3.zero;
    }

    /// <summary>
    /// Rota al personaje hacia del direccion de movimiento en XZ
    /// Evita que el personaje se incline aunque este en pendiente
    /// </summary>
    void RotateTowards(Vector3 moveWorld, float dt)
    {
        //Rotacion XZ
        Vector3 flat = Vector3.ProjectOnPlane(moveWorld, Vector3.up);
        if (flat.sqrMagnitude < 0.0001f) return;

        Quaternion target = Quaternion.LookRotation(flat.normalized, Vector3.up);

        float rotSpeed = 14f;
        transform.rotation = Quaternion.Slerp(transform.rotation, target, 1f - Mathf.Exp(-rotSpeed * dt));
    }
}
