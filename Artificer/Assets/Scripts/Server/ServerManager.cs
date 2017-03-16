using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using Networking;
using Steamworks;
using Data.UI;

namespace Server
{
    /// <summary>
    /// Manager object for Lobby scene
    /// Manages interactions between lobby and user
    /// and server messages e.g. display player in lobby
    /// 
    /// </summary>
    public class ServerManager : NetworkBehaviour
    {
        [SerializeField]
        private ServerAttributes m_att;

        #region MONO BEHAVIOUR

        void Start()
        {
            // Define parent of player lobby items
            PlayerServerItem.ParentRect = m_att.PlayerList;
            PlayerServerItem.Message = m_att.Message;

            if(!isServer)
                JoinServer();
        }

        #endregion

        #region PUBLIC INTERATION

        [Server]
        public void InitializeLobby(ServerData server)
        {
            // Build control panel based
            // on LAN lobby. 
            // Create Start Match but disable it for now
            m_att.StartBtn.gameObject.SetActive(true);
            m_att.StartBtn.interactable = false;

            m_att.LeaveBtn.gameObject.SetActive(true);
        }

        public void UpdateCount(int current)
        {
            m_att.PlayerCountText.text = string.Format
                ("Player Count: {0} | Required to Start: {1}",
                current, m_att.MinPlayers);
        }

        #region GAME MANAGER CALLS

        /// <summary>
        /// called from event listener when
        /// game manager calls to add player to lobby list
        /// </summary>
        /// <param name="Player"></param>
        [Server]
        public void AddPlayerToLobby
            (NetworkConnection conn)
        {
            // Spawn icon with player authority
            // Create gameobject for player lobby
            GameObject PlayerLobbyGO = Instantiate(m_att.PlayerPrefab);

            // Spawn with player auth
            NetworkServer.SpawnWithClientAuthority
                (PlayerLobbyGO, conn);

            // Keep reference
            m_att.PlayerItems.Add(PlayerLobbyGO.
                GetComponent<PlayerServerItem>());

            if (m_att.PlayerItems.Count >= m_att.MinPlayers)
                m_att.StartBtn.interactable = true;

            // Update counter on all clients
            NetworkServer.SendToAll((short)MSGCHANNEL.LOBBYPLAYERJOINED, 
                new IntegerMessage(m_att.PlayerItems.Count));
        }

        /// <summary>
        /// iterates through each lobby player
        /// and deletes the indicated player
        /// </summary>
        /// <param name="PlayerID"></param>
        [Server]
        public void DeletePlayerFromServer(int PlayerID)
        {
            for(int i = 0; i < m_att.PlayerItems.Count; i++)
            {
                if(m_att.PlayerItems[i].ID == PlayerID)
                {
                    // the one to delete
                    NetworkServer.UnSpawn
                        (m_att.PlayerItems[i].gameObject);
                    Destroy(m_att.PlayerItems[i].gameObject);
                    m_att.PlayerItems.RemoveAt(i);

                    if (m_att.PlayerItems.Count < m_att.MinPlayers)
                        m_att.StartBtn.interactable = false;

                    // update player counter
                    NetworkServer.SendToAll((short)MSGCHANNEL.LOBBYPLAYERLEFT,
                        new IntegerMessage(m_att.PlayerItems.Count));

                    return;
                }
            }
            // Shouldn't reach here
            Debug.Log("Error: Server Manager - Delete Player: Player not found.");
        }

        #endregion

        #endregion

        #region PRIVATE UTILITIES

        private void JoinServer()
        {
            m_att.StartBtn.gameObject.SetActive(false);
            m_att.LeaveBtn.gameObject.SetActive(true);
        }

        #endregion
    }
}
