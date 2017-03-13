using UnityEngine;
using System.Collections;
using Steamworks;

namespace Menu.Lobby
{
    /// <summary>
    /// Matchmaker_ callback.
    /// central source of functions used for steam callbacks and call results.
    /// assigns functions to callbacks within attributes.
    /// Dispatches events once updating attibutes
    /// </summary>
    public class LobbyCallback : MonoBehaviour
    {
        #region ATTRIBUTES

        // Contains a reference to attributes
        [SerializeField]
        private LobbyAttributes m_att;

        #endregion

        #region EVENTS

        // delegate functions all events are based on
        public delegate void CALLBACK_EVENT
            (CSteamID pLobby = new CSteamID());

        // called when the lobbylist has been populated
        public event CALLBACK_EVENT OnLobbyListCompleted;

        // called when we have succesfully connected to a lobby
        public event CALLBACK_EVENT OnLobbyConnectSuccess;

        // called when we have failed to connect to lobby
        public event CALLBACK_EVENT OnLobbyConnectFailed;

        // called when creation of lobby failed
        public event CALLBACK_EVENT OnLobbyCreationFailed;

        // called when a lobby is created by us
        public event CALLBACK_EVENT OnLobbyCreationSuccess;

        #endregion

        #region MONO BEHAVIOUR

        void Awake()
        {
        }

        void OnEnable()
        {
            // Assign functions to callback attributes

            //
            // CALL RESULT ASSIGNMENT
            //
            m_att.OnLobbyMatchListCallResult = CallResult<LobbyMatchList_t>.
                Create(OnLobbyMatchList);

            m_att.LobbyEnterCallResult = CallResult<LobbyEnter_t>
                .Create(OnLobbyEnter);

            m_att.LobbyCreatedCallResult = CallResult<LobbyCreated_t>
                .Create(OnLobbyCreate);
        }

        #endregion

        #region CALLBACKS

        /// <summary>
        /// Raises the lobby match list event.
        /// Called when steam returns a lobby list
        /// adds to att.lobbylist and raises the list created event
        /// </summary>
        /// <param name="pCallback">P callback.</param>
        /// <param name="bIOFailure">If set to <c>true</c> b IO failure.</param>
        private void OnLobbyMatchList(LobbyMatchList_t pCallback, bool bIOFailure)
        {
            if(bIOFailure)
            {
                Debug.Log("Error retriving lobby list");
                OnLobbyListCompleted();
                return;
            }
            
            if(pCallback.m_nLobbiesMatching == 0)
            {
                Debug.Log("No lobbies found");
                OnLobbyListCompleted();
                return;
            }

            // Initialize lobbylist
            m_att.LobbyList = new CSteamID[pCallback.m_nLobbiesMatching];
            
            // iterate through list of lobbies and retrive their info
            for(int i = 0; i < pCallback.m_nLobbiesMatching;i++)
            {
                // retrive SteamID of lobby
                CSteamID pLobby = SteamMatchmaking.GetLobbyByIndex(i);

                // store lobby in lobby list within attributes
                m_att.LobbyList[i] = pLobby;
            }

            // raise an event to show that we have finished the list
            OnLobbyListCompleted();
        }

        /// <summary>
        /// Raises the lobby enter event.
        /// When the player has entered the lobby
        /// raise an event whether successful or not
        /// </summary>
        /// <param name="pCallback">P callback.</param>
        /// <param name="bIOFailure">If set to <c>true</c> b IO failure.</param>
        private void OnLobbyEnter(LobbyEnter_t pCallback, bool bIOFailure)
        {
            if (pCallback.m_EChatRoomEnterResponse ==
                (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
                OnLobbyConnectSuccess((CSteamID)
                    pCallback.m_ulSteamIDLobby);
            else
                OnLobbyConnectFailed();
        }

        private void OnLobbyCreate(LobbyCreated_t pCallback, bool bIOFailure)
        {
            if(pCallback.m_eResult == EResult.k_EResultOK)
            {
                // Allow us to intialize the lobby
                OnLobbyCreationSuccess((CSteamID)
                    pCallback.m_ulSteamIDLobby);

                // and then update the player data
                OnLobbyConnectSuccess((CSteamID)
                    pCallback.m_ulSteamIDLobby);
            }
            else
            {
                OnLobbyCreationFailed();
            }
        }

        #endregion
    }
}

        //NEED CONSIDERATION

    
/*

 /// <summary>
        /// Raises the lobby create event.
        /// Check status of lobby creation and assign 
        /// default values and update m_att.Lobby
        /// </summary>
        /// <param name="pCallback">P callback.</param>
        /// <param name="bIOFailure">If set to <c>true</c> b IO failure.</param>
        private void OnLobbyCreate
            (LobbyCreated_t pCallback, bool bIOFailure)
        {
            if(!bIOFailure && pCallback.m_eResult == EResult.k_EResultOK)
            {
                m_att.Lobby = (CSteamID)pCallback.m_ulSteamIDLobby;
                
                // set default values
                SteamMatchmaking.SetLobbyData
                    (m_att.Lobby, "ver", SystemManager.Version); 
                SteamMatchmaking.SetLobbyData
                    (m_att.Lobby, "name", "untitled lobby"); 
                SteamMatchmaking.SetLobbyData
                    (m_att.Lobby, "desc", ""); 
                SteamMatchmaking.SetLobbyData
                    (m_att.Lobby, "type", "default"); 

                // send to viewer?
                //m_viewer.ViewLobby(m_Lobby);
            }
            else
            {
                Debug.Log("Server Creation Failed, unable to build server!");
                return;
            }
        }
*/
