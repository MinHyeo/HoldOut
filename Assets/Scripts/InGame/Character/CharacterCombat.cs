using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterCombat : NetworkBehaviour 
{
    private readonly List<Ability> _abilities = new();

    public void InitAbilities(JobData data)
    {
        if (IsServer == false)
            return;

        _abilities.Clear();
        foreach(var abilityData in data.AbilityDatas)
        {
            AddAbility(abilityData);
        }
    }

    public void AddAbility(AbilityData data)
    {
        _abilities.Add(data.CreateAbility(this));
    }

    private void Update()
    {
        if (IsServer == false)
            return;

        foreach(var ability in _abilities)
        {
            ability.Tick(Time.deltaTime);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayWarriorClientRpc(Vector2 dir)
    {
        Debug.Log($"공격! {dir.x}, {dir.y}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3f);   // 사거리
    }
}