﻿using UnityEngine;
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
        #region ATTRIBUTES

        private CSteamID m_lobbyID;

        #endregion

        #region EVENT

        public delegate void LobbyUpdate();

        public event LobbyUpdate OnDataUpdate;

        public event LobbyUpdate OnUserUpdate;

        public event LobbyUpdate OnChatUpdate;

        #endregion

        #region CONSTRUCTOR

        public LobbyObject(CSteamID lobbyID)
        {
            m_lobbyID = lobbyID;
        }

        #endregion

        #region PUBLIC INTERACTION

        public void Initialize()
        {
            // Initialize callbacks
            m_PersonaStateChange = Callback<PersonaStateChange_t>
                .Create(OnPersonaStateChange);

            m_LobbyDataUpdate = Callback<LobbyDataUpdate_t>
                .Create(OnLobbyDataUpdate);

            m_LobbyChatUpdate = Callback<LobbyChatUpdate_t>
                .Create(OnLobbyChatUpdate);
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
            OnUserUpdate();
        }

        private void OnLobbyDataUpdate(LobbyDataUpdate_t pCallback)
        {
            // Make sure this callback is relevent to our lobby
            if (m_lobbyID != (CSteamID)pCallback.m_ulSteamIDLobby)
                return;

            Debug.Log("Data Update");

            // When lobby data is created it will be retreived

            // Call function to refresh the lobby viewer
            OnDataUpdate();
        }

        private void OnLobbyChatUpdate(LobbyChatUpdate_t pCallback)
        {
            if (m_lobbyID != (CSteamID)pCallback.m_ulSteamIDLobby)
                return;

            Debug.Log("Chat Update");

            // call function to refresh lobby viewer
            OnChatUpdate();
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