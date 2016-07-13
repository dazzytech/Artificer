using UnityEngine;
using System.Collections;
using Steamworks;

namespace Menu.Matchmaker
{
    /// <summary>
    /// Matchmaking_ event listener.
    /// Listens to events raised my other components or UI elements
    /// acts as an interface to matchmaking behaviour
    /// </summary>
    [RequireComponent(typeof(Matchmaking_Behaviour))]
    public class Matchmaking_EventListener : MonoBehaviour
    {
        #region VARIABLE DECLARATION

        // event listener causes the behaviour to preform
        // certain tasks
        private Matchmaking_Behaviour m_beh;

        // event listener listens to the callback events
        private Matchmaking_Callback m_cal;

        #endregion

        #region MONO BEHAVIOUR

        void Awake()
        {
            m_beh = GetComponent<Matchmaking_Behaviour>();
            m_cal = GetComponent<Matchmaking_Callback>();
        }

        void OnEnable()
        {
            // Assign listeners to callback events
            m_cal.OnLobbyListCompleted += OnLobbyListCompleted;
            m_cal.OnLobbyConnectSuccess += OnJoinSuccess;
            m_cal.OnLobbyConnectFailed += OnJoinFailed;
            m_cal.OnLobbyCreationSuccess += OnCreateSuccess;
        }

        void OnDisable()
        {
            // Release listeners from callback events
            m_cal.OnLobbyListCompleted -= OnLobbyListCompleted;
            m_cal.OnLobbyConnectSuccess -= OnJoinSuccess;
            m_cal.OnLobbyConnectFailed -= OnJoinFailed;
            m_cal.OnLobbyCreationSuccess -= OnCreateSuccess;
        }

        // For now just start matchmaker on Start
        void Start()
        {
            //BeginLobbySearch();
        }

        #endregion

        #region CALLBACK EVENTS

        /// <summary>
        /// Listens for when attributes list is complete
        /// and calls behaviour to start next step
        /// </summary>
        private void OnLobbyListCompleted(CSteamID isNil)
        {
            m_beh.JoinSuitableLobby();
        }

        /// <summary>
        /// listens for successful connection to lobby
        /// </summary>
        /// <param name="pLobby"></param>
        private void OnJoinSuccess(CSteamID pLobby)
        {
            m_beh.JoinAttemptSuccess(pLobby);
        }

        /// <summary>
        /// listens for lobby connection failure
        /// </summary>
        /// <param name="pLobby"></param>
        private void OnJoinFailed(CSteamID pLobby)
        {
            m_beh.JoinAttemptFailed(pLobby);
        }

        /// <summary>
        /// listens for if a creation attempt has failed
        /// error will need to be displayed
        /// </summary>
        /// <param name="isNil"></param>
        private void OnCreateFailed(CSteamID isNil)
        {
            m_beh.OnLobbyCreationFailed();
        }

        /// <summary>
        /// If lobby was successfully create inform controller
        /// </summary>
        /// <param name="pLobby"></param>
        private void OnCreateSuccess(CSteamID pLobby)
        {
            m_beh.OnLobbyCreationSuccess(pLobby);
        }

        #endregion

        #region UI INTERFACE EVENTS

        /// <summary>
        /// starts searching through lobbies with game settings
        /// </summary>
        public void BeginLobbySearch()
        {
            // Use UI elements to build up 
            // game settings for filter

            // invoke behaviour function
            m_beh.StartLobbySearch();
        }

        /// <summary>
        /// When the player leaves the lobby
        /// inform the controller to create a 
        /// private and 
        /// </summary>
        public void LeaveLobby()
        {
            m_beh.QuitLobby();
        }

        /// <summary>
        /// prompts the controller to activate the steam 
        /// friend invite overlay
        /// </summary>
        public void InviteFriends()
        {
            m_beh.InviteFriends();
        }

        #endregion

        /*
        /// <summary>
        /// Raises the enable event.
        /// </summary>
        void OnEnable()
        {
            m_controller.OnStateChanged += OnStateChanged;

            m_beh.CurrentState = OnlineState.Idle;
        }
        
        /// <summary>
        /// Raises the disable event.
        /// </summary>
        void OnDisable()
        {
            m_controller.OnStateChanged -= OnStateChanged;
        }
        
        /// <summary>
        /// Applys the newly changed menu state
        /// </summary>
        void OnStateChanged(OnlineState newState)
        {
            CloseAllTabs();
            
            switch (newState)
            {
                case OnlineState.LobbyList:
                    m_attributes.LobbyListPanel.SetActive(true);
                    if(m_attributes.fInfo != null)
                    {
                        m_attributes.LobbyListPanel.SendMessage("SetFilterData", m_attributes.fInfo);
                        m_attributes.fInfo = null;
                    }
                    break;
                case OnlineState.Lobby:
                    m_attributes.LobbyPanel.SetActive(true);
                    if(m_attributes.sID != CSteamID.Nil)
                        m_attributes.LobbyPanel.SendMessage("AssignLobby", m_attributes.sID);
                    else
                        m_attributes.LobbyPanel.SendMessage("CreateLobby");
                    break;
                case OnlineState.Filter:
                    m_attributes.FilterPanel.SetActive(true);
                    break;
                case OnlineState.Popup:
                    m_attributes.PopupPanel.SetActive(true);
                    // Add string messages to popup
                    m_attributes.PopupPanel.SendMessage("AssignMsg", 
                        new string[2]{m_attributes.dHeader, m_attributes.dMsg});
                    break;
            }
            
            m_attributes.OnlineState = newState;
        }
        
        /// <summary>
        /// Closes all tabs.
        /// in the menu
        /// </summary>
        private void CloseAllTabs()
        {
            if (m_attributes.LobbyListPanel.activeSelf)
            {
                m_attributes.LobbyListPanel.SetActive(false);
            }
            if (m_attributes.LobbyPanel.activeSelf)
            {
                m_attributes.LobbyPanel.SendMessage("LeaveLobby");
                m_attributes.LobbyPanel.SetActive(false);
            }
            if (m_attributes.FilterPanel.activeSelf)
            {
                m_attributes.FilterPanel.SetActive(false);
            }
            if (m_attributes.PopupPanel.activeSelf)
            {
                m_attributes.PopupPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Opens the filter window.
        /// button press
        /// </summary>
        public void OpenFilterWindow()
        {
            m_controller.CurrentState = OnlineState.Filter;
        }

        public void OpenLobbyListWindow()
        {
            m_controller.CurrentState = OnlineState.LobbyList;
        }

        public void OpenLobbyListWindowFiltered(FilterInfo filter = null)
        {
            m_controller.CurrentState = OnlineState.LobbyList;
            m_attributes.fInfo = filter;
        }

        public void OpenLobbyWindow(CSteamID pLobby)
        {
            m_controller.CurrentState = OnlineState.Lobby;
            m_attributes.sID = pLobby;   
        }

        public void OpenPopupWindow(string[] args)
        {
            m_controller.CurrentState = OnlineState.Popup;
            m_attributes.dHeader = args[0];
            m_attributes.dMsg = args[1];
        }*/
    }
}

