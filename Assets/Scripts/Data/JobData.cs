using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/JobData", fileName = "JobData")]
public class JobData : GameDataBase
{
    public string JobName;
    public Sprite JobIcon;
    public AnimatorOverrideController AnimatorController;
    public int Hp;
    public int Power;
    public int Defence;
    public List<AbilityData> AbilityDatas;
}