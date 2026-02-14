using UnityEngine;

public class MotorGizmos : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private KinematicMover mover;
    [SerializeField] private ThirdPersonMotor motor;
    [SerializeField] private CapsuleCollider capsule;
    [SerializeField] private PlayerStats stats;

    [Header("Draw Toggles")]
    public bool drawCapsule = true;
    public bool drawVelocity = true;
    public bool drawGroundProbe = true;
    public bool drawGroundNormal = true;
    public bool drawWallHit = true;
    public bool drawStepInfo = true;
    public bool drawSnapDown = true;

    [Header("Scales")]
    public float velocityScale = 0.25f;
    public float normalScale = 0.5f;

    private void Reset()
    {
        mover = GetComponent<KinematicMover>();
        motor = GetComponent<ThirdPersonMotor>();
        capsule = GetComponent<CapsuleCollider>();
    }

    private void OnDrawGizmosSelected()
    {
        if (mover == null) mover = GetComponent<KinematicMover>();
        if (motor == null) motor = GetComponent<ThirdPersonMotor>();
        if (capsule == null) capsule = GetComponent<CapsuleCollider>();

        // Stats puede venir del motor o asignarse a mano
        if (stats == null && motor != null)
        {
            // Si tu ThirdPersonMotor no expone stats públicamente, asígnalo en inspector.
        }

        if (capsule == null) return;

        GetWorldCapsule(capsule, out Vector3 p1, out Vector3 p2, out float radius);

        if (drawCapsule)
        {
            Gizmos.color = mover != null && mover.IsGrounded ? new Color(0.2f, 1f, 0.2f, 1f) : new Color(1f, 0.3f, 0.3f, 1f);
            DrawWireCapsule(p1, p2, radius);
        }

        if (mover == null) return;

        if (drawVelocity && motor != null)
        {
            // Si velocity no es accesible, comenta esto o expón una property en el motor.
            // Gizmos.DrawRay(transform.position + Vector3.up * 0.05f, motor.DebugVelocity * velocityScale);
        }

        if (drawGroundProbe)
        {
            if (mover.Debug_LastGroundHitValid)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, mover.Debug_LastGroundHit.point);
                Gizmos.DrawWireSphere(mover.Debug_LastGroundHit.point, 0.05f);
            }
        }

        if (drawGroundNormal && mover.Debug_LastGroundHitValid)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(mover.Debug_LastGroundHit.point, mover.Debug_LastGroundHit.normal * normalScale);
        }

        if (drawWallHit && mover.Debug_LastWallHitValid)
        {
            Gizmos.color = new Color(1f, 0.6f, 0.1f, 1f);
            Gizmos.DrawWireSphere(mover.Debug_LastWallHit.point, 0.06f);
            Gizmos.DrawRay(mover.Debug_LastWallHit.point, mover.Debug_LastWallHit.normal * normalScale);
        }

        if (drawStepInfo && stats != null)
        {
            // Plano de step height sobre el “suelo” aproximado del capsule
            Vector3 bottom = GetBottomWorld(capsule);
            Gizmos.color = mover.Debug_LastStepSucceeded ? Color.green : new Color(0.6f, 0.6f, 0.6f, 1f);
            Gizmos.DrawLine(bottom + Vector3.up * stats.collision.stepHeight + Vector3.left * 0.3f, bottom + Vector3.up * stats.collision.stepHeight + Vector3.right * 0.3f);
            Gizmos.DrawLine(bottom + Vector3.up * stats.collision.stepHeight + Vector3.forward * 0.3f, bottom + Vector3.up * stats.collision.stepHeight + Vector3.back * 0.3f);
        }

        if (drawSnapDown && mover.Debug_LastSnapApplied > 0.0001f)
        {
            Gizmos.color = new Color(0.8f, 0.2f, 1f, 1f);
            Gizmos.DrawRay(GetBottomWorld(capsule), Vector3.down * mover.Debug_LastSnapApplied);
        }
    }

    // ---------- Helpers ----------
    private static void GetWorldCapsule(CapsuleCollider c, out Vector3 p1, out Vector3 p2, out float radius)
    {
        // Asumimos CapsuleCollider.direction = Y
        Transform t = c.transform;
        Vector3 center = t.TransformPoint(c.center);

        radius = c.radius;
        float height = Mathf.Max(c.height, radius * 2f);
        float half = Mathf.Max(0f, (height * 0.5f) - radius);

        Vector3 up = t.up;
        p1 = center + up * half;
        p2 = center - up * half;
    }

    private static Vector3 GetBottomWorld(CapsuleCollider c)
    {
        Transform t = c.transform;
        Vector3 centerW = t.TransformPoint(c.center);
        float half = Mathf.Max(0f, (c.height * 0.5f) - c.radius);
        return centerW - t.up * half;
    }

    private static void DrawWireCapsule(Vector3 p1, Vector3 p2, float r)
    {
        Gizmos.DrawWireSphere(p1, r);
        Gizmos.DrawWireSphere(p2, r);

        Vector3 up = (p1 - p2).normalized;
        Vector3 right = Vector3.Cross(up, Vector3.forward);
        if (right.sqrMagnitude < 0.001f) right = Vector3.Cross(up, Vector3.right);
        right.Normalize();
        Vector3 forward = Vector3.Cross(right, up).normalized;

        Gizmos.DrawLine(p1 + right * r, p2 + right * r);
        Gizmos.DrawLine(p1 - right * r, p2 - right * r);
        Gizmos.DrawLine(p1 + forward * r, p2 + forward * r);
        Gizmos.DrawLine(p1 - forward * r, p2 - forward * r);
    }
}
