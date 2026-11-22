using System;
using TMPro;
using UnityEngine;

public class UpgradeCardButton : MonoBehaviour
{
    [SerializeField] private UIButton _button;
    [SerializeField] private TextMeshProUGUI _upgardeNameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    private StatType _statType;

    public event Action<StatType> OnClickUpgradeCard;

    private void OnEnable()
    {
        _button.BindOnClickButtonEvent(OnClickCard);
    }

    private void OnDisable()
    {
        _button.UnBindOnClickButtonEvent(OnClickCard);
    }

    public void SetUpgradeCard(StatType statType)
    {
        _statType = statType;
        string path = $"Data/Job/UpgradeStat/{statType.ToString()}";
        var statData = GameDataManager.Instance.GetData<UpgradeData>(path);

        _upgardeNameText.text = statData.UpgradeName;
        _descriptionText.text = statData.Description;
    }

    private void OnClickCard()
    {
        OnClickUpgradeCard?.Invoke(_statType);
    }
}