using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Steamworks;

namespace Data.UI
{
    /// <summary>
    /// Data container for information regarding a server
    /// when a steam lobby is successfully joined then
    /// the information for the server would be copied into
    /// here
    /// Contains:
    ///     -   Server Name
    ///     -   Population Count
    ///     -   Player List
    ///     -   Game Settings
    /// </summary>
    public class ServerData
    {
        #region SERVER INFORMATION

        // Public name
        public string ServerName;

        // Hosts IP
        public string ServerIP;

        // Connection port
        public int ServerPort;

        // Game version host is running
        public string ServerVersion;

        #endregion

        #region PLAYER INFORMATION

        // All connected players
        // may not be needed
        public List<PlayerData> PlayerList;

        #endregion

        #region GAME SETTINGS

        // not yet implemented

        #endregion
    }
}
