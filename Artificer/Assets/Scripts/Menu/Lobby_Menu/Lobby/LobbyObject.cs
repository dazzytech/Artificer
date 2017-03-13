using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Steamworks;
using Data.UI;

namespace Menu.Lobby
{
    /// <summary>
    /// Encapsulated Lobby Object divided into three parts
    /// data members - stores the steamID, Game Options and list of players
    /// Callback - PersonaStateChange, LobbyDataUpdate, LobbyChatUpdate
    /// Implimentation - Communicates with the Lobby Viewer to display the Lobby Object
    /// </summary>
    public class LobbyObject
    {
        #region DATA MEMBERS

        private CSteamID m_lobbyID;
        private LobbyViewer m_viewer;

        // game options or object would be here

        #endregion

        #region CONSTRUCTOR

        public LobbyObject(CSteamID lobbyID, LobbyViewer viewer)//CSteamID lobbyID, LobbyViewer viewer)
        {
            m_viewer = viewer;
            m_lobbyID = lobbyID;

            m_viewer.AssignLobby(m_lobbyID);

            // Initialize callbacks
            m_PersonaStateChange = Callback<PersonaStateChange_t>
                .Create(OnPersonaStateChange);

            m_LobbyDataUpdate = Callback<LobbyDataUpdate_t>
                .Create(OnLobbyDataUpdate);

            m_LobbyChatUpdate = Callback<LobbyChatUpdate_t>
                .Create(OnLobbyChatUpdate);

            // Call function to refresh lobby viewer
            m_viewer.ViewLobby();
        }

        #endregion

        #region CALLBACK

        // Handles a user changing their details
        private Callback<PersonaStateChange_t> m_PersonaStateChange;

        // Handles lobby data changing
        private Callback<LobbyDataUpdate_t> m_LobbyDataUpdate;

        // handle users in the lobby joining or leaving
        private Callback<LobbyChatUpdate_t> m_LobbyChatUpdate;

        #endregion

        #region IMPLEMENT

        // Listeners
        private void OnPersonaStateChange(PersonaStateChange_t pCallback)
        {
            if(!SteamFriends.IsUserInSource((CSteamID)pCallback.m_ulSteamID, m_lobbyID))
            {
                return;
            }

            Debug.Log("Persona Update");

            // Call function to refresh lobby viewer
            m_viewer.ViewLobby();
        }

        private void OnLobbyDataUpdate(LobbyDataUpdate_t pCallback)
        {
            // Make sure this callback is relevent to our lobby
            if (m_lobbyID != (CSteamID)pCallback.m_ulSteamIDLobby)
                return;

            Debug.Log("Data Update");

            // When lobby data is created it will be retreived

            // Call function to refresh the lobby viewer
            m_viewer.ViewLobby();
        }

        private void OnLobbyChatUpdate(LobbyChatUpdate_t pCallback)
        {
            if (m_lobbyID != (CSteamID)pCallback.m_ulSteamIDLobby)
                return;

            Debug.Log("Chat Update");

            // call function to refresh lobby viewer
            m_viewer.ViewLobby();
        }

        #endregion

        #region ACCESSORS

        public CSteamID GetID
        {
            get { return m_lobbyID; }
        }

        #endregion
    }
}
