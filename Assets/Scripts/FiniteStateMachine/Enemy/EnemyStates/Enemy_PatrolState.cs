using UnityEngine;

public class Enemy_PatrolState : EnemyState
{
    private int patrolIndex = 0;

    public Enemy_PatrolState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (enemy.patrolPoints.Length == 0)
        {
            Debug.LogWarning("<color=red>Enemy AI:</color> No patrol points assigned in the Inspector!");
            return;
        }

        enemy.agent.isStopped = false;
        enemy.agent.SetDestination(enemy.patrolPoints[patrolIndex].position);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.CanSeePlayer())
        {
            stateMachine.ChangeState(enemy.chaseState);
            return; 
        }

        if (enemy.patrolPoints.Length == 0) return;

        if (!enemy.agent.pathPending && enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
        {
            patrolIndex++;
            if (patrolIndex >= enemy.patrolPoints.Length)
            {
                patrolIndex = 0;
            }

            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}