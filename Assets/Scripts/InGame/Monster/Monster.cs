using System;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Monster : NetworkBehaviour
{
    [Header("기초 능력치")]
    [SerializeField] private float _maxHp = 30f;
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _retargetInterval = 0.5f;
    [SerializeField] private float _exp = 10f;
    [SerializeField] private SpriteRenderer _sprite;

    [Header("공격 능력치")]
    [SerializeField] private float _attackRange = 1f;
    [SerializeField] private float _damage = 2f;
    [SerializeField] private float _attackCooldown = 1f;
    private float _attackTimer;

    private readonly NetworkVariable<float> _hp = new(30f);

    private Transform _target;
    private float _retargetTimer;

    public bool IsAlive => _hp.Value > 0f;

    public event Action<Monster> OnDespawned;

    private void Awake()
    {
        var rigid = GetComponent<Rigidbody2D>();
        rigid.bodyType = RigidbodyType2D.Kinematic;
        rigid.gravityScale = 0f;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _hp.Value = _maxHp;
            _retargetTimer = 0f;
        }
    }

    public override void OnNetworkDespawn()
    {
        OnDespawned?.Invoke(this);
        OnDespawned = null;
    }

    private void Update()
    {
        if (IsServer == false || IsAlive == false || GameManager.Instance.MatchState.IsRoundPaused.Value)
            return;

        _retargetTimer -= Time.deltaTime;
        if (_retargetTimer <= 0f)
        {
            _target = FindNearestAlivePlayer();
            _retargetTimer = _retargetInterval;
        }

        if (_target == null)
            return;

        _attackTimer -= Time.deltaTime;

        float distance = Vector2.Distance(transform.position, _target.position);
        if (distance <= _attackRange)
        {
            if (_attackTimer <= 0)
            {
                Attack();
            }
        }
        else
        {
            Vector2 dir = ((Vector2)_target.position - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(dir * (_moveSpeed * Time.deltaTime));
        }
           
    }

    private void Attack()
    {
        _attackTimer = _attackCooldown;

        if (_target.TryGetComponent<CharacterHealth>(out CharacterHealth health) == false || health.IsDowned)
            return;

        health.TakeDamage(_damage);
    }

    private Transform FindNearestAlivePlayer()
    {
        Transform nearest = null;
        float nearestSqr = float.MaxValue;

        foreach (NetworkClient client in NetworkManager.ConnectedClientsList)
        {
            NetworkObject player = client.PlayerObject;
            if (player == null)
                continue;
            if (player.TryGetComponent(out CharacterHealth health) && health.IsDowned)
                continue;

            float sqr = (player.transform.position - transform.position).sqrMagnitude;
            if (sqr < nearestSqr)
            {
                nearestSqr = sqr;
                nearest = player.transform;
            }
        }
        return nearest;
    }

    public void TakeDamage(float damage)
    {
        if (!IsServer || !IsAlive)
            return;

        _hp.Value -= damage;
        if (_hp.Value <= 0f)
            Die();
    }

    private void Die()
    {
        NetworkObject.Despawn(destroy: true);   // 모든 화면에서 사라짐
        GameManager.Instance.PartyProgress.AddProgressExp(_exp);
    }
}