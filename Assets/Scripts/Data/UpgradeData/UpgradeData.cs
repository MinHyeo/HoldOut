using UnityEngine;

public abstract class UpgradeData : GameDataBase
{
    public string UpgradeName;
    public string Description;

    public abstract void Apply(Character target);
}