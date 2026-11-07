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
    public void PlayWarriorClientRpc(Vector2 dir, string abilityId)
    {
        Debug.Log($"공격! {dir.x}, {dir.y}");
        var data = GameDataManager.Instance.GetData<AbilityData>(abilityId);
        var warriorData = (WarriorData)data;

        Vector3 spawnPosition = new Vector3(dir.x, dir.y, 0);
        GameObject effect = GameObjectManager.Instance.CreateObject(warriorData.effectPath, Vector3.zero);
        effect.transform.SetParent(transform);
        effect.transform.localPosition = spawnPosition;

        if(effect.TryGetComponent(out WarriorEffect warrior))
        {
            warrior.Play(dir, warriorData.halfAngle, 0.5f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3f);   // 사거리
    }
}