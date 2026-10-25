using UnityEngine;

public class LobbyView : UIBase
{
    [SerializeField] private UIButton _createRoomButton;
    [SerializeField] private UIButton _joinRoomButton;

    private void OnEnable()
    {
        _createRoomButton.BindOnClickButtonEvent(OnClickCreateRoomButton);
        _joinRoomButton.BindOnClickButtonEvent(OnClickJoinRoomButton);
    }

    private void OnDisable()
    {
        _createRoomButton.UnBindOnClickButtonEvent(OnClickCreateRoomButton);
        _joinRoomButton.UnBindOnClickButtonEvent(OnClickJoinRoomButton);
    }

    private async void OnClickCreateRoomButton()
    {
        // 방 만들어지는 동안 비활성화
        _createRoomButton.SetInteractable(false);
        bool isSuccess = await LobbyManager.Instance.CreateRoom();
        _createRoomButton.SetInteractable(true);

        if (isSuccess)
        {
            UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.LobbyView);
            UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.RoomView);
        }
    }

    private void OnClickJoinRoomButton()
    {

    }
}
