using System.Collections;
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

        [Header("Prefabs")]
        public GameObject PlayerPrefab;

        // Lobby Viewer
        public LobbyViewer LobbyViewer;

        // Sync attributes assigned by server

        // Lobby the player is currently in
        public LobbyObject CurrentLobby;

        public List<PlayerLobbyItem> PlayerItems;

        // What type of lobby are we in
        [SyncVar]
        public LobbyType LType;
    }
}