using UnityEngine;

public class Enemy_InvestigateState : EnemyState
{
    private Vector3 targetPosition;
    private float waitTime = 3f;
    private bool hasArrived;

    public Enemy_InvestigateState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public void SetTargetPosition(Vector3 position) => targetPosition = position;

    public override void Enter()
    {
        base.Enter();

        hasArrived = false;
        enemy.agent.isStopped = false;
        enemy.agent.SetDestination(targetPosition);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.CanSeePlayer())
        {
            stateMachine.ChangeState(enemy.chaseState);
            return;
        }

        if (!hasArrived && !enemy.agent.pathPending && enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
        {
            hasArrived = true;
            stateTimer = waitTime; 
            enemy.agent.isStopped = true; 
        }

        if (hasArrived)
        {
            if (stateTimer < 0)
            {
                stateMachine.ChangeState(enemy.idleState);
            }
        }
    }
}