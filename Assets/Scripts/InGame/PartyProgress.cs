using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class PartyProgress : NetworkBehaviour
{
    private float _maxExp = 100f;
    private NetworkVariable<float> _partyExp = new(0);

    private readonly HashSet<ulong> _chosenClients = new();

    public override void OnNetworkSpawn()
    {
        GameManager.Instance.MatchState.Phase.OnValueChanged += HandlePhaseChaged;
    }

    public override void OnNetworkDespawn()
    {
        GameManager.Instance.MatchState.Phase.OnValueChanged -= HandlePhaseChaged;
    }

    private void HandlePhaseChaged(GamePhase previous, GamePhase current)
    {
        if(current == GamePhase.InGame)
        {
            ResetPartyProgress();
        }
    }

    private void ResetPartyProgress()
    {
        if (IsServer == false)
            return;

        _partyExp.Value = 0;
    }

    public void AddProgressExp(float exp)
    {
        if (IsServer == false)
            return;

        _partyExp.Value += exp;
        Debug.Log($"경험치 획득 : {exp}");
        if(_partyExp.Value >= _maxExp)
        {
            _partyExp.Value -= _maxExp;
            UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.LevelUpPopup);
            GameManager.Instance.MatchState.IsRoundPaused.Value = true;
        }
    }

    public void NotifyUpgradeChosen(ulong clientId)
    {
        if (IsServer == false)
            return;

        _chosenClients.Add(clientId);

        if(_chosenClients.Count >= NetworkManager.Singleton.ConnectedClientsList.Count)
        {
            _chosenClients.Clear();
            GameManager.Instance.MatchState.IsRoundPaused.Value = false;
        }
    }
}