using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Player : Entity
{
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_FallState fallState { get; private set; }


    [Header("Movement Specs")]
    public Vector2 moveInput { get; private set; }
    public Camera cam;
    public float jumpForce = 5;

    

    protected override void Awake()
    {
        base.Awake();
        idleState = new Player_IdleState(this, stateMachine, "Idle");
        moveState = new Player_MoveState(this, stateMachine, "Move");
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        fallState = new Player_FallState(this, stateMachine, "jumpFall");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);

        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    protected override void Update()
    {
        base.Update();
        moveInput = InputManager.Instance.GetplayerMovement();

        Vector2 dir = new Vector2(moveInput.x, moveInput.y);

        if (dir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

    }

    public Vector2 MovementDirectionToCamera(Vector2 _moveInput)
    {
        if (cam == null)
        {
            Debug.LogWarning("No camera assigned!");
            return _moveInput;
        }
        Vector3 xVector = cam.transform.right;
        Vector3 zVector = Vector3.Cross(xVector, Vector3.up);
        Vector3 moveVector = _moveInput.x * xVector + _moveInput.y * zVector;
        Vector3 moveVector2 = new Vector2(moveVector.x, moveVector.z);
        return moveVector2;
    }

    // This method will be called by the player's attack or movement actions to alert nearby enemies.
    public void AlertEnemies(float soundRadius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, soundRadius);

        foreach (var collider in hitColliders)
        {
            IHear hearer = collider.GetComponent<IHear>();

            if (hearer != null)
            {
                hearer.OnHearSound(transform.position, soundRadius);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 2f); 
    }
}
