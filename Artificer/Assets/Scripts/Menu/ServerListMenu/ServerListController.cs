﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Steamworks;

// Artificer
using Data.UI;
using UI;

namespace Menu.Server
{
    /// <summary>
    /// Behavour for server list
    /// responsible for populating the serverlist,
    /// displayed selected server and initializing 
    /// serverlist object
    /// </summary>
    [RequireComponent(typeof(ServerListAttributes))]
    public class ServerListController : HUDPanel
    {
        #region ATTRIBUTES

        [SerializeField]
        private ServerListAttributes m_att;

        #endregion

        #region MONO BEHAVIOUR

        protected override void OnEnable()
        {
            // until we have online servers
            // just start local discovery here
            SystemManager.StartListening();

            DisplayPlayerSelf();
        }

        protected override void OnDisable()
        {
            SystemManager.StopListening();
        }

        void Awake()
        {
            // Initialize variables
            m_att.SelectedServer = null;

            m_att.ServerList = new List<ServerItemPrefab>();
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Creates an instance of the server
        /// list item and displays it
        /// </summary>
        /// <param name="serverData"></param>
        public void BuildServerItem(ServerData serverData)
        {
            // quit if we already have this item
            if (m_att.ServerList.Find
                (x => x.Server.InternalIP == serverData.InternalIP)
                != null)
                return;

            // Build gameobject
            GameObject ServerItemGO = Instantiate(m_att.ServerPrefab);
            ServerItemGO.transform.SetParent(m_att.ServerListTransform, false);

            // retrieve server item prefab
            ServerItemPrefab ServerItem = 
                ServerItemGO.GetComponent<ServerItemPrefab>();
            ServerItem.BuildServer(serverData);

            // store server item
            m_att.ServerList.Add(ServerItem);
        }

        /// <summary>
        /// Processes player clicking on a server
        /// </summary>
        /// <param name="serverItem"></param>
        public void ComponentSelected(ServerItemPrefab serverItem)
        {
            // If server is null then just set selection
            if(m_att.SelectedServer == null)
            {
                m_att.SelectedServer = serverItem;
                m_att.SelectedServer.SetSelected(true);
                return;
            }

            // Server been selected before
            if (!m_att.SelectedServer.Equals(serverItem))
            {
                m_att.SelectedServer.SetSelected(false);
                m_att.SelectedServer = serverItem;
                m_att.SelectedServer.SetSelected(true);
            }
        }

        /// <summary>
        /// If we have a server selected 
        /// then command SystemManager to join 
        /// that IP as client
        /// </summary>
        public void JoinServer()
        {
            if (m_att.SelectedServer == null)
                return;

            SystemManager.JoinClient(m_att.SelectedServer.Server);
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// If connected to then retrieves the players
        /// steam information and displays,
        /// otherwise displays default unregistered player
        /// </summary>
        private void DisplayPlayerSelf()
        {
            // display name
            m_att.AccountUserName.text = 
                SystemManager.Player.PlayerName;
        }

        #endregion

    }
}
