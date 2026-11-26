using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class ExpBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    private void OnEnable()
    {
        _slider.value = 0;
        GameManager.Instance.PartyProgress.PartyExp.OnValueChanged += HandleExpChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.PartyProgress.PartyExp.OnValueChanged -= HandleExpChanged;
    }

    private void HandleExpChanged(float previous, float current)
    {
        float maxHp = GameManager.Instance.PartyProgress.MaxExp;
        _slider.value = current / maxHp;
    }
}
