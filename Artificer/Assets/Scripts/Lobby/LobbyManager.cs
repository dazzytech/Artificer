using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Steamworks;
using Data.UI;

namespace Lobby
{
    /// <summary>
    /// Manager object for Lobby scene
    /// Manages interactions between lobby and user
    /// and server messages e.g. display player in lobby
    /// 
    /// </summary>
    public class LobbyManager : NetworkBehaviour
    {
        [SerializeField]
        private LobbyAttributes m_att;

        #region MONO BEHAVIOUR

        void Start()
        {
            // Define parent of player lobby items
            PlayerLobbyItem.ParentRect = m_att.PlayerList;

            /*if (SteamManager.Initialized)
            {
                //CreateHiddenLobby();
            }*/

            JoinLANServer();
        }

       /* void LateUpdate()
        {
            if (m_att.CurrentLobby != null)
            {
                if (SteamMatchmaking.GetLobbyData(m_att.CurrentLobby.GetID, "live") == "true")
                {
                    // we are in a live game, need the leave option and invite option
                    m_att.SearchBtn.gameObject.SetActive(false);
                    m_att.InviteBtn.gameObject.SetActive(true);
                    m_att.LeaveBtn.gameObject.SetActive(true);
                }
                else
                {
                    //cant leave our private lobby
                    m_att.LeaveBtn.gameObject.SetActive(false);
                    m_att.InviteBtn.gameObject.SetActive(true);
                    // only lobby owners can start search
                    if (SteamMatchmaking.GetLobbyOwner(m_att.CurrentLobby.GetID).Equals(SteamUser.GetSteamID()))
                        m_att.SearchBtn.gameObject.SetActive(true);
                    else
                        m_att.SearchBtn.gameObject.SetActive(false);
                }
            }
            else
            {
                // possible error
                m_att.SearchBtn.gameObject.SetActive(false);
                m_att.InviteBtn.gameObject.SetActive(false);
                m_att.LeaveBtn.gameObject.SetActive(false);
            }
        }*/

        #endregion

        #region PUBLIC INTERATION

        [Server]
        public void InitializeLobby(string type, ServerData server)
        {
            // Build server based on parameters
            switch(type)
            {
                case "lan":
                    m_att.LType = LobbyType.LAN;
                    BuildLANServer(server);
                    break;
                case "steam":
                    m_att.LType = LobbyType.Steam;
                    break;
            }
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
                GetComponent<PlayerLobbyItem>());

            if (m_att.PlayerItems.Count >= m_att.MinPlayers)
                m_att.StartBtn.interactable = true;

            // Update counter on all clients
            UpdateCount(m_att.PlayerItems.Count);
        }

        /// <summary>
        /// iterates through each lobby player
        /// and deletes the indicated player
        /// </summary>
        /// <param name="PlayerID"></param>
        [Server]
        public void DeletePlayerFromLobby(int PlayerID)
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
                    UpdateCount(m_att.PlayerItems.Count);

                    return;
                }
            }

            // Shouldn't reach here
            Debug.Log("Error: Lobby Manager - Delete Player: Player not found.");
        }

        #endregion

        #endregion

        #region PRIVATE UTILITIES

        private void BuildLANServer(ServerData Server)
        {
            // Build control panel based
            // on LAN lobby. 
            // Create Start Match but disable it for now
            m_att.StartBtn.gameObject.SetActive(true);
            m_att.StartBtn.interactable = false;
                

            m_att.SearchBtn.gameObject.SetActive(false);
            m_att.InviteBtn.gameObject.SetActive(false);
            m_att.LeaveBtn.gameObject.SetActive(true);
        }

        private void JoinLANServer()
        {
            m_att.StartBtn.gameObject.SetActive(false);
            m_att.SearchBtn.gameObject.SetActive(false);
            m_att.InviteBtn.gameObject.SetActive(false);
            m_att.LeaveBtn.gameObject.SetActive(true);
        }

        [ClientCallbackAttribute]
        private void UpdateCount(int current)
        {
            m_att.PlayerCount.text = string.Format
                ("Player Count: {0} | Required to Start: {1}",
                current, m_att.MinPlayers);
        }

        #endregion
    }
}
