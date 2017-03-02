using Data.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Lobby
{
    /// <summary>
    /// updates attaches object UI with playerdata
    /// Has a accessor to ID for removal
    /// </summary>
    public class PlayerLobbyItem : NetworkBehaviour
    {
        #region ATTRIBUTES

        [Header("HUD Elements")]

        [SerializeField]
        private Text m_nameText;

        [SyncVar]
        private PlayerData m_playerData;

        #endregion

        #region ACCESSOR

        public int ID
        {
            get { return m_playerData.PlayerID; }
        }

        #endregion

        #region PUBLIC INTERACTION

        public void DefinePlayer(PlayerData newPlayer)
        {
            m_playerData = newPlayer;

            RpcSetDisplay();
        }

        #endregion

        [ClientRpc]
        private void RpcSetDisplay()
        {
            // update HUD 
            m_nameText.text = m_playerData.PlayerName;
        }
    }
}