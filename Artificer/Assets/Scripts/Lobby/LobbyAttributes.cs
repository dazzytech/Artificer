using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public enum LobbyType { LAN, Steam };

    public class LobbyAttributes : MonoBehaviour
    {
        // Lobby Viewer
        public LobbyViewer LobbyViewer;

        // UI For Lobby Controller
        public Button SearchBtn;

        public Button InviteBtn;

        public Button LeaveBtn;

        // Steam references
        // Lobby the player is currently in
        public LobbyObject CurrentLobby;

        // What type of lobby are we in
        public LobbyType LType;
    }
}