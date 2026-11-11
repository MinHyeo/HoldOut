using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterCombat : NetworkBehaviour 
{
    private readonly List<Ability> _abilities = new();

    private readonly Dictionary<int, ProjectileEffect> _projectileEffects = new();

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
            ability.OnUpdate(Time.deltaTime);
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

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayProjectileClientRpc(int arrowId, Vector2 origin, Vector2 direction, float speed, float lifeTime, string effectPath)
    {
        GameObject obj = GameObjectManager.Instance.CreateObject(effectPath, origin);
        if (obj == null || obj.TryGetComponent(out ProjectileEffect projectile) == false)
            return;

        // 화살 회전
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        obj.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // id로 기억해두기
        _projectileEffects[arrowId] = projectile;

        // 받은 방향·속도·수명으로 날리기. 끝나면 목록에서 제거
        projectile.Play(direction, speed, lifeTime, () => _projectileEffects.Remove(arrowId));
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ProjectileHitClientRpc(int arrowId)
    {
        // 서버에서 맞았다고 알려주면 그 화살 그림을 바로 사라지게
        if (_projectileEffects.TryGetValue(arrowId, out ProjectileEffect arrow))
        {
            arrow.Finish();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ExplodeClientRpc(int id, Vector2 center, float radius, string explosionEffectPath)
    {
        if (_projectileEffects.TryGetValue(id, out ProjectileEffect projectile))
            projectile.Finish();

        GameObject obj = GameObjectManager.Instance.CreateObject(explosionEffectPath, center);
        if (obj != null && obj.TryGetComponent(out ExplosionEffect explosion))
            explosion.Play(radius);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3f);   // 사거리
    }
}