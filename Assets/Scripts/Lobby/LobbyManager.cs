using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Multiplayer;
using Unity.Netcode;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;
using System.Collections.Generic;

public class LobbyManager : SingletonBase<LobbyManager>
{
    private const int _maxPlayer = 3;
    private string _playerId => AuthenticationService.Instance.PlayerId;
    private Lobby _currentLobby;
    public Lobby CurrentLobby => _currentLobby;

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async Task<bool> CreateRoom()
    {
        try
        {
            // 방 생성 옵션 설정
            var options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = new Player(_playerId)
            };

            // 로비 생성
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(_playerId, _maxPlayer, options);
            _currentLobby = lobby;

            // Relay 생성
            var data = await RelayManager.Instance.CreateRelay(_maxPlayer, "production");

            // 연결할 곳 설정 및 Host 시작
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(data.IPv4Address, data.Port, data.AllocationIDBytes, data.Key, data.ConnectionData);
            NetworkManager.Singleton.StartHost();

            Debug.Log("방 생성");
            return true;
        }
        catch(System.Exception e)
        {
            Debug.LogError(e);
            return false;
        }
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
