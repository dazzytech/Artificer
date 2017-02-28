using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Steamworks;

namespace Menu.Matchmaker
{
    //public enum OnlineState{Idle, Searching};
    public class Matchmaking_Attributes : MonoBehaviour
    {
        /*public GameObject LobbyListPanel;
        public GameObject FilterPanel;
        public GameObject LobbyPanel;
        public GameObject PopupPanel;

        public OnlineState OnlineState;

        // panel attributes
        public FilterInfo fInfo;
        public CSteamID sID;
        public string dHeader;
        public string dMsg;*/

        // Lobby Viewer
        //public Lobby_Viewer LobbyViewer;

        // UI For Lobby Controller
        public Button SearchBtn;

        public Button InviteBtn;

        public Button LeaveBtn;

        // Steam references
        // Lobby the player is currently in
        //public Lobby_Object CurrentLobby;

        // List of lobbies for matckmaking
        public CSteamID[] LobbyList;

        // Call Results
        public CallResult<LobbyMatchList_t> OnLobbyMatchListCallResult;
        public CallResult<LobbyEnter_t> LobbyEnterCallResult;
        public CallResult<LobbyCreated_t> LobbyCreatedCallResult;
    }
}

