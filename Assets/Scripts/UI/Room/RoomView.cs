using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class RoomView : UIBase
{
    private MatchState _matchState;

    [SerializeField] private TextMeshProUGUI _joinCodeText;
    [SerializeField] private List<PlayerSlot> _playerSlotList;

    [SerializeField] private UIButton _selectJobButton;

    private void OnEnable()
    {
        _matchState = GameManager.Instance.MatchState;
        _matchState.OnPlayerChanged += UpdatePlayerSlot;

        string code = LobbyManager.Instance.CurrentLobby.LobbyCode;
        _joinCodeText.text = code;

        _selectJobButton.BindOnClickButtonEvent(OpenSelectJobPopup);

        LobbyManager.Instance.OnRoomChanged += UpdatePlayerSlot;
        UpdatePlayerSlot();

        
    }

    private void OnDisable()
    {
        LobbyManager.Instance.OnRoomChanged -= UpdatePlayerSlot;
        _matchState.OnPlayerChanged -= UpdatePlayerSlot;

        _selectJobButton.UnBindOnClickButtonEvent(OpenSelectJobPopup);
    }

    private void UpdatePlayerSlot()
    {
        List<RoomPlayerState> players = _matchState.GetPlayers();
        ulong localId = NetworkManager.Singleton.LocalClientId;

        for(int i = 0; i < _playerSlotList.Count; i++)
        {
            if(i < players.Count)
            {
                var player = players[i];
                _playerSlotList[i].SetPlayerName(player.PlayerName.ToString());

                var data = GameDataManager.Instance.GetData<JobData>(LabelType.Job, player.JobId.ToString());
                _playerSlotList[i].SetJobIcon(data.JobIcon);
            }
        }
    }

    private void OpenSelectJobPopup()
    {
        var uiBase = UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.JobSelectPopup);
        if(uiBase is JobSelectPopup selectPopup)
        {
            selectPopup.OnJobSelected += UpdatePlayerSlot;
        }
    }
}