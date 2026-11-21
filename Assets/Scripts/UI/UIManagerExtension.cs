using System;
using UnityEngine;

public enum UIRootType 
{
    None,
    BackgroundUI,
    MainUI,
    ContentUI,
    PopupUI,
    VeryFrontUI,
}

public enum UIType
{
    LobbyView,
    RoomView,
    CreateRoomPopup,
    JobSelectPopup,
    JoinRoomPopup,
}

public static class UIManagerExtension
{
    public static string GetUIPath(this UIManager uiManager, UIRootType uiRootType, UIType uiType)
    {
        string path = string.Empty;

        path = $"UI/{uiRootType}/{uiType}";
        return path;
    }

    public static void OpenJoinRoomPopup(this UIManager uiManager, string name)
    {
        var uibase = UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.JoinRoomPopup);
        if(uibase is JoinRoomPopup joinRoomPopup)
        {
            joinRoomPopup.OpenJoinRoomPopup(name);
        }
    }
}