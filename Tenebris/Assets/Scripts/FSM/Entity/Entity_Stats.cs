using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public Stats maxHealth;
    public Stats vitality;
    public Stats attackDamage;

    public float GetMaxetHealth()
    {
        float baseHp = maxHealth.GetValue();
        float vitBonus = vitality.GetValue() * 5f;

        return baseHp + vitBonus;
    }

    public float GetMaxAttack()
    {
        float damage = attackDamage.GetValue();
        return damage;
    }
}
