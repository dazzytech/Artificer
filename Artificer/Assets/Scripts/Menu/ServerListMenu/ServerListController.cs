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

        void OnEnable()
        {
            // until we have online servers
            // just start local discovery here
            GameManager.StartListening();
        }

        void OnDisable()
        {
            GameManager.StopListening();
        }

        void Awake()
        {
            // Initialize variables
            m_att.SelectedServer = null;

            m_att.ServerList = new List<ServerItemPrefab>();

            // Define player information from either
            // steam or savedinfo?
            m_att.PlayerSelf = new PlayerData();
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
                (x => x.Server.ServerIP == serverData.ServerIP)
                != null)
                return;

            // Build gameobject
            GameObject ServerItemGO = Instantiate(m_att.ServerPrefab);
            ServerItemGO.transform.SetParent(m_att.ServerListTransform);

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
            if (!m_att.SelectedServer.Equals(serverItem))
            {
                m_att.SelectedServer.SetSelected(false);
                m_att.SelectedServer = serverItem;
                m_att.SelectedServer.SetSelected(true);
            }
        }

        /// <summary>
        /// If we have a server selected 
        /// then command gamemanager to join 
        /// that IP as client
        /// </summary>
        public void JoinServer()
        {
            if (m_att.SelectedServer == null)
                return;

            // We have a selected server
            string ipAddress = m_att.SelectedServer.Server.ServerIP;

            GameManager.JoinAsClient(ipAddress);
        }

        #endregion

        #region PRIVATE UTILITIES



        #endregion

    }
}
