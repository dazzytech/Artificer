using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Artificer
using Data.Menu;

namespace Menu.Server
{
    /// <summary>
    /// Container for server list
    ///     -   Contains playerdata for self (initialized on startup)
    ///     -   Contains server list retrieved from network discovery
    ///     -   Contains server that has been selected 
    ///         Set by event listener
    /// </summary>
    public class ServerListAttributes : MonoBehaviour
    {
        #region SERVER LIST

        [Header("Server List Attributes")]

        public PlayerData PlayerSelf;

        public List<ServerData> ServerList;

        public ServerData SelectedServer;

        #endregion

        #region HUD ELEMENTS

        //[Header("HUD Elements")]

        #endregion

        // At some point will consider filters, LAN or Online etc
    }
}