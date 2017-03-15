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

        public GameObject LobbyPanel;

        // UI For Lobby Controller
        public Button SearchBtn;

        public Button InviteBtn;

        public Button LeaveBtn;

        #endregion

        #region LOBBY CONFIG

        // If we are searching or not
        //public LobbyState LobbyState;

        // panel attributes
        //public CSteamID SteamID;
        //public string dHeader;
        //public string dMsg;

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

