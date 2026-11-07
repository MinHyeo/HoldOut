using System.Collections.Generic;
using UnityEngine;

public class WarriorAbility : Ability
{
    private readonly WarriorData _data;
    private readonly List<Monster> _targets = new();

    public WarriorAbility(CharacterCombat ower, WarriorData data) : base(ower, data)
    {
        _data = data;
    }

    protected override bool TryActivate()
    {
        Debug.Log("공격 시도");

        Vector2 origin = Owner.transform.position;
        Monster target = TargetFinder.FindNearest(origin, _data.range);
        if (target == null)
            return false;

        Vector2 dir = ((Vector2)target.transform.position - origin).normalized;
        TargetFinder.FindInRange(origin, _data.range, _targets);
        foreach (Monster monster in _targets)
        {
            Vector2 to = (Vector2)monster.transform.position - origin;
            //if (Vector2.Angle(dir, to) <= _data.halfAngle)
                //monster.TakeDamage(_data.damage);
        }

        Owner.PlayWarriorClientRpc(dir);
        return true;
    }
}