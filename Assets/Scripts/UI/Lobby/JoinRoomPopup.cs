using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JoinRoomPopup : UIBase
{
    [SerializeField] private TMP_InputField _joinCodeInputField;
    [SerializeField] private UIButton _enterButton;
    [SerializeField] private UIButton _exitButton;

    [SerializeField] private ErrorMessageText _errorMessagText;

    private void OnEnable()
    {
        _enterButton.BindOnClickButtonEvent(OnClickEnterButton);
        _exitButton.BindOnClickButtonEvent(OnClickExitButton);
    }

    private void OnDisable()
    {
        _enterButton.UnBindOnClickButtonEvent(OnClickEnterButton);
        _exitButton.UnBindOnClickButtonEvent(OnClickExitButton);
    }

    private async void OnClickEnterButton()
    {
        string joinCode = _joinCodeInputField.text;

        if(await LobbyManager.Instance.JoinRoomByCode(joinCode, "dd"))
        {
            UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.LobbyView);
            UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.JoinRoomPopup);
            UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.RoomView);
        }
        else
        {
            Debug.LogWarning("방 입장 실패");
            _errorMessagText.Show("Not Find Room");
        }
    }

    private void OnClickExitButton()
    {
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.JoinRoomPopup);
    }
}
