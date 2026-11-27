using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Reviver : MonoBehaviour
{
    [SerializeField] private Character _character;
    [SerializeField] private float _reviveRange = 1.1f;

    private bool _isRevivePressed;

    private void Update()
    {
        if (_character.IsOwner == false)
            return;

        if (_isRevivePressed == false)
            return;

        CharacterHealth target = FindNearestDownedAlly();
        if(target != null)
        {
            _character.RequestReviveRpc(target.NetworkObject);
        }
    }

    private CharacterHealth FindNearestDownedAlly()
    {
        CharacterHealth nearest = null;
        float nearestSqr = _reviveRange * _reviveRange;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            NetworkObject player = client.PlayerObject;
            if (player == null || player == _character.NetworkObject)
                continue;

            if (!player.TryGetComponent(out CharacterHealth health) || health.IsDowned == false)
                continue;

            float sqr = (player.transform.position - transform.position).sqrMagnitude;
            if (sqr <= nearestSqr)
            {
                nearestSqr = sqr;
                nearest = health;
            }
        }
        return nearest;
    }

    private void OnRevive(InputValue value)
    {
        _isRevivePressed = value.isPressed;
        Debug.Log(_isRevivePressed);
    }
}
