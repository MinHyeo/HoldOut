using TMPro;
using UnityEngine;

public class PlayerSlot : UIBase
{
    [SerializeField] private TextMeshProUGUI _nameText;

    public void SetPlayerName(string name)
    {
        Debug.Log($"{name}");
        _nameText.text = name;
    }
}
