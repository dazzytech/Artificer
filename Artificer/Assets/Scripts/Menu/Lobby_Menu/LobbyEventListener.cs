using UnityEngine;
using System.Collections;
using Steamworks;

namespace Menu.Lobby
{
    /// <summary>
    /// event listener.
    /// Listens to events called
    /// by steammatchmaker e.g join lobby
    /// </summary>
    [RequireComponent(typeof(LobbyManager))]
    public class LobbyEventListener : MonoBehaviour
    {
        #region ATTRIBUTES

        // event listener causes the behaviour to preform
        // certain tasks
        [SerializeField]
        private LobbyManager m_con;

        // event listener listens to the callback events
        [SerializeField]
        private LobbyCallback m_cal;

        #endregion

        #region MONO BEHAVIOUR

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

        #endregion

        #region CALLBACK EVENTS

        /// <summary>
        /// Listens for when attributes list is complete
        /// and calls behaviour to start next step
        /// </summary>
        private void OnLobbyListCompleted(CSteamID isNil)
        {
            m_con.JoinSuitableLobby();
        }

        /// <summary>
        /// listens for successful connection to lobby
        /// </summary>
        /// <param name="pLobby"></param>
        private void OnJoinSuccess(CSteamID pLobby)
        {
            m_con.JoinAttemptSuccess(pLobby);
        }

        /// <summary>
        /// listens for lobby connection failure
        /// </summary>
        /// <param name="pLobby"></param>
        private void OnJoinFailed(CSteamID pLobby)
        {
            m_con.JoinAttemptFailed(pLobby);
        }

        /// <summary>
        /// listens for if a creation attempt has failed
        /// error will need to be displayed
        /// </summary>
        /// <param name="isNil"></param>
        private void OnCreateFailed(CSteamID isNil)
        {
            m_con.OnLobbyCreationFailed();
        }

        /// <summary>
        /// If lobby was successfully create inform controller
        /// </summary>
        /// <param name="pLobby"></param>
        private void OnCreateSuccess(CSteamID pLobby)
        {
            m_con.OnLobbyCreationSuccess(pLobby);
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
            m_con.StartLobbySearch();
        }

        /// <summary>
        /// When the player leaves the lobby
        /// inform the controller to create a 
        /// private and 
        /// </summary>
        public void LeaveLobby()
        {
            m_con.QuitLobby();
        }

        /// <summary>
        /// prompts the controller to activate the steam 
        /// friend invite overlay
        /// </summary>
        public void InviteFriends()
        {
            m_con.InviteFriends();
        }

        #endregion
    }
}
