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

        public static Transform ParentRect;

        public static Space.UI.MessageHUD Message;

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

        public string Name
        {
            get { return m_playerData.PlayerName; }
        }

        #endregion

        private void OnDestroy()
        {
            Message.DisplayMessege(new Space.UI.MsgParam("bold",
                        Name + " has left the Lobby"));
        }

        #region NETWORKBEHAVIOUR

        public override void OnStartAuthority()
        {
            if (hasAuthority)
            {
                CmdDefinePlayer(SystemManager.Player);
            }
        }

        #endregion

        #region CMD & RPC

        /// <summary>
        /// Pull player data from 
        /// system manager and update all clients
        /// </summary>
        [Command]
        public void CmdDefinePlayer(PlayerData myPlayer)
        {
            m_playerData = myPlayer;

            RpcSetDisplay();
        }

        #endregion

        [ClientRpc]
        private void RpcSetDisplay()
        {
            // update HUD 
            m_nameText.text = m_playerData.PlayerName;

            transform.SetParent(ParentRect);

            Message.DisplayMessege(new Space.UI.MsgParam("bold", m_playerData.PlayerName + " has entered the Lobby"));
        }
    }
}