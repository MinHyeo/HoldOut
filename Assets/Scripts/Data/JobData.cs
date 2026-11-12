using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/JobData", fileName = "JobData")]
public class JobData : GameDataBase
{
    public string JobName;
    public Sprite JobIcon;
    public AnimatorOverrideController AnimatorController;
    public float Hp;
    public float MoveSpeed;
    public float Power;
    public float Defence;
    public List<AbilityData> AbilityDatas;
}