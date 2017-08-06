﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;

using Networking;
using UI;
using Data.Space.Library;
using Data.Space;

namespace Space.UI.Spawn
{
    /// <summary>
    /// Interaction with spawn selector hud e.g.
    /// Select Ship, Select spawn, Spawn button down
    /// </summary>
    public class SpawnEventListener : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private SpawnController m_con;

        #endregion

        #region EVENT LISTENER

        /// <summary>
        /// Called when ship icon is selected
        /// </summary>
        /// <param name="selected"></param>
        public void ShipSelected(SelectableHUDItem selected)
        {
            m_con.SelectShip((ShipUIPrefab)selected);
        }

        /// <summary>
        /// Called when ship icon is selected
        /// </summary>
        /// <param name="selected"></param>
        public void SpawnSelected(SelectableHUDItem selected)
        {
            m_con.SelectSpawn((SpawnSelectItem)selected);
        }

        /// <summary>
        /// Sends our spawn info to the Game Controller
        /// todo: check we have ship and spawn selected
        /// </summary>
        public void SpawnPressed()
        {
            WalletData temp = SystemManager.Wallet;

            if (temp.Purchase(m_con.SelectedShip.Cost))
            {
                SystemManager.Wallet = temp;

                // For now we don't use spawn or ship info
                SpawnSelectionMessage ssm = new SpawnSelectionMessage();
                ssm.PlayerID = SystemManager.Space.ID;
                ssm.Ship = m_con.SelectedShip;
                ssm.SpawnID = m_con.SelectedSpawn;

                SystemManager.singleton.client.Send((short)MSGCHANNEL.SPAWNPLAYER, ssm);

                SystemManager.Space.StartCoroutine
                    ("UpdatePlayerSpawn", m_con.SelectedShipID);
            }
        }

        #endregion
    }
}
