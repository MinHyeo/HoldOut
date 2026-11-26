using UnityEngine;

public class ResultView : UIBase
{
    [SerializeField] private UIButton _restartButton;
    [SerializeField] private UIButton _backToLobbyButton;

    private void OnEnable()
    {
        _restartButton.BindOnClickButtonEvent(OnClickRestartButton);
        _backToLobbyButton.BindOnClickButtonEvent(OnClickBackToLobbyButton);
    }

    private void OnDisable()
    {
        _restartButton.UnBindOnClickButtonEvent(OnClickRestartButton);
        _backToLobbyButton.UnBindOnClickButtonEvent(OnClickBackToLobbyButton);
    }

    private void OnClickRestartButton()
    {
        GameManager.Instance.MatchState.RequestStartGame();
    }

    private void OnClickBackToLobbyButton()
    {

    }
}
