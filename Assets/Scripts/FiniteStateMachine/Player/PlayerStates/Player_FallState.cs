using UnityEngine;

public class Player_FallState : PlayerState
{
    public Player_FallState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Player has entered Fall State.");
    }


    public override void Update()
    {
        base.Update();
        if (player.groundDetected)
        {
            stateMachine.ChangeState(player.idleState);
        }

    }
    public override void Exit()
    {
        base.Exit();
    }
}
