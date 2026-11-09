using UnityEngine;

[CreateAssetMenu(menuName = "Game/Ability/Archer")]
public class ArcherData : AbilityData
{
    public float ArrowSpeed;
    public float HitRadius;
    public int PierceCount;
    public float LifeTime;
    public string EffectPath;

    public override Ability CreateAbility(CharacterCombat owner)
    {
        return new ArcherAbility(owner, this);
    }
}