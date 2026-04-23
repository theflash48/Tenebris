using UnityEngine;

public class Player_MoveState : Player_GroundedState
{
    private float noiseTimer;
    private float noiseInterval = 0.2f; 

    public Player_MoveState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Player has entered Move State.");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (player.moveInput == Vector2.zero)
        {
            stateMachine.ChangeState(player.idleState);
            return; 
        }

        // Handle noise generation for alerting enemies
        noiseTimer -= Time.deltaTime;
        if (noiseTimer <= 0)
        {
            float soundRadius = 10f; 
            player.AlertEnemies(soundRadius);
            noiseTimer = noiseInterval;
        }

        Vector2 redirectedInput = player.MovementDirectionToCamera(player.moveInput);
        player.SetVelocity(redirectedInput.x * player.moveSpeed, redirectedInput.y * player.moveSpeed);
    }
}
