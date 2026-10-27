using UnityEngine;

[CreateAssetMenu(menuName = "Game/JobData", fileName = "JobData")]
public class JobData : GameDataBase
{
    public string JobName;
    public Sprite JobIcon;
    public int hp;
    public int power;
    public int defence;
}