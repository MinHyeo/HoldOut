using System;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Monster : NetworkBehaviour
{
    [SerializeField] private float _maxHp = 30f;
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _retargetInterval = 0.5f;
    [SerializeField] private SpriteRenderer _sprite;

    private readonly NetworkVariable<float> _hp = new(30f);

    private Transform _target;
    private float _retargetTimer;

    public bool IsAlive => _hp.Value > 0f;

    public event Action<Monster> OnDespawned;

    private void Awake()
    {
        var rigid = GetComponent<Rigidbody2D>();
        rigid.bodyType = RigidbodyType2D.Kinematic;   // 물리로 밀지 않고 transform으로 이동
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
        if (!IsServer || !IsAlive)
            return;

        _retargetTimer -= Time.deltaTime;
        if (_retargetTimer <= 0f)
        {
            _target = FindNearestAlivePlayer();
            _retargetTimer = _retargetInterval;
        }

        if (_target == null)
            return;

        Vector2 dir = ((Vector2)_target.position - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(dir * (_moveSpeed * Time.deltaTime));
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
    }
}