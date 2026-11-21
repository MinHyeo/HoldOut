using Unity.Netcode;
using UnityEngine;

public class HealPulseAbility : Ability
{
    private readonly HealPulseData _data;

    public HealPulseAbility(CharacterCombat owner, HealPulseData data) : base(owner, data)
    {
        _data = data;
    }

    protected override bool TryActivate()
    {
        Vector2 center = Owner.transform.position;
        float radiusSqr = _data.range * _data.range;
        float amount = Owner.Character.Stats.Power * (_data.damage / 100f);

        foreach(NetworkClient client in Owner.NetworkManager.ConnectedClientsList)
        {
            var playerObject = client.PlayerObject;
            if (playerObject == null)
                continue;

            float distance = ((Vector2)playerObject.transform.position - center).sqrMagnitude;
            if (distance > radiusSqr)
                continue;

            if(playerObject.TryGetComponent<CharacterHealth>(out CharacterHealth health))
            {
                health.Heal(amount);
            }
        }

        return true;
    }

    public override void OnUpdate(float deltaTime)
    {

    }
}
