using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Components;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Multiplayer;
using UnityEngine;

public class LobbyManager : SingletonBase<LobbyManager>
{
    private const int _maxPlayer = 3;
    private string _playerId => AuthenticationService.Instance.PlayerId;
    private Lobby _currentLobby;
    public Lobby CurrentLobby => _currentLobby;

    public event Action OnRoomChanged;
    private ILobbyEvents _lobbyEvents;

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async Task<bool> CreateRoom(string playerName)
    {
        try
        {
            // Relay 생성
            var data = await RelayManager.Instance.CreateRelay(_maxPlayer, "production");

            // 방 생성 옵션 설정
            var options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = new Player(
                    id: _playerId,
                    data: new Dictionary<string, PlayerDataObject>
                    {
                        {
                            "PlayerName",
                            new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)
                        }
                    }
                ),
                Data = new Dictionary<string, DataObject>
                {
                    {
                        "JoinCode",
                        new DataObject(DataObject.VisibilityOptions.Member, data.JoinCode)
                    }
                }
            };

            // 로비 생성
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(_playerId, _maxPlayer, options);
            _currentLobby = lobby;
            await SubscribeLobbyEvents();

            // 연결할 곳 설정 및 Host 시작
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(data.IPv4Address, data.Port, data.AllocationIDBytes, data.Key, data.ConnectionData);
            NetworkManager.Singleton.StartHost();

            Debug.Log("방 생성");
            return true;
        }
        catch(System.Exception e)
        {
            Debug.LogError($"[Lobby] 방 생성 실패 {e.Message}");
            return false;
        }
    }

    public async Task<bool> JoinRoom(string roomId, string playerName)
    {
        try
        {
            // 로비 참여
            var options = new JoinLobbyByIdOptions
            {
                Player = new Player(
                    id: _playerId,
                    data: new Dictionary<string, PlayerDataObject>
                    {
                        {
                            "PlayerName",
                            new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)
                        }
                    }
                ),
            };
            Lobby joinRoom = await LobbyService.Instance.JoinLobbyByIdAsync(roomId, options);
            Debug.Log($"입장하는 방 Id : {roomId}");
            _currentLobby = joinRoom;
            await SubscribeLobbyEvents();

            string joinCode = null;

            // JoinCode 찾기
            if (joinRoom.Data != null && joinRoom.Data.ContainsKey("JoinCode") && joinRoom.Data["JoinCode"] != null)
            {
                joinCode = joinRoom.Data["JoinCode"].Value;
            }

            // 참여 실패
            if (string.IsNullOrEmpty(joinCode))
            {
                Debug.Log("JoinCode가 데이터에 없습니다!");
                await LeaveRoom();
                return false;
            }

            // 연결할 곳 설정
            var data = await RelayManager.Instance.JoinRelay(joinCode, "production");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                data.IPv4Address,
                data.Port,
                data.AllocationIDBytes,
                data.Key,
                data.ConnectionData,
                data.HostConnectionData);

            if(NetworkManager.Singleton.StartClient() == false)
            {
                Debug.LogError("클라이언트 시작 실패");
                await LeaveRoom();
                return false;
            }

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Lobby] 방 입장 실패 {e.Message}");
            await LeaveRoom();
            return false;
        }
    }

    private async Task LeaveRoom()
    {
        if (_currentLobby == null)
            return;

        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_currentLobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch { }
        _currentLobby = null;
    }
    
    // 방 변경 시 실행될 이벤트 구독
    private async Task SubscribeLobbyEvents()
    {
        var callback = new LobbyEventCallbacks();

        callback.LobbyChanged += changes =>
        {
            if (_currentLobby == null)
                return;

            if (changes.LobbyDeleted)
            {
                _currentLobby = null;
            }
            else
            {
                changes.ApplyToLobby(_currentLobby);
            }
            OnRoomChanged?.Invoke();
        };

        _lobbyEvents = await LobbyService.Instance.SubscribeToLobbyEventsAsync(_currentLobby.Id, callback);
    }

    // 방 나가면 이벤트 구독 해체
    private async Task UnsubscribeLobbyEvents()
    {
        if (_lobbyEvents == null) return;
        try { await _lobbyEvents.UnsubscribeAsync(); } catch { }
        _lobbyEvents = null;
    }

    public async Task<List<Lobby>> GetRoomList()
    {
        try
        {
            var options = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse respones = await LobbyService.Instance.QueryLobbiesAsync(options);
            return respones.Results;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Lobby] 방 목록 조회 실패 : {e.Message}");
            return new List<Lobby>();
        }
    }
}
