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

        [SyncVar]
        private bool m_spawned;

        [SyncVar]
        private bool m_host;

        [Header("Colours")]

        // Colour for other players
        [SerializeField]
        private Color m_remoteColour;

        // Colour for local players
        [SerializeField]
        private Color m_localColour;

        // Colour if hosting game
        [SerializeField]
        private Color m_hostColour;

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
            CmdDefinePlayer(SystemManager.Player);
        }

        #endregion

        #region MONO BEHAVIOUR

        private void Start()
        {
            if (m_spawned)
                SetOtherDisplay();
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Build display on our client
        /// because player had already spawned
        /// </summary>
        private void SetOtherDisplay()
        {
            // update visual text
            m_nameText.text = m_playerData.PlayerName;

            // Place within lobby player list
            transform.SetParent(ParentRect);

            if (m_host)
                GetComponent<Image>().color = m_hostColour;
            else
                GetComponent<Image>().color = m_remoteColour;
        }

        #region CMD & RPC

        /// <summary>
        /// Pull player data from 
        /// system manager and update all clients
        /// </summary>
        [Command]
        public void CmdDefinePlayer(PlayerData myPlayer)
        {
            // assign info
            m_playerData = myPlayer;

            m_spawned = true;

            m_host = myPlayer.IsHost;

            // display on clients
            RpcSetDisplay(myPlayer);
        }

        #endregion

        /// <summary>
        /// Display information for
        /// player on all clients
        /// </summary>
        /// <param name="myPlayer">passed in param because m_playerData
        /// Won't be assigned yet.</param>
        [ClientRpc]
        private void RpcSetDisplay(PlayerData myPlayer)
        {
            // update visual text
            m_nameText.text = myPlayer.PlayerName;

            // Place within lobby player list
            transform.SetParent(ParentRect);

            if (myPlayer.IsHost)
                GetComponent<Image>().color = m_hostColour;
            else
            {
                if (hasAuthority)
                    GetComponent<Image>().color = m_localColour;
                else
                    GetComponent<Image>().color = m_remoteColour;
            }

            // If we are here than we have just spawned
            // so add to message list
            Message.DisplayMessege(new Space.UI.MsgParam
                ("bold", myPlayer.PlayerName + " has entered the Lobby"));
        }

        #endregion
    }
}