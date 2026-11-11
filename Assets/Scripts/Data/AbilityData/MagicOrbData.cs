using UnityEngine;
 
[CreateAssetMenu(menuName = "Game/Ability/MagicOrb")]
public class MagicOrbData : AbilityData
{
    [Header("구체")]
    public float Speed = 6f;
    public float HitRadius = 0.25f;
    public float Lifetime = 1f;
    public string OrbEffectPath;

    [Header("폭발")]
    public float ExplosionRadius = 1.2f;
    public bool ExplodeOnExpire = true;
    public string ExplosionEffectPath;

    public override Ability CreateAbility(CharacterCombat owner) => new MagicOrbAbility(owner, this);
}
