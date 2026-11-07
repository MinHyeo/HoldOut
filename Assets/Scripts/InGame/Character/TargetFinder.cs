using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TargetFinder
{
    private static readonly List<Collider2D> _hits = new(128);
    private static readonly int MonsterMask = LayerMask.GetMask("Monster");
    private static ContactFilter2D _filter;
    private static bool _initialized = false;

    private static void EnsureInitialized()
    {
        if (_initialized) 
            return;

        _filter = new ContactFilter2D();
        _filter.SetLayerMask(MonsterMask);
        _filter.useTriggers = true;
        _initialized = true;
    }

    public static Monster FindNearest(Vector2 origin, float range) 
    {
        EnsureInitialized();

        Physics2D.OverlapCircle(origin, range, _filter, _hits);

        Monster nearest = null;
        float nearestSqr = float.MaxValue;

        foreach (var hit in _hits)
        {
            if (hit.TryGetComponent(out Monster monster) == false) 
                continue;

            float sqr = ((Vector2)hit.transform.position - origin).sqrMagnitude;
            if (sqr < nearestSqr)
            {
                nearestSqr = sqr;
                nearest = monster;
            }
        }

        return nearest;
    }

    public static void FindInRange(Vector2 origin, float range, List<Monster> targets)
    {
        EnsureInitialized();
        _hits.Clear();

        Physics2D.OverlapCircle(origin, range, _filter, _hits);
        foreach (Collider2D hit in _hits)
        {
            if (hit.TryGetComponent(out Monster monster))
                targets.Add(monster);
        }
    }
}
