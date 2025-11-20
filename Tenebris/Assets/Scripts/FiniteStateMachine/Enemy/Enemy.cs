using UnityEngine;
using UnityEngine.AI;

public class Enemy : Entity
{
    public Enemy_IdleState idleState;

    [Header("Enemy Specs")]
    public float range;
    public float attackRange;
    public float waitTime;
    public float acceleration;

    [Header("Enemy Attack Specs")]
    public float attackAcceleration;
    public int damage;
    public float attackSpeed;
    public bool isAttacking;
    public float flinchTime;

    [Header("Player Coords")]
    [HideInInspector] public Transform playerTransform;
    public bool jugadorDetectado;
    public int nearness;

    public NavMeshAgent agent;

    public int facingDirection = 1;

    public Transform player { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
}
