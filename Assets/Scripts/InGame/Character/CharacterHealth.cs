using System;
using Unity.Netcode;
using UnityEngine;

public class CharacterHealth : NetworkBehaviour
{
    private readonly NetworkVariable<float> _hp = new(100f);
    private readonly NetworkVariable<float> _maxHp = new(100f);

    public float Hp => _hp.Value;
    public float MaxHp => _maxHp.Value;
    public bool IsDowned => _hp.Value <= 0f;
    public bool IsFull => _hp.Value >= _maxHp.Value;

    /// <summary>(현재 체력, 최대 체력). HP바 같은 UI가 구독한다.</summary>
    public event Action<float, float> OnHpChanged;

    public override void OnNetworkSpawn()
    {
        _hp.OnValueChanged += HandleChanged;
        _maxHp.OnValueChanged += HandleChanged;
        OnHpChanged?.Invoke(_hp.Value, _maxHp.Value);
    }

    public override void OnNetworkDespawn()
    {
        _hp.OnValueChanged -= HandleChanged;
        _maxHp.OnValueChanged -= HandleChanged;
    }

    private void HandleChanged(float previous, float current)
    {
        OnHpChanged?.Invoke(_hp.Value, _maxHp.Value);
    }

    // ───────── 서버 전용 ─────────

    /// <summary>직업 적용 시. 최대 체력을 정하고 가득 채운다.</summary>
    public void SetMaxHp(float maxHp)
    {
        if (!IsServer)
            return;

        _maxHp.Value = maxHp;
        _hp.Value = maxHp;
    }

    /// <summary>회복. 쓰러진 상태는 회복하지 않는다. 실제로 회복된 양을 반환.</summary>
    public float Heal(float amount)
    {
        if (!IsServer || IsDowned || amount <= 0f)
            return 0f;

        float before = _hp.Value;
        _hp.Value = Mathf.Min(_maxHp.Value, before + amount);
        return _hp.Value - before;
    }

    /// <summary>피해. 0이 되면 쓰러짐(부활은 나중에 구현).</summary>
    public void TakeDamage(float amount)
    {
        if (!IsServer || IsDowned || amount <= 0f)
            return;

        _hp.Value = Mathf.Max(0f, _hp.Value - amount);
    }
}