using System;
using UnityEngine;

public class CharacterStats
{
    public float MaxHp { get; private set; }
    public float MoveSpeed { get; private set; }
    public float Power { get; private set; }

    public event Action OnValueChanged;

    public void InitStat(JobData data)
    {
        MaxHp = data.Hp;
        MoveSpeed = data.MoveSpeed;
        Power = data.Power;
    }

    public void ApplyPercentBonus(StatType type, float percent)
    {
        switch (type)
        {
            case StatType.MaxHp:
                AddMaxHp(percent);
                break;
            case StatType.AttackPower:
                AddPower(percent);
                break;
            case StatType.AttackRange:
                break;
            case StatType.AttackSpeed:
                break;
            case StatType.MoveSpeed:
                AddMoveSpeed(percent);
                break;
        }
    }

    private void AddMaxHp(float amount)
    {
        MaxHp = MaxHp + amount;
        OnValueChanged?.Invoke();
    }

    private void AddMoveSpeed(float pecent)
    {
        MoveSpeed = MoveSpeed + MoveSpeed * (pecent / 100);
        OnValueChanged?.Invoke();
    }

    private void AddPower(float pecent)
    {
        Power = Power + Power * (pecent / 100);
        OnValueChanged?.Invoke();
    }
}
