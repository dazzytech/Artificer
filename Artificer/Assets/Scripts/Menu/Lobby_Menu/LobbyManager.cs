using UnityEngine;
using System.Collections;
using Steamworks;
using Data.UI;

namespace Menu.Lobby
{
    /// <summary>
    /// Seperate minimal lobby item for creating 
    /// steam matchmaker lobby
    /// </summary>
    public class LobbyManager : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private LobbyAttributes m_att;

        #endregion

        #region ACCESSOR

        private CSteamID LobbyID
        {
            get { return m_att.CurrentLobby.GetID; }
        }

        #endregion

        #region EVENTS

        // Trigger this event when we switch from idle to searching etc.
        public delegate void ChangeState();
        public event ChangeState OnUpdateState;

        #endregion

        #region MONO BEHAVIOUR

        private void OnEnable()
        {
            OnUpdateState += UpdateState;

            if (SteamManager.Initialized)
            {
                CreateHiddenLobby();
            }
        }

        private void OnDisable()
        {
            if (m_att.CurrentLobby != null)
                if (SteamManager.Initialized)
                    QuitLobby();

            OnUpdateState -= UpdateState;
        }

        #endregion

        #region EVENT LISTENER INTERFACE

        public void StartLobbySearch() // filter will be passed through params (maybe create)
        {
            // SystemManager keeps a persistant string that stores 
            // the current version
            SteamMatchmaking.AddRequestLobbyListStringFilter
                ("ver", SystemManager.Version, 
                ELobbyComparison.k_ELobbyComparisonEqual);

            SteamMatchmaking.
            AddRequestLobbyListFilterSlotsAvailable
                (SteamMatchmaking.GetNumLobbyMembers(m_att.CurrentLobby.GetID));
            // Find enough space for everyone within the private lobby

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

            // create new lobby object within memory
            // FIX
            m_att.CurrentLobby = new LobbyObject(pLobby);

            m_att.CurrentLobby.OnDataUpdate += UpdateLobby;

            m_att.CurrentLobby.OnUserUpdate += UpdateLobby;

            m_att.CurrentLobby.OnChatUpdate += UpdateLobby;

            m_att.CurrentLobby.Initialize();

            OnUpdateState();
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

            Network.Connect("http://www.google.com");

            // Define IP address
            SteamMatchmaking.SetLobbyData(pLobby, "ip", Network.player.externalIP);

            Network.Disconnect();

            // Add more when games are customized
        }

        
        public void QuitLobby()
        {
            Debug.Log("Attempt to leave Lobby");

            // Check that we arent already in a hidden lobby
            if (SteamMatchmaking.GetLobbyData(m_att.CurrentLobby.GetID, "live") == "true")
            {
                // Leave the current lobby we are occupying
                LeaveLobby();

                Debug.Log("Left Lobby");

                // Create a new private lobby
                CreateHiddenLobby();
            }

            // else we have no lobby to leave
            if(OnUpdateState != null)
                OnUpdateState();
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
            if (SteamManager.Initialized)
            {
                LeaveLobby();

                SteamAPICall_t handle = SteamMatchmaking.JoinLobby(pLobby);
                m_att.LobbyEnterCallResult.Set(handle);
            }
        }

        /// <summary>
        /// Couldn't find a suitable lobby,
        /// make our private lobby public
        /// </summary>
        private void MakeLobbyPublic()
        {
            if(SteamMatchmaking.SetLobbyType(m_att.CurrentLobby.GetID, 
                ELobbyType.k_ELobbyTypePublic))
            {
                // Update the variable to show that we are now in a live lobby
                SteamMatchmaking.SetLobbyData(LobbyID, "live", "true");

                // Begin starting timer
                StartCoroutine("BeginTimer");
            }

            // need to consider if failed

            OnUpdateState();
        }

        /// <summary>
        /// Creates an friends only lobby for 
        /// invitations and initialing vars for filters 
        /// </summary>
        private void CreateHiddenLobby()
        {
            if (SteamManager.Initialized)
            {
                SteamAPICall_t handle = SteamMatchmaking.CreateLobby
                    (ELobbyType.k_ELobbyTypePrivate, 32);
                m_att.LobbyCreatedCallResult.Set(handle);
            }
        } 

