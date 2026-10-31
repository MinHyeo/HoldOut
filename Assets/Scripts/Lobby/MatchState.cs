using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public enum GamePhase : byte 
{
    Room,
    InGame,
}


public class MatchState : NetworkBehaviour
{
    private NetworkVariable<GamePhase> _phase { get; } = new(GamePhase.Room);
    private NetworkList<RoomPlayerState> _players;

    public event Action OnPlayerChanged;

    private void Awake()
    {
        _players = new NetworkList<RoomPlayerState>();
    }

    public override void OnNetworkSpawn()
    {
        _players.OnListChanged += HandleListChanged;

        // 방장인 경우
        if (IsServer)
        {
            _players.Clear();
            _phase.Value = GamePhase.Room;

            NetworkManager.OnClientDisconnectCallback += HandleClientDisconnected;
        }

        // 클라이언트인 경우
        if (IsClient)
        {
            string name = LobbyManager.Instance.LocalPlayerName;
            RegisterPlayerRpc(ToFixedName(name));
        }

        OnPlayerChanged?.Invoke();
    }

    public override void OnNetworkDespawn()
    {
        _players.OnListChanged -= HandleListChanged;

        if (IsServer && NetworkManager != null)
        {
            NetworkManager.OnClientDisconnectCallback -= HandleClientDisconnected;
        }

        OnPlayerChanged?.Invoke();
    }

    // 조회
    public List<RoomPlayerState> GetPlayers()
    {
        var result = new List<RoomPlayerState>();
        foreach(var player in _players)
        {
            result.Add(player);
        }
        return result;
    }

    public bool TryGetPlayer(ulong clientId, out RoomPlayerState player)
    {
        int index = IndexOf(clientId);
        player = index >= 0 ? _players[index] : default;
        return index >= 0;
    }

    public bool IsJobTaken(ulong currentClientId, string jobId)
    {
        foreach(var player in _players)
        {
            if(player.ClientId != currentClientId && player.JobId == jobId)
            {
                return true;
            }
        }
        return false;
    }

    // 클라이언트 -> 서버
    public void RequestSelectJob(string jobId)
    {
        SelectJobRpc(jobId);
    }

    // 서버

    // RPC
    [Rpc(SendTo.Server)]
    private void RegisterPlayerRpc(FixedString64Bytes playerName, RpcParams rpcParams = default)
    {
        // 이미 플레이어 목록에 있으면 반환
        ulong sender = rpcParams.Receive.SenderClientId;
        if (IndexOf(sender) >= 0)
            return;

        if (_phase.Value != GamePhase.Room)
        {
            NetworkManager.DisconnectClient(sender, "지금은 입장할 수 없습니다.");
            return;
        }

        _players.Add(new RoomPlayerState
        {
            ClientId = sender,
            PlayerName = playerName,
            JobId = string.Empty
        });
    }

    [Rpc(SendTo.Server)]
    private void SelectJobRpc(string jobId, RpcParams rpcParams = default)
    {
        if (_phase.Value != GamePhase.Room)
            return;

        ulong sender = rpcParams.Receive.SenderClientId;
        int index = IndexOf(sender);
        if (index < 0)
            return;

        var player = _players[index];
        if (IsJobTaken(sender, jobId))
            return;

        player.JobId = jobId;
        _players[index] = player;
    }

    // 내부 처리
    private void HandleListChanged(NetworkListEvent<RoomPlayerState> changeEvent)
    {
        OnPlayerChanged?.Invoke();
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        int index = IndexOf(clientId);

        if(index != -1)
        {
            _players.RemoveAt(index);
        }
    }

    private int IndexOf(ulong clientId)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            var player = _players[i];
            if (player.ClientId == clientId)
            {
                return i;
            }
        }
        return -1;
    }

    private static FixedString64Bytes ToFixedName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) name = "Player";
        name = name.Trim();
        if (name.Length > 100) name = name.Substring(0, 100);
        return new FixedString64Bytes(name);
    }
}