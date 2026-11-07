using UnityEngine;

public abstract class AbilityData : ScriptableObject
{
    public string Id;
    public float Cooldown = 1f;
    public float damage = 10f;
    public float range = 3f;

    public abstract Ability CreateAbility(CharacterCombat owner); 
}