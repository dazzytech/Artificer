using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Data.Menu
{
    /// <summary>
    /// Data container for server objects
    /// These are stored in a list within a server 
    /// list or stored singularly by a Lobby object
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
        public string ServerPort;

        public bool Visible;

        // Currently how many players in server
        public int ServerPopulation;

        // Maximum players allowed - Considered server setting not game
        public int ServerMaxPopulation;

        // Game version host is running
        public string ServerVersion;

        #endregion

        #region PLAYER INFORMATION

        // All connected players
        public List<PlayerData> PlayerList;

        // hosting player (can edit game)
        public PlayerData Host;

        #endregion

        #region GAME SETTINGS

        // not yet implemented

        #endregion
    }
}
