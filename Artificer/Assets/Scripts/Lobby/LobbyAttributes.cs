﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Lobby
{
    public enum LobbyType { LAN, Steam };

    public class LobbyAttributes : NetworkBehaviour
    {
        [Header("HUD Elements")]
        public Transform PlayerList;

        // UI For Lobby Controller
        public Button SearchBtn;

        public Button InviteBtn;

        public Button LeaveBtn;

        public Button StartBtn;

        public Text PlayerCountText;

        public Space.UI.MessageHUD Message;

        [Header("Prefabs")]
        public GameObject PlayerPrefab;

        [Header("Custom Attributes")]
        public int MinPlayers;

        // Lobby Viewer
        public LobbyViewer LobbyViewer;

        public LobbyObject SteamLobby;

        public List<PlayerLobbyItem> PlayerItems;

        // Sync attributes assigned by server

        // What type of lobby are we in
        [SyncVar]
        public LobbyType LType;
    }
}