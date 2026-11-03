using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : NetworkBehaviour
{
    private float _moveSpeed = 4f;

    private Rigidbody2D _rigid;
    private PlayerInput _moveAction;
    private Vector2 _input;

    private void Awake()
    {
        _moveAction = GetComponent<PlayerInput>();
        _rigid = GetComponent<Rigidbody2D>();
        _rigid.gravityScale = 0;
        _rigid.freezeRotation = true;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            _rigid.bodyType = RigidbodyType2D.Dynamic;
        }
        else
        {
            _rigid.bodyType = RigidbodyType2D.Kinematic;
        }

        _moveAction.enabled = IsO;
    }

    private void FixedUpdate()
    {
        if (IsOwner == false)
            return;

        _rigid.linearVelocity = _input.normalized * _moveSpeed;
    }

    private void OnMove(InputValue value)
    {
        if (IsOwner == false)
            return;   // 남의 캐릭터면 무시

        _input = value.Get<Vector2>();
    }
}