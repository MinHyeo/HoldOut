using System.Collections.Generic;
using UnityEngine;

public class MagicOrbAbility : Ability
{
    private class OrbState
    {
        public int Id;
        public Vector2 Position;
        public Vector2 Direction;
        public float RemainTime;
    }

    private readonly MagicOrbData _data;
    private readonly List<OrbState> _orbs = new();
    private readonly List<Monster> _targets = new();
    private int _nextOrbId;

    public MagicOrbAbility(CharacterCombat owner, MagicOrbData data) : base(owner, data)
    {
        _data = data;
    }


    protected override bool TryActivate()
    {
        Vector2 origin = Owner.transform.position;
        float range = _data.Speed * _data.Lifetime;

        Monster target = TargetFinder.FindNearest(origin, range);
        if (target == null)
            return false;

        Vector2 dir = ((Vector2)target.transform.position - origin).normalized;

        var orb = new OrbState
        {
            Id = _nextOrbId++,
            Position = origin,
            Direction = dir,
            RemainTime = _data.Lifetime
        };
        _orbs.Add(orb);

        Owner.PlayProjectileClientRpc(orb.Id, origin, dir, _data.Speed, _data.Lifetime, _data.OrbEffectPath);
        return true;
    }

    public override void OnUpdate(float deltaTime)
    {
        for (int i = _orbs.Count - 1; i >= 0; i--)
        {
            OrbState orb = _orbs[i];
            float distance = _data.Speed * deltaTime;

            // 이번 프레임에 지나가는 길에 몬스터가 있는지
            TargetFinder.FindAlongPath(orb.Position, orb.Direction, distance, _data.HitRadius, _targets);

            if (_targets.Count > 0)
            {
                // 처음 닿은 몬스터 위치에서 폭발
                Explode(i, orb, _targets[0].transform.position);
                continue;
            }

            orb.Position += orb.Direction * distance;
            orb.RemainTime -= deltaTime;

            if (orb.RemainTime <= 0f)
            {
                if (_data.ExplodeOnExpire)
                    Explode(i, orb, orb.Position);
                else
                    _orbs.RemoveAt(i);
            }
        }
    }

    private void Explode(int index, OrbState orb, Vector2 center)
    {
        _orbs.RemoveAt(index);

        // 폭발 범위 안의 모든 적에게 피해
        TargetFinder.FindInRange(center, _data.ExplosionRadius, _targets);
        float damage = _data.damage;
        foreach (Monster monster in _targets)
            monster.TakeDamage(damage);

        Owner.ExplodeClientRpc(orb.Id, center, _data.ExplosionRadius, _data.ExplosionEffectPath);
    }
}
