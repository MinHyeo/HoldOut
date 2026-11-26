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

    public void SetMaxHp(float maxHp)
    {
        if (!IsServer)
            return;

        _maxHp.Value = maxHp;
        _hp.Value = maxHp;
    }

    public float Heal(float amount)
    {
        if (!IsServer || IsDowned || amount <= 0f)
            return 0f;

        float before = _hp.Value;
        _hp.Value = Mathf.Min(_maxHp.Value, before + amount);
        return _hp.Value - before;
    }

    public void TakeDamage(float amount)
    {
        if (!IsServer || IsDowned || amount <= 0f)
            return;

        _hp.Value = Mathf.Max(0f, _hp.Value - amount);
        Debug.Log($"공격 받음 : {_hp.Value}");
    }
}