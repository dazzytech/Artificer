using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lobby
{
    /// <summary>
    /// Listens for events called within the lobby viewer
    /// </summary>
    [RequireComponent(typeof(LobbyManager))]
    public class LobbyEventListener :
        MonoBehaviour
    {
        #region ATTRIBUTES

        // Reference to controller obj
        [SerializeField]
        private LobbyManager m_con;

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
    }
}
