using UnityEngine;

public enum StatType 
{ 
    AttackPower,
    AttackSpeed,
    AttackRange,
    MoveSpeed,
    MaxHp,
}


[CreateAssetMenu(menuName = "Game/Upgrade/Stat")]
public class StatUpdateData : UpgradeData
{
    public StatType StatType;
    public float Percent;

    public override void Apply(Character target)
    {
        target.Stats.ApplyPercentBonus(StatType, Percent);
    }
}