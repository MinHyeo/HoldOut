using UnityEngine;

public class GameManager : SingletonBase<GameManager> 
{
    [SerializeField] private MatchState _matchState;
    [SerializeField] private PartyProgress _partyProgress;

    public MatchState MatchState => _matchState;
    public PartyProgress PartyProgress => _partyProgress;

    private void OnEnable()
    {
        _matchState.Phase.OnValueChanged += HandlePhaseChanged;
    }

    private void OnDisable()
    {
        _matchState.Phase.OnValueChanged -= HandlePhaseChanged;
    }

    private void HandlePhaseChanged(GamePhase previous, GamePhase current)
    {
        if(current == GamePhase.InGame)
        {
            UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.RoomView);
            // UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.GameHUD);
        }
    }
}