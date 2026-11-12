using System;
using UnityEngine;

public class CharacterStats
{
    public float MaxHp { get; private set; }
    public float MoveSpeed { get; private set; }
    public float Power { get; private set; }
    public float Defence { get; private set; }

    public event Action OnValueChanged;

    public void InitStat(JobData data)
    {
        MaxHp = data.Hp;
        MoveSpeed = data.MoveSpeed;
        Power = data.Power;
        Defence = data.Defence;
    }

    public void AddMaxHp(float amount)
    {
        MaxHp = MaxHp + amount;
        OnValueChanged?.Invoke();
    }

    public void AddMoveSpeed(float pecent)
    {
        MoveSpeed = MoveSpeed + MoveSpeed * (pecent / 100);
        OnValueChanged?.Invoke();
    }

    public void AddPower(float pecent)
    {
        Power = Power + Power * (pecent / 100);
        OnValueChanged?.Invoke();
    }

    public void AddDefence(float pecent)
    {
        Defence = Defence + Power * (pecent / 100);
        OnValueChanged?.Invoke();
    }
}
