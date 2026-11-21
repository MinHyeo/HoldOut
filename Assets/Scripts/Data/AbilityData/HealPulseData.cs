using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Ability/HealPulse")]
public class HealPulseData : AbilityData
{
    public override Ability CreateAbility(CharacterCombat owner)
    {
        return new HealPulseAbility(owner, this);
    }
}
