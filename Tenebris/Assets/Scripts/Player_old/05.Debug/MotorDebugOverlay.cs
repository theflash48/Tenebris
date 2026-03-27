using UnityEngine;

public class MotorDebugOverlay : MonoBehaviour
{
    [SerializeField] private KinematicMover mover;
    [SerializeField] private ThirdPersonMotor motor;
    [SerializeField] private bool show = true;

    private void Reset()
    {
        mover = GetComponent<KinematicMover>();
        motor = GetComponent<ThirdPersonMotor>();
    }

    private void OnGUI()
    {
        if (!show || mover == null) return;

        GUILayout.BeginArea(new Rect(10, 10, 360, 240), GUI.skin.box);
        GUILayout.Label($"Grounded: {mover.IsGrounded}");
        GUILayout.Label($"GroundNormal: {mover.GroundNormal}");
        GUILayout.Label($"SnapApplied: {mover.Debug_LastSnapApplied:0.000}");
        GUILayout.Label($"Step Attempted: {mover.Debug_LastStepAttempted}  Succeeded: {mover.Debug_LastStepSucceeded}  Forward: {mover.Debug_LastStepForward:0.000}");

        if (motor != null) GUILayout.Label($"Velocity: {motor.DebugVelocity}");

        if (mover.Debug_LastGroundHitValid) GUILayout.Label($"Slope: {Vector3.Angle(mover.Debug_LastGroundHit.normal, Vector3.up):0.0}°");

        GUILayout.EndArea();
    }
}