using UnityEngine;
using System.Collections;
using Steamworks;
using Data.UI;
using UnityEngine.Networking.Types;
using System;

namespace Menu.Lobby
{
    /// <summary>
    /// Seperate minimal lobby item for creating 
    /// steam matchmaker lobby
    /// </summary>
    [RequireComponent(typeof(LobbyAttributes))]
    [RequireComponent(typeof(LobbyCallback))]
    public class LobbyManager : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private LobbyAttributes m_att;

        [SerializeField]
        private LobbyCallback m_cal;

        #endregion

        #region ACCESSOR

        private CSteamID LobbyID
        {
            get
            {
                if (m_att.CurrentLobby == null)
                    return CSteamID.Nil;

                return m_att.CurrentLobby.GetID;
            }
        }

        /// <summary>
        /// Retrieves if we are the host of the 
        /// current lobby
        /// also returns false if we are not in lobby
        /// </summary>
        private bool IsHost
        {
            get
            {
                if (LobbyID == CSteamID.Nil)
                    return false;

                return (SteamMatchmaking.GetLobbyOwner(LobbyID)
                    == SteamUser.GetSteamID());
            }
        }

        private bool IsLive
        {
            get
            {
                if (LobbyID == CSteamID.Nil)
                    return false;

                return (SteamMatchmaking.GetLobbyData
                    (LobbyID, "live") == "true");
            }
        }

        private bool IsRunning
        {
            get
            {
                if (LobbyID == CSteamID.Nil)
                    return false;

                return (SteamMatchmaking.GetLobbyData
                    (LobbyID, "running") == "true");
            }
        }

        #endregion

        #region MONO BEHAVIOUR

        /// <summary>
        /// Create lobby if we arent 
        /// in a lobby then build one
        /// </summary>
        private void OnEnable()
        {
            // Init the callbacks before 
            // using them
            m_cal.Intitialize();

            if (SteamManager.Initialized &&
                LobbyID == CSteamID.Nil)
            {
                CreateHiddenLobby();
            }
        }

        private void OnDisable()
        {
            if (IsLive && !IsRunning)
                QuitLobby();
            else
                LeaveLobby();
        }

