using UnityEngine;

public class GameManager : SingletonBase<GameManager> 
{
    [SerializeField] private MatchState _matchState;
    public MatchState MatchState => _matchState;

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