using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using Networking;

namespace Server
{
    /// <summary>
    /// Listens for events called within the lobby viewer
    /// </summary>
    [RequireComponent(typeof(ServerManager))]
    public class ServerEventListener :
        MonoBehaviour
    {
        #region ATTRIBUTES

        // Reference to controller obj
        [SerializeField]
        private ServerManager m_con;

        #endregion

        #region MONO BEHAVIOUR

        private void Awake()
        {
            NetworkManager.singleton.client.RegisterHandler
                ((short)MSGCHANNEL.LOBBYPLAYERJOINED, OnPlayerJoined);

            NetworkManager.singleton.client.RegisterHandler
                ((short)MSGCHANNEL.LOBBYPLAYERLEFT, OnPlayerLeave);
        }

        #endregion

        #region EVENT LISTENER

        /// <summary>
        /// Leave lobby and go back to title
        /// only act with LAN atm
        /// /// </summary>
        public void LeaveLobby()
        {
            SystemManager.Disconnect();
        }

        public void StartMatch()
        {
            SystemManager.StartMatch();
        }

        #endregion

        #region NETWORK MESSAGES

        public void OnPlayerJoined(NetworkMessage msg)
        {
            m_con.UpdateCount(msg.ReadMessage
                 <IntegerMessage>().value);
        }

        public void OnPlayerLeave(NetworkMessage msg)
        {
            m_con.UpdateCount(msg.ReadMessage
                 <IntegerMessage>().value);
        }

        #endregion
    }
}
