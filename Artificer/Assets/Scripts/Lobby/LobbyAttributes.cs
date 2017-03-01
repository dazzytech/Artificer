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
        // Lobby Viewer
        public LobbyViewer LobbyViewer;

        // UI For Lobby Controller
        public Button SearchBtn;

        public Button InviteBtn;

        public Button LeaveBtn;

        // Sync attributes assigned by server

        // Lobby the player is currently in
        public LobbyObject CurrentLobby;

        // What type of lobby are we in
        [SyncVar]
        public LobbyType LType;
    }
}