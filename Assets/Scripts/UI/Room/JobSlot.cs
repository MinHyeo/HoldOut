using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class JobSlot : MonoBehaviour 
{
    private string _jobId;

    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Slider _atkSlider;
    [SerializeField] private Slider _defSlider;
    [SerializeField] private UIButton _button;

    public event Action OnClickJobButton;

    private void OnEnable()
    {
        _button.BindOnClickButtonEvent(OnClickButton);
    }

    private void OnDisable()
    {
        _button.UnBindOnClickButtonEvent(OnClickButton);
    }

    public void UpdateSlot(JobData jobData)
    {
        _jobId = jobData.Id;
        _icon.sprite = jobData.JobIcon;
        _nameText.text = jobData.name;

        _hpSlider.value = jobData.hp / 10f;
        _atkSlider.value = jobData.power / 10f;
        _defSlider.value = jobData.defence / 10f;
    }

    private void OnClickButton()
    {
        var matchState = GameManager.Instance.MatchState;
        matchState.RequestSelectJob(_jobId);

        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.JobSelectPopup);
        //OnClickJobButton?.Invoke();
    }
}