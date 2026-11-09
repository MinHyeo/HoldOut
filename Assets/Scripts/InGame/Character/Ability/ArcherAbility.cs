using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArcherAbility : Ability
{
    private class ArrowState
    {
        public int Id;
        public Vector2 Position;
        public Vector2 Direction;
        public float RemainTime;
        public int RemainPierce;
        public readonly HashSet<Monster> HitMonsters = new();
    }

    private readonly ArcherData _data;
    private readonly List<ArrowState> _arrows = new();
    private readonly List<Monster> _targets = new();

    private int _nextArrowId;
    private int _bonusPierce;

    public ArcherAbility(CharacterCombat ower, ArcherData data) : base(ower, data)
    {
        _data = data;
    }

    protected override bool TryActivate()
    {
        Vector2 origin = Owner.transform.position;
        float range = _data.ArrowSpeed * _data.LifeTime;

        Monster target = TargetFinder.FindNearest(origin, range);
        if (target == null) return false;

        Vector2 dir = ((Vector2)target.transform.position - origin).normalized;

        var arrow = new ArrowState
        {
            Id = _nextArrowId++,
            Position = origin,
            Direction = dir,
            RemainTime = _data.LifeTime,
            RemainPierce = _data.PierceCount + _bonusPierce
        };
        _arrows.Add(arrow);

        // 모두에게 "이 화살을 이 위치에서 이 방향으로 쐈다"만 알림
        Owner.PlayArrowClientRpc(arrow.Id, origin, dir, _data.ArrowSpeed, _data.LifeTime, _data.EffectPath);
        return true;
    }

    public override void OnUpdate(float deltaTime)
    {
        // 뒤에서부터 돌아야 제거해도 인덱스가 안 꼬임
        for (int i = _arrows.Count - 1; i >= 0; i--)
        {
            ArrowState arrow = _arrows[i];
            float distance = _data.ArrowSpeed * deltaTime;

            // 이번 프레임에 지나간 구간 전체를 검사 (빠른 화살이 적을 건너뛰지 않게)
            TargetFinder.FindAlongPath(arrow.Position, arrow.Direction, distance, _data.HitRadius, _targets);

            bool removed = false;
            foreach (Monster monster in _targets)
            {
                if (!arrow.HitMonsters.Add(monster)) continue;   // 이미 맞춘 적

                monster.TakeDamage(_data.damage);

                if (arrow.RemainPierce <= 0)
                {
                    RemoveArrow(i, arrow, hit: true);
                    removed = true;
                    break;
                }
                arrow.RemainPierce--;
            }
            if (removed) continue;

            arrow.Position += arrow.Direction * distance;
            arrow.RemainTime -= deltaTime;

            if (arrow.RemainTime <= 0f)
                RemoveArrow(i, arrow, hit: false);   // 사거리 끝: 클라이언트도 lifetime으로 알아서 사라짐
        }
    }

    private void RemoveArrow(int index, ArrowState arrow, bool hit)
    {
        _arrows.RemoveAt(index);
        if (hit)
        {
            Owner.ArrowHitClientRpc(arrow.Id);   // 맞아서 사라질 때만 알림
        }
    }
}