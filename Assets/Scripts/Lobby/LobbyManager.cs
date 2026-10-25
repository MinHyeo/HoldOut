using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Multiplayer;
using Unity.Netcode;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;
using System.Threading.Tasks;

public class LobbyManager : SingletonBase<LobbyManager>
{
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
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(_playerId, 3, options);
            _currentLobby = lobby;

            // Relay 생성
            var data = await RelayManager.Instance.CreateRelay(3, "production");

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
}
