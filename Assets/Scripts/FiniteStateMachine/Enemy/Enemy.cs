using UnityEngine;
using UnityEngine.AI;

public class Enemy : Entity, IHear
{
    [Header("NavMesh & Movement")]
    public NavMeshAgent agent { get; private set; }
    public Transform[] patrolPoints;

    [Header("Perception & Sensors")]
    public float visionRange = 10f;
    [Range(0, 360)] public float visionAngle = 90f;
    public float eyeHeight = 1.5f;

    public LayerMask whatIsPlayer;
    public LayerMask whatIsObstacle;

    [Header("Combat Stats")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    [Header("Hearing Settings")]
    public float immediateChaseRadius = 3f;

    // References
    public Transform player { get; private set; }

    // States
    public Enemy_IdleState idleState { get; private set; }
    public Enemy_PatrolState patrolState { get; private set; }
    public Enemy_ChaseState chaseState { get; private set; }
    
    public Enemy_InvestigateState investigateState { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        agent = GetComponent<NavMeshAgent>();

        if (rb != null)
            rb.isKinematic = true;

        idleState = new Enemy_IdleState(this, stateMachine, "Idle");
        patrolState = new Enemy_PatrolState(this, stateMachine, "Move");
        chaseState = new Enemy_ChaseState(this, stateMachine, "Chase");
        investigateState = new Enemy_InvestigateState(this, stateMachine, "Investigate");
    }

    protected override void Start()
    {
        base.Start();

        FindPlayer();
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update(); 
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }


    public bool CanSeePlayer()
    {
        if (player == null) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > visionRange) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer < visionAngle / 2f)
        {
            if (!Physics.Raycast(transform.position + Vector3.up * eyeHeight, directionToPlayer, distanceToPlayer, whatIsObstacle))
            {
                return true; 
            }
        }

        return false;
    }

    public void OnHearSound(Vector3 soundOrigin, float soundRadius)
    {
        if (stateMachine.currentState == chaseState) return;

        float distanceToSound = Vector3.Distance(transform.position, soundOrigin);

        if (distanceToSound <= immediateChaseRadius)
        {
            stateMachine.ChangeState(chaseState);
            return;
        }

        if (stateMachine.currentState == investigateState) return;

        float randomErrorX = Random.Range(-2.5f, 2.5f);
        float randomErrorZ = Random.Range(-2.5f, 2.5f);
        Vector3 approximateLocation = new Vector3(soundOrigin.x + randomErrorX, soundOrigin.y, soundOrigin.z + randomErrorZ);

        investigateState.SetTargetPosition(approximateLocation);
        stateMachine.ChangeState(investigateState);
    }

    public bool IsPlayerInAttackRange()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) <= attackRange;
    }

 
    public override void SetVelocity(float xVelocity, float yVelocity)
    {
    }

    // --- DEBUGGING ---

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        // 2. Calculate the left and right limits of the vision cone
        Vector3 leftViewBoundary = Quaternion.Euler(0, -visionAngle / 2f, 0) * transform.forward;
        Vector3 rightViewBoundary = Quaternion.Euler(0, visionAngle / 2f, 0) * transform.forward;

        // 3. Draw the actual V-shaped cone of vision
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position + Vector3.up * eyeHeight, leftViewBoundary * visionRange);
        Gizmos.DrawRay(transform.position + Vector3.up * eyeHeight, rightViewBoundary * visionRange);

        // 4. Draw the attack range (Red sphere)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}