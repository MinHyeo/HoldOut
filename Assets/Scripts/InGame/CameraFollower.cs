using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    private Transform _target;
    private Vector3 _velocity;

    private Vector3 _offset = new(0f, 0f, -10f);
    private float _smoothTime = 0.2f;

    private void Start()
    {
        GameManager.Instance.MatchState.Phase.OnValueChanged += HandlePhaseChaged;
    }

    private void OnDestroy()
    {
        GameManager.Instance.MatchState.Phase.OnValueChanged -= HandlePhaseChaged;
    }

    private void HandlePhaseChaged(GamePhase previous, GamePhase current)
    {
        if(current == GamePhase.InGame)
        {
            var playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
            if (playerObject == null)
                return;

            _target = playerObject.transform;
        }
    }

    private void LateUpdate()
    {
        if (_target == null)
            return;

        Vector3 desiredPosition = _target.position + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, _smoothTime);
    }
}