        /// <summary>
        /// If player is currently in a lobby then leave 
        /// said lobby
        /// </summary>
        public void LeaveLobby()
        {
            if (m_att.CurrentLobby != null)
            {
                SteamMatchmaking.LeaveLobby(m_att.CurrentLobby.GetID);

                m_att.CurrentLobby.OnDataUpdate -= UpdateLobby;

                m_att.CurrentLobby.OnUserUpdate -= UpdateLobby;

                m_att.CurrentLobby.OnChatUpdate -= UpdateLobby;

                m_att.CurrentLobby.OnDataUpdate
                    -= m_att.LobbyViewer.ViewLobby;

                m_att.CurrentLobby = null;
            }
        }

        #endregion

        #region PRIVATE UTILITIES

        private void UpdateState()
        {
            // Change so this is only implimented when event is triggered
            if (m_att.CurrentLobby != null)
            {
                if (SteamMatchmaking.GetLobbyData(LobbyID, "live") == "true")
                {
                    // we are in a live game, need the leave option and invite option
                    m_att.SearchBtn.gameObject.SetActive(false);
                    m_att.InviteBtn.gameObject.SetActive(false);
                    m_att.LeaveBtn.gameObject.SetActive(true);
                }
                else
                {
                    //cant leave our private lobby
                    m_att.LeaveBtn.gameObject.SetActive(false);
                    m_att.InviteBtn.gameObject.SetActive(true);
                    // only lobby owners can start search
                    if (SteamMatchmaking.GetLobbyOwner(LobbyID).
                            Equals(SteamUser.GetSteamID()))
                        m_att.SearchBtn.gameObject.SetActive(true);
                    else
                        m_att.SearchBtn.gameObject.SetActive(false);
                }
            }
            else
            {
                // possible error
                m_att.SearchBtn.gameObject.SetActive(false);
                m_att.InviteBtn.gameObject.SetActive(false);
                m_att.LeaveBtn.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Performs any necessary actions when lobby
        /// has changed e.g. reseting start counter
        /// or starting match is game has started
        /// </summary>
        private void UpdateLobby()
        {
            // Update visually
            m_att.LobbyViewer.ViewLobby();

            // Check if the lobby game has started
            // for us to start 
            if (SteamMatchmaking.GetLobbyData(LobbyID, "running") == "true")
            {
                SystemManager.JoinOnlineClient
                    (SteamMatchmaking.GetLobbyData
                    (LobbyID, "ip"), LobbyID);
            }
        }

        /// <summary>
        /// Called on the host
        /// to create the online match
        /// </summary>
        private void BuildGame()
        {
            if (SteamMatchmaking.GetLobbyOwner(LobbyID)
                    != SteamUser.GetSteamID())
                // exit if not host
                return;

            // set game to running
            SteamMatchmaking.SetLobbyData
                (LobbyID, "running", "true");


            // Build Server with game manager
            ServerData newServer = new ServerData();

            // Populate connection info
            newServer.ServerIP = SteamMatchmaking
                .GetLobbyData(LobbyID, "ip");  

            newServer.ServerPort = 7777;

            newServer.ServerVersion = SteamMatchmaking
                .GetLobbyData(LobbyID, "version");

            newServer.ServerName = "Steam Game";

            SystemManager.CreateOnlineServer(newServer, LobbyID);
        }

        #endregion

        #region COROUTINE

        /// <summary>
        /// A countdown timer that will
        /// start a match with the minimum amount of
        /// players
        /// </summary>
        /// <returns></returns>
        private IEnumerator BeginTimer()
        {
            // Loop through each second
            int seconds = 0;

            while(seconds < m_att.LobbyTimer)
            {
                // Update text item
                m_att.CounterText.text =
                    string.Format("Seconds remaining: {0}",
                    m_att.LobbyTimer - seconds);

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
                m_att.CounterText.text = m_att.MinPlayers + " Players Required";
                // Dont have enough players to start
                QuitLobby();
            }
            else
                BuildGame();

            yield return null;
        }

        #endregion
    }
}