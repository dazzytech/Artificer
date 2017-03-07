﻿using UnityEngine;
using System.Collections;
using Data.UI;

namespace Menu.Server
{
    [RequireComponent(typeof(ServerListController))]
    public class ServerListEventListener : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private ServerListController m_con;

        #endregion

        #region MONO BEHAVIOUR

        void OnEnable()
        {
            ServerItemPrefab.OnClickedEvent += ComponentSelected;

            if(SystemManager.Discovery != null)
                SystemManager.Discovery.OnServerDiscovery += ServerDiscovered;
        }

        void OnDisable()
        {
            ServerItemPrefab.OnClickedEvent -= ComponentSelected;

            if (SystemManager.Discovery != null)
                SystemManager.Discovery.OnServerDiscovery -= ServerDiscovered;
        }

        #endregion

        #region EVENT LISTENER

        /// <summary>
        /// Creates a name input popup and then 
        /// triggers the server creation process
        /// </summary>
        public void CreateServer()
        {
            string defaultServer = 
                ("Game_" + Random.Range(0, 10000))
                        .ToString();

            Popup_Dialog.ShowPopup("Name New Server",
                "Define the new server's name.", 
                DialogType.INPUTFIELD,
                defaultServer);

            Popup_Dialog.OnDialogEvent += GetServerName;
        }

        /// <summary>
        /// Prompts controller to join server
        /// </summary>
        public void JoinServer()
        {
            m_con.JoinServer();
        }

        /// <summary>
        /// Triggered when user selects a server item
        /// displays selected items
        /// </summary>
        /// <param name="serverItem"></param>
        public void ComponentSelected(ServerItemPrefab serverItem)
        {
            m_con.ComponentSelected(serverItem);
        }

        /// <summary>
        /// Creates a server data object from
        /// the passed variables and send it to controller
        /// </summary>
        /// <param name="serverIP"></param>
        /// <param name="serverName"></param>
        public void ServerDiscovered(string serverIP, string serverName)
        {
            // Build server
            ServerData server = new ServerData();
            server.ServerIP = serverIP;
            server.ServerName = serverName;
            server.ServerPort = 7777;
            server.Visible = false;

            // send
            m_con.BuildServerItem(server);
        }

        // Listen for a 
        public void GetServerName(DialogResult result, 
            object returnValue)
        {
            if (result == DialogResult.OK)
            {
                string serverName;

                if (returnValue == null)
                    serverName = ("Game_" + Random.Range(0, 10000))
                        .ToString();
                else
                    serverName = returnValue.ToString();

                SystemManager.CreateServer(serverName);
            }

            Popup_Dialog.OnDialogEvent -= GetServerName;
        }

        #endregion
    }
}
