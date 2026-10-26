using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomView : UIBase
{
    [SerializeField] private TextMeshProUGUI _joinCodeText;

    [SerializeField] private List<PlayerSlot> _playerSlotList;

    private void OnEnable()
    {
        string code = LobbyManager.Instance.CurrentLobby.LobbyCode;
        _joinCodeText.text = code;

        LobbyManager.Instance.OnRoomChanged += UpdatePlayerSlot;
        UpdatePlayerSlot();
    }

    private void OnDisable()
    {
        LobbyManager.Instance.OnRoomChanged -= UpdatePlayerSlot;
    }

    private void UpdatePlayerSlot()
    {
        List<Player> players = LobbyManager.Instance.CurrentLobby.Players;

        for(int i = 0; i < _playerSlotList.Count; i++)
        {
            string name = string.Empty;
            if (i < players.Count)
            {
                var player = players[i];
                PlayerDataObject nameData = null;              

                if (player.Data != null && player.Data.TryGetValue("PlayerName", out nameData))
                {
                    name = nameData.Value;
                }
                else
                {
                    name = "Empty";
                }
            }
            _playerSlotList[i].SetPlayerName(name);
        }
    }
}