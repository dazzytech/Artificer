﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;

using Networking;

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

        #region MONO BEHAVIOUR

        private void OnEnable()
        {
            ShipSelectItem.OnShipSelected += ShipSelected;
        }

        private void OnDisable()
        {
            ShipSelectItem.OnShipSelected -= ShipSelected;
        }

        #endregion

        #region EVENT LISTENER

        /// <summary>
        /// Called when ship icon is selected
        /// </summary>
        /// <param name="selected"></param>
        public void ShipSelected(ShipSelectItem selected)
        {
            m_con.SelectShip(selected);
        }

        /// <summary>
        /// Sends our spawn info to the Game Controller
        /// todo: check we have ship and spawn selected
        /// </summary>
        public void SpawnPressed()
        {
            // For now we don't use spawn or ship info
            SpawnSelectionMessage ssm = new SpawnSelectionMessage();
            ssm.PlayerID = SystemManager.Space.ID;
            ssm.ShipName = m_con.SelectedName;
            ssm.SpawnID = 0;

            SystemManager.singleton.client.Send((short)MSGCHANNEL.SPAWNPLAYER, ssm);
        }

        /// <summary>
        /// Set the controller countdown and enable spawn button
        /// </summary>
        /// <param name="delay"></param>
        public void SetSpawnTimer(int delay)
        {
            if (m_con != null)
                m_con.EnableSpawn(delay);
        }

        #endregion
    }
}