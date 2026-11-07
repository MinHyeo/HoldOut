using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    [SerializeField] private MatchState _matchState;


    private readonly List<NetworkObject> _spawned = new List<NetworkObject>();

    private void OnEnable()
    {
        _matchState.Phase.OnValueChanged += HandlePhaseChaged;
    }

    private void OnDisable()
    {
        _matchState.Phase.OnValueChanged -= HandlePhaseChaged;
    }

    private void HandlePhaseChaged(GamePhase previous, GamePhase current)
    {
        if (NetworkManager.Singleton.IsServer == false)
            return;

        if(current == GamePhase.InGame)
        {
            SpawnAllCharacter();
            return;
        }

        if(previous == GamePhase.InGame)
        {
            DespawnAllCharacter();
            return;
        }
    }

    private void SpawnAllCharacter()
    {
        DespawnAllCharacter();

        List<RoomPlayerState> players = _matchState.GetPlayers();
        for(int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            var instance = GameObjectManager.Instance.CreateObject("Character", Vector3.zero).GetComponent<NetworkObject>();

            var character = instance.GetComponent<Character>();
            if (character == null)
                return;

            character.JobId.Value = player.JobId;

            instance.SpawnAsPlayerObject(player.ClientId, destroyWithScene: true);
            _spawned.Add(instance);
        }
    }

    private void DespawnAllCharacter()
    {

    }
}
