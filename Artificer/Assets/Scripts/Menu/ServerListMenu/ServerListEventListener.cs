using UnityEngine;
using System.Collections;
using Data.Menu;

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

            if(GameManager.Discovery != null)
                GameManager.Discovery.OnServerDiscovery += ServerDiscovered;
        }

        void OnDisable()
        {
            ServerItemPrefab.OnClickedEvent -= ComponentSelected;

            if (GameManager.Discovery != null)
                GameManager.Discovery.OnServerDiscovery -= ServerDiscovered;
        }

        #endregion

        #region EVENT LISTENER

        /// <summary>
        /// Triggered when the user selected the 
        /// button to create a server
        /// send host cmd to gamemanager
        /// </summary>
        public void CreateServer()
        {
            GameManager.CreateServer();
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
            server.ServerPort = "7777";
            server.Visible = false;

            // send
            m_con.BuildServerItem(server);
        }

        #endregion
    }
}
