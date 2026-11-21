using TMPro;
using UnityEngine;

public class CreateRoomPopup : UIBase
{
    [SerializeField] private TMP_InputField _roomNameInput;
    [SerializeField] private UIButton _createButton;
    [SerializeField] private UIButton _exitButton;

    private string _playerName;

    private void OnEnable()
    {
        _roomNameInput.text = string.Empty;

        _createButton.BindOnClickButtonEvent(OnClickCreateButton);
        _exitButton.BindOnClickButtonEvent(OnClickExitButton);
    }

    private void OnDisable()
    {
        _createButton.UnBindOnClickButtonEvent(OnClickCreateButton);
        _exitButton.UnBindOnClickButtonEvent(OnClickExitButton);
    }

    public void OpenCreateRoomPopup(string playerName)
    {
        _playerName = playerName;
    }

    private async void OnClickCreateButton()
    {
        if (_roomNameInput.text == string.Empty)
            return;

        string roomName = _roomNameInput.text;
        bool isSuccessed = await LobbyManager.Instance.CreateRoom(roomName, _playerName);
        if (isSuccessed)
        {
            UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.LobbyView);
            UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.CreateRoomPopup);
            UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.RoomView);
        }
    }

    private void OnClickExitButton()
    {
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.CreateRoomPopup);
    }
}