        private void Awake()
        {
            m_att.Timeout = false;
            m_att.Starting = false;

            // Do not display buttons first
            m_att.SearchBtn.gameObject.SetActive(false);
            m_att.InviteBtn.gameObject.SetActive(false);
            m_att.LeaveBtn.gameObject.SetActive(false);

            m_att.CounterText.text = "Initializing Matchmaker";
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Retrieve a list of current steam
        /// lobbies with the same version
        /// and attempt to join the most suitable one
        /// </summary>
        public void StartLobbySearch()
        {
            // SystemManager keeps a persistant string that stores 
            // the current version
            SteamMatchmaking.AddRequestLobbyListStringFilter
                ("ver", SystemManager.Version, 
                ELobbyComparison.k_ELobbyComparisonEqual);

            // Find enough space for everyone within the private lobby
            SteamMatchmaking.
            AddRequestLobbyListFilterSlotsAvailable
                (SteamMatchmaking.GetNumLobbyMembers(m_att.CurrentLobby.GetID));

            // Apply request to callback
            SteamAPICall_t handle = SteamMatchmaking.RequestLobbyList();
            m_att.OnLobbyMatchListCallResult.Set(handle);
        }

        /// <summary>
        /// When lobby list has been created
        /// attempt to join the first lobby
        /// or make invisible lobby public if none available
        /// </summary>
        public void JoinSuitableLobby()
        { 
            // check that we have lobbies
            if (m_att.LobbyList == null)
            {
                // no lobbies, create one
                MakeLobbyPublic();
                return;
            }
            else if(m_att.LobbyList.Length == 0)
            {
                // no lobbies, create one
                MakeLobbyPublic();
                return;
            }

            Debug.Log("attemping to join lobby");

            // As filters are already applied join the first available lobby
            CSteamID newLobby = m_att.LobbyList[0];

            if (newLobby != CSteamID.Nil)
                JoinLobby(newLobby);
            else
                Debug.Log("Something went wrong..");
        }

        /// <summary>
        /// Called when player joins a lobby
        /// </summary>
        public void JoinAttemptSuccess(CSteamID pLobby)
        {
            // if we are not already in this lobby then leave the lobby
            // before joining new
            if (!SteamFriends.IsUserInSource(SteamUser.GetSteamID(), pLobby))
            {
                LeaveLobby();
            }

            BuildLobby(pLobby);
        }

        /// <summary>
        /// Called when player rejected by lobby
        /// take lobby out of list and attempt to join next
        /// make invisible lobby public if none available
        /// </summary>
        /// <param name="pLobby"></param>
        public void JoinAttemptFailed(CSteamID pLobby)
        {
            // remove pLobby from lobbylist
            Debug.Log("failed");

            // create new list with one less lobby
            CSteamID[] newLobbyList = new CSteamID[m_att.LobbyList.Length];

            // Add lobbys to list that arent the failed lobby
            int index = 0;
            foreach (CSteamID lobby in m_att.LobbyList)
                if (lobby != pLobby)
                    newLobbyList[index++] = lobby;

            // assign new list to old
            m_att.LobbyList = newLobbyList;

            // recall join lobby func
            //JoinSuitableLobby();
        }

        /// <summary>
        /// if lobby has failed to create 
        /// then we need to inform the player
        /// </summary>
        public void OnLobbyCreationFailed()
        {
            Debug.Log("Lobby failed to create");
        }

        /// <summary>
        /// if lobby has been created then this
        /// function is responsible for assigning initial/default values
        /// </summary>
        public void OnLobbyCreationSuccess(CSteamID pLobby)
        {
            // When a lobby is created it is originally invisible for inviting friends
            // Create data to inform if the lobby is live ot not
            SteamMatchmaking.SetLobbyData(pLobby, "live", "false");

            // Assign the game version to the lobby
            SteamMatchmaking.SetLobbyData(pLobby, "ver", SystemManager.Version);

            // Set game not running
            SteamMatchmaking.SetLobbyData(pLobby, "running", "false");

            // Add more when games are customized
        }

        /// <summary>
        /// Removes the lobby from the viewer
        /// and then actually exits the lobby entering 
        /// another empty lobby
        /// </summary>
        public void QuitLobby()
        {
            // Check that we arent already in a hidden lobby
            if (IsLive)
            {
                // Leave the current lobby we are occupying
                LeaveLobby();

                // Create a new private lobby
                CreateHiddenLobby();
            }
        }

        /// <summary>
        /// Opens a steam overlay and that takes over
        /// </summary>
        public void InviteFriends()
        {
            // Make sure we have enough spaces

            // Open a steam window with a lobby invite
            SteamFriends.ActivateGameOverlay("LobbyInvite");
        }

        #endregion

        #region CALLBACK INTERACTION

        /// <summary>
        /// Creates a callback to join a lobby through steam
        /// </summary>
        /// <param name="pLobby"></param>
        private void JoinLobby(CSteamID pLobby)
        {
            Debug.Log("Joined Lobby");
            if (SteamManager.Initialized)
            {
                LeaveLobby();

                SteamAPICall_t handle = SteamMatchmaking.JoinLobby(pLobby);
                m_att.LobbyEnterCallResult.Set(handle);
            }
        }

        /// <summary>
        /// Sets lobby state to live for others 
        /// to join
        /// </summary>
        private void MakeLobbyPublic()
        {
            if(SteamMatchmaking.SetLobbyType(m_att.CurrentLobby.GetID, 
                ELobbyType.k_ELobbyTypePublic))
                // Update the variable to show that we are now in a live lobby
                SteamMatchmaking.SetLobbyData(LobbyID, "live", "true");

            // need to consider if failed
        }

        /// <summary>
        /// Creates an friends only lobby for 
        /// invitations and initialing vars for filters 
        /// </summary>
        private void CreateHiddenLobby()
        {
            Debug.Log("Creating Hidden Lobby");

            if (SteamManager.Initialized)
            {
                SteamAPICall_t handle = SteamMatchmaking.CreateLobby
                    (ELobbyType.k_ELobbyTypePrivate, 32);
                m_att.LobbyCreatedCallResult.Set(handle);
            }
        }

        #endregion

        #region PRIVATE UTILITIES

        #region UPDATES

        /// <summary>
        /// Called when any data relating to the Lobby
        /// is changed e.g. game rules, game state
        /// </summary>
        private void UpdateLobby(object param)
        {
            // Update visually
            m_att.LobbyViewer.UpdateSettings();

            UpdateLobbyState();

            if (IsHost && IsLive && !m_att.Starting)
            {
                StartCoroutine("StartDelay");
            }

            // Check if the lobby game has started
            // for us to start 
            if (IsRunning)
            {
                JoinGame();
            }

            Debug.Log(String.Format("Lobby Update: IsHost - {0}, Is Live - {1}, Running - {2} "
                , IsHost, IsLive,  IsRunning ));
        }

        /// <summary>
        /// Update lobbyviewer and delay timeout 
        /// if player joins
        /// </summary>
        /// <param name="param"></param>
        private void UpdatePlayers(object param)
        {
            // Update visually
            m_att.LobbyViewer.UpdatePlayers();

            // Only perform if the timeout function is invoked
            if (m_att.Timeout)
            {
                // See if we are called due to player 
                // Joining the lobby
                if((int)param == 1)
                {
                    // Increase timeout by 5 sec
                    // if player joined
                    m_att.TimeoutTimer += 5;
                }
            }
        }

        private void UpdatePersona(object param)
        {
            // Update visually
            m_att.LobbyViewer.UpdatePlayers();
        }

        #endregion

        #region GAME CONTROLS

        /// <summary>
        /// Called on the host
        /// to create the online match
        /// </summary>
        private void BuildGame()
        {
            if (!IsHost)
                // exit if not host
                return;

            // Build Server with game manager
            ServerData newServer = new ServerData();

            newServer.ServerPort = 7777;

            newServer.ServerVersion = SteamMatchmaking
                .GetLobbyData(LobbyID, "version");

            newServer.ServerName = "Steam Game";

            SystemManager.CreateOnlineServer(newServer, LobbyID);
        }

        /// <summary>
        /// Joins the match specified 
        /// in the LobbyData
        /// </summary>
        private void JoinGame()
        {
            Debug.Log("Attempting to join Game");

            SystemManager.JoinOnlineClient
                (SteamMatchmaking.GetLobbyData(LobbyID, "publicIP"),
                SteamMatchmaking.GetLobbyData(LobbyID, "internalIP"),
                Convert.ToUInt64(SteamMatchmaking.GetLobbyData(LobbyID, "guid")),
                LobbyID);
        }

        #endregion

        #region LOBBY CONTROLS

        /// <summary>
        /// Build our lobby within 
        /// memory when joined
        /// </summary>
        /// <param name="pLobby"></param>
        private void BuildLobby(CSteamID pLobby)
        {
            Debug.Log("Building Lobby");

            // create new lobby object within memory
            m_att.CurrentLobby = new LobbyObject(pLobby);

            // Listen for changes
            m_att.CurrentLobby.OnDataUpdate += UpdateLobby;
            m_att.CurrentLobby.OnUserUpdate += UpdatePersona;
            m_att.CurrentLobby.OnChatUpdate += UpdatePlayers;

            // Initialize Lobby and view the players and
            // setting before updates
            m_att.CurrentLobby.Initialize();
            m_att.LobbyViewer.UpdateSettings();
            m_att.LobbyViewer.UpdatePlayers();
        }

        /// <summary>
        /// Removes any reference to the Lobby in the viewer
        /// does not technically quit the actual lobby in steam
        /// </summary>
        private void LeaveLobby()
        {
            Debug.Log("Leaving Lobby - Running: " + IsRunning.ToString());

            // Stop any timers that may be happening
            StopAllCoroutines();

            m_att.Starting = false;
            m_att.Timeout = false;

            m_att.CounterText.text = "Left lobby";

            if (m_att.CurrentLobby != null)
            {
                if(!IsRunning)
                    SteamMatchmaking.LeaveLobby(m_att.CurrentLobby.GetID);
                m_att.CurrentLobby.OnDataUpdate -= UpdateLobby;
                m_att.CurrentLobby.OnUserUpdate -= UpdatePersona;
                m_att.CurrentLobby.OnChatUpdate -= UpdatePlayers;
                m_att.CurrentLobby = null;
            }
        }

        /// <summary>
        /// Change the viewer based on lobby state
        /// </summary>
        private void UpdateLobbyState()
        {
            m_att.CounterText.text = "Connected";

            if (IsLive)
            {
                // we are in a live game, need the leave option and invite option
                m_att.SearchBtn.gameObject.SetActive(false);
                m_att.InviteBtn.gameObject.SetActive(false);
                m_att.LeaveBtn.gameObject.SetActive(true);

                return;
            }

            //cant leave our private lobby
            m_att.LeaveBtn.gameObject.SetActive(false);
            m_att.InviteBtn.gameObject.SetActive(true);
            // only lobby owners can start search
            if (IsHost)
                m_att.SearchBtn.gameObject.SetActive(true);
            else
                m_att.SearchBtn.gameObject.SetActive(false);
        }

        #endregion

        #endregion

        #region COROUTINE

        /// <summary>
        /// A countdown timer that will
        /// start a match with the minimum amount of
        /// players
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartDelay()
        {
            // Loop through each second
            int seconds = 0;

            m_att.Starting = true;

            while(seconds < m_att.StartTimer)
            {
                float remaining = m_att.StartTimer - seconds;

                // Update text item
                // Display remaining time in format
                // 0:00 minutes and seconds
                m_att.CounterText.text =
                    string.Format("Waiting for players: {0:D1}:{1:D2}",
                    Mathf.FloorToInt(remaining / 60),
                    Mathf.CeilToInt(remaining % 60));

                // increment seconds
                seconds++;

                // check if we reached full pop
                if (SteamMatchmaking.GetNumLobbyMembers(LobbyID) >=
                    SteamMatchmaking.GetLobbyMemberLimit(LobbyID))
                // reached full population
                {
                    BuildGame();
                    yield break;
                }

                // Wait for a second
                yield return new WaitForSeconds
                    (1f);
            }

            if (SteamMatchmaking.GetNumLobbyMembers(LobbyID)
                < m_att.MinPlayers)
            {
                // Begin the timeout if we have not 
                // got enough players
                yield return TimeoutDelay();
            }
            else
                BuildGame();

            m_att.Starting = false;

            yield break;
        }

        /// <summary>
        /// Called after alloted min start
        /// time elapsed. causes timeout
        /// and leaves lobby after alloted time
        /// </summary>
        /// <returns></returns>
        private IEnumerator TimeoutDelay()
        {
            m_att.Timeout = true;

            // Loop through each second
            int seconds = 0;

            while (seconds < m_att.TimeoutTimer)
            {
                float remaining = m_att.TimeoutTimer - seconds;

                // Display remaining time in format
                // 0:00 minutes and seconds
                m_att.CounterText.text =
                    string.Format("Timeout in: {0:D1}:{1:D2}",
                    Mathf.FloorToInt(remaining / 60),
                    Mathf.CeilToInt(remaining % 60));

                // increment seconds
                seconds++;

                // if we reach the minimum amount of players then 
                // begin the match
                if (SteamMatchmaking.GetNumLobbyMembers(LobbyID) >=
                    m_att.MinPlayers)
                {
                    // Attained enough players 
                    // to begin game
                    BuildGame();
                    yield break;
                }

                // Wait for a second
                yield return new WaitForSeconds
                    (1f);
            }

            // Dont have enough players to start
            m_att.CounterText.text = "Lobby timed out - not enough players.";
            QuitLobby();

            m_att.Timeout = false;

            Popup_Dialog.ShowPopup("Not Enough Players",
                "Not enough players have been found, please try again.",
                DialogType.IMAGE, true);

            yield break;
        }

        #endregion
    }
}