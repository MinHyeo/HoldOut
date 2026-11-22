using UnityEngine;
using Unity.Netcode;

public class PartyProgress : NetworkBehaviour
{
    private float _maxExp = 100f;
    private NetworkVariable<float> _partyExp = new(0);

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
}