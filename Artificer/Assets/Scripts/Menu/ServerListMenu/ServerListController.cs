using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Steamworks;

// Artificer
using Data.Menu;

namespace Menu.Server
{
    /// <summary>
    /// Behavour for server list
    /// responsible for populating the serverlist,
    /// displayed selected server and initializing 
    /// serverlist object
    /// </summary>
    [RequireComponent(typeof(ServerListAttributes))]
    public class ServerListController : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private ServerListAttributes m_att;

        #endregion

        #region MONO BEHAVIOUR

        void Awake()
        {
            // Initialize variables
            m_att.SelectedServer = null;

            m_att.ServerList = null;

            // Define player information from either
            // steam or savedinfo?
            m_att.PlayerSelf = new PlayerData();
        }

        #endregion

        #region PUBLIC INTERACTION



        #endregion

        #region PRIVATE UTILITIES



        #endregion

    }
}
