using UnityEngine;

public class Player_JumpState : PlayerState
{
    public Player_JumpState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Player has entered Jump State.");

        player.rb.AddForce(Vector3.up * player.jumpForce, ForceMode.Impulse); // Aplica una fuerza hacia arriba al saltar
        //player.SetVelocity(rb.linearVelocity.x, player.jumpForce);
    }

    public override void Update()
    {
        base.Update();
        if (player.rb.linearVelocity.y < 0 && player.groundDetected)
        {
            stateMachine.ChangeState(player.fallState);
        }
    }
    public override void Exit()
    {
        base.Exit();
    }
}
