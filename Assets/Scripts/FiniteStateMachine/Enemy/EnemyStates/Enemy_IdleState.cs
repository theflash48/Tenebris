using UnityEngine;

public class Enemy_IdleState : EnemyState
{
    public Enemy_IdleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = 1f;
        //enemy.SetVelocity(0f, 0f);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.CanSeePlayer())
        {
            stateMachine.ChangeState(enemy.chaseState);
            return; 
        }

        if (stateTimer < 0f)
        {
            stateMachine.ChangeState(enemy.patrolState);
        }
    }
    public override void Exit()
    {
        base.Exit();
    }
}
