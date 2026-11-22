using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LevelUpPopup : UIBase
{
    [SerializeField] private List<UpgradeCardButton> buttonList;

    private void OnEnable()
    {
        foreach(var button in buttonList)
        {
            int count = System.Enum.GetValues(typeof(StatType)).Length;
            int randomSelect = Random.Range(0, count);

            button.SetUpgradeCard((StatType)randomSelect);
            button.OnClickUpgradeCard += OnClickButton;
        }
    }

    private void OnDisable()
    {
        foreach (var button in buttonList)
        {
            button.OnClickUpgradeCard -= OnClickButton;
        }
    }

    private void OnClickButton(StatType type)
    {
        string path = $"Data/Job/UpgradeStat/{type.ToString()}";
        var data = GameDataManager.Instance.GetData<UpgradeData>(path);
        var character = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Character>();
        data.Apply(character);

        GameManager.Instance.MatchState.IsRoundPaused.Value = false;
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.LevelUpPopup);
    }
}
