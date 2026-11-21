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
    // MainUI
    LobbyView,
    RoomView,

    // PopupUI
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

    public static void OpenCreateRoomPopup(this UIManager uiManager, string name)
    {
        var uibase = UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.CreateRoomPopup);
        if(uibase is CreateRoomPopup createRoomPopup)
        {
            createRoomPopup.OpenCreateRoomPopup(name);
        }
    }
}