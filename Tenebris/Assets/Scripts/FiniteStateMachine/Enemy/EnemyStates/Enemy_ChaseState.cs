using UnityEngine;

public class Enemy_ChaseState : EnemyState
{
    public Enemy_ChaseState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.isStopped = false;
    }

    public override void Update()
    {
        base.Update();

        enemy.agent.SetDestination(enemy.player.position);

        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.position);

        if (distanceToPlayer > enemy.visionRange * 1.2f)
        {
            Debug.Log("<color=yellow>Enemy AI:</color> Lost track of the player.");
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}