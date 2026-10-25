using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomView : UIBase
{
    [SerializeField] TextMeshProUGUI _joinCodeText;

    private void OnEnable()
    {
        string code = LobbyManager.Instance.CurrentLobby.LobbyCode;
        _joinCodeText.text = code;
    }
}