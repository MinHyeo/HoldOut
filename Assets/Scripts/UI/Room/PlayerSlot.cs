using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlot : UIBase
{
    [SerializeField] private Image _jobIcon;
    [SerializeField] private TextMeshProUGUI _nameText;

    public void SetPlayerName(string name)
    {
        Debug.Log($"{name}");
        _nameText.text = name;
    }

    public void SetJobIcon(Sprite icon)
    {
        _jobIcon.sprite = icon;
    }
}
