using UnityEngine;

public class Player_GroundedState : PlayerState
{
    public Player_GroundedState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        if (rb.linearVelocity.y < 0 && !player.groundDetected)
            stateMachine.ChangeState(player.fallState);

        if (input.Player.Jump.WasPerformedThisFrame())
            stateMachine.ChangeState(player.jumpState);

    }
    public override void Exit()
    {
        base.Exit();
    }
}
