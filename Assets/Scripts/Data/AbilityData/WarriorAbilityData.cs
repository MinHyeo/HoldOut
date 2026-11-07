using UnityEngine;

[CreateAssetMenu(menuName = "Game/Ability/Warrior")]
public class WarriorData : AbilityData
{
    public float halfAngle = 60f;   // 부채꼴 반각

    public override Ability CreateAbility(CharacterCombat owner)
    {
        return new WarriorAbility(owner, this);
    }
}