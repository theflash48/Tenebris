using UnityEngine;
using System;
using System.Collections;

public class Entity : MonoBehaviour
{
    public Animator anim { get; private set; }
    public Rigidbody rb { get; private set; }
    protected StateMachine stateMachine;

    [Header("Collision detection")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform groundCheck;
    //[SerializeField] public Transform targetCheck;
    //[SerializeField] public float targetCheckRadius = 1;
    public bool groundDetected { get; private set; }

    public float moveSpeed;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;

    

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogWarning("Animator component not found on " + gameObject.name);
        }
        rb = GetComponent<Rigidbody>();
        stateMachine = new StateMachine();
    }

    protected virtual void Start()
    {

    }
    protected virtual void Update()
    {
        HandleCollisionDetected();
        stateMachine.UpdateActiveState();
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        Vector3 inputDirection = new Vector3(xVelocity, 0f, yVelocity);

        // The Entity is on flat ground
        Vector3 velocity = rb.linearVelocity;
        velocity.x = xVelocity;
        velocity.z = yVelocity;

        // Normalizar solo si hay movimiento
        if (velocity.magnitude > 1f)
        {
            velocity = velocity.normalized * moveSpeed;
        }
        rb.linearVelocity = new Vector3(xVelocity, 0f, yVelocity);
        
    }

    private void HandleCollisionDetected()
    {
        groundDetected = Physics.Raycast(groundCheck.position, Vector3.down, groundCheckDistance, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        float rayDistance = 1f * 0.5f + 0.3f;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance));


        // Raycast para detectar el suelo
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, rayDistance))
        {
            // Dibujar la normal de la superficie (perpendicular a la pendiente)
            Gizmos.DrawRay(hit.point, hit.normal * 2f);

            // Punto de impacto
            Gizmos.DrawSphere(hit.point, 0.1f);
        }
        //Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }

    internal void CurrentStateAnimationTrigger()
    {
        stateMachine.currentState.CallAnimationTrigger();
    }
}
