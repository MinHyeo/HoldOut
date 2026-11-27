using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoSlot : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private GameObject _reviveIcon;
    [SerializeField] private Slider _hpSlider;

    private CharacterHealth _health;

    private void OnDisable()
    {
        if (_health != null)
        {
            _health.OnHpChanged -= HandleHpChanged;
            _health = null;
        }
    }

    public void SetSlot(NetworkClient client)
    {
        var character = client.PlayerObject.GetComponent<Character>();
        if (character == null)
            return;

        var jobId = character.JobId.Value.ToString();
        JobData jobData = GameDataManager.Instance.GetData<JobData>(jobId);
        _icon.sprite = jobData.JobIcon;

        _health = character.GetComponent<CharacterHealth>();
        _health.OnHpChanged += HandleHpChanged;
        HandleHpChanged(_health.Hp, _health.MaxHp);
    }

    private void HandleHpChanged(float hp, float maxHp)
    {
        _hpSlider.maxValue = maxHp;
        _hpSlider.value = hp;

        _reviveIcon.SetActive(hp >= 0);
    }
}
