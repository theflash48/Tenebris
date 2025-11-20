using UnityEngine;

public class EnemyState : EntityState
{
    protected Enemy enemy;

    public EnemyState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        // Referencia del enemigo
        this.enemy = enemy;

        // Referencias comunes
        rb = enemy.rb;
        anim = enemy.anim;
    }
}
