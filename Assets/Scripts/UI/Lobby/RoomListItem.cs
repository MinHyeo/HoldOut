using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private Button _joinButton;

    public void Set(string roomName, int current, int max, Action onClick)
    {
        _nameText.text = roomName;
        _countText.text = $"{current}/{max}";
        _joinButton.onClick.RemoveAllListeners();
        _joinButton.onClick.AddListener(() => onClick());
    }
}
