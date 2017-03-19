using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Steamworks;

namespace Menu.Lobby
{
    //public enum LobbyState{Idle, Searching, Joined};

    /// <summary>
    /// Attributes for Lobby manager
    /// </summary>
    public class LobbyAttributes : MonoBehaviour
    {
        #region HUD ELEMENTS

        [Header("HUD Elements")]

        // UI For Lobby Controller
        public Button SearchBtn;

        public Button InviteBtn;

        public Button LeaveBtn;

        public Text CounterText;

        #endregion

        #region LOBBY CONFIG

        public int MinPlayers;

        // How long should we be matchmaking
        public float LobbyTimer;

        #endregion

        #region LOBBY TRACKING

        // Lobby Viewer
        public LobbyViewer LobbyViewer;

        // Steam references
        // Lobby the player is currently in
        public LobbyObject CurrentLobby;

        // List of lobbies for matckmaking
        public CSteamID[] LobbyList;

        #endregion

        #region CALL RESULTS

        // Call Results
        public CallResult<LobbyMatchList_t> OnLobbyMatchListCallResult;
        public CallResult<LobbyEnter_t> LobbyEnterCallResult;
        public CallResult<LobbyCreated_t> LobbyCreatedCallResult;

        #endregion
    }
}

