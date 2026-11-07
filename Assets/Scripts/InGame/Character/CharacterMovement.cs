using System;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : NetworkBehaviour
{
    private float _moveSpeed = 4f;

    private Animator _anim;
    private Rigidbody2D _rigid;
    private PlayerInput _moveAction;
    private Vector2 _input;
    private readonly NetworkVariable<Vector2> _moveInput = new(
        Vector2.zero,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    public event Action<Vector2> OnMoveInputChanged;

    private void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
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

        _moveAction.enabled = IsOwner;
        _moveInput.OnValueChanged += HandleMoveInputChanged;
    }

    public override void OnNetworkDespawn()
    {
        _moveInput.OnValueChanged -= HandleMoveInputChanged;
    }

    private void FixedUpdate()
    {
        if (IsOwner == false)
            return;

        _rigid.linearVelocity = _input.normalized * _moveSpeed;

        Vector2 rounded = new(Mathf.Round(_input.x), Mathf.Round(_input.y));
        if (_moveInput.Value != rounded)
            _moveInput.Value = rounded;
    }

    private void OnMove(InputValue value)
    {
        if (IsOwner == false)
            return;   // 남의 캐릭터면 무시

        _input = value.Get<Vector2>();
    }

    private void HandleMoveInputChanged(Vector2 previous, Vector2 current)
    {
        OnMoveInputChanged?.Invoke(current);
    }
}