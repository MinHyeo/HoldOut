using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class JobSlot : MonoBehaviour 
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Slider _atkSlider;
    [SerializeField] private Slider _defSlider;
    [SerializeField] private UIButton _button;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public void UpdateSlot(JobData jobData)
    {
        _icon.sprite = jobData.JobIcon;
        _nameText.text = jobData.name;

        _hpSlider.value = jobData.hp / 10f;
        _atkSlider.value = jobData.power / 10f;
        _defSlider.value = jobData.defence / 10f;
    }
}