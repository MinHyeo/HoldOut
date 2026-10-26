using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class LobbyView : UIBase
{
    [Header("Name Input")]
    [SerializeField] private TMP_InputField _nameInput;

    [Header("Room List")]
    [SerializeField] private GameObject _roomPrefab;
    [SerializeField] private Transform _content;
    [SerializeField] private UIButton _refreshButton;
    private readonly List<RoomListItem> _items = new List<RoomListItem>();

    [Header("Buttons")]
    [SerializeField] private UIButton _createRoomButton;
    [SerializeField] private UIButton _joinRoomButton;

    private void OnEnable()
    {
        Invoke("RoomListRefresh", 1f);
        _refreshButton.BindOnClickButtonEvent(RoomListRefresh);

        _createRoomButton.BindOnClickButtonEvent(OnClickCreateRoomButton);
        _joinRoomButton.BindOnClickButtonEvent(OnClickJoinRoomButton);
    }

    private void OnDisable()
    {
        _refreshButton.UnBindOnClickButtonEvent(RoomListRefresh);
        _createRoomButton.UnBindOnClickButtonEvent(OnClickCreateRoomButton);
        _joinRoomButton.UnBindOnClickButtonEvent(OnClickJoinRoomButton);
    }

    private async void RoomListRefresh()
    {
        List<Lobby> rooms = await LobbyManager.Instance.GetRoomList();

        foreach (var item in _items)
        {
            Destroy(item.gameObject);
        }
        _items.Clear();

        foreach (var room in rooms)
        {
            GameObject item = Instantiate(_roomPrefab, _content);
            RoomListItem itemComponent = item.GetComponent<RoomListItem>();
            if (itemComponent == null)
                return;

            itemComponent.Set(room.Name, room.Players.Count, room.MaxPlayers, () => OnClickRoom(room.Id));
            _items.Add(itemComponent);
        }
    }

    private async void OnClickRoom(string roomId)
    {
        string name = _nameInput.text;
        if(await LobbyManager.Instance.JoinRoom(roomId, name))
        {
            UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.LobbyView);
            UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.RoomView);
        }
    }

    private async void OnClickCreateRoomButton()
    {
        // 방 만들어지는 동안 비활성화
        _createRoomButton.SetInteractable(false);
        string name = _nameInput.text;
        bool isSuccess = await LobbyManager.Instance.CreateRoom(name);
        _createRoomButton.SetInteractable(true);

        // 룸 UI로 전환
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
