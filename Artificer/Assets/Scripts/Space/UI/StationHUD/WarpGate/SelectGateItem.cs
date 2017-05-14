using Stations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

using UI;

namespace Space.UI.Station.Map
{
    /// <summary>
    /// Controller object for individual 
    /// Warp Gate HUD Items
    /// </summary>
    public class SelectGateItem : SelectableHUDItem
    {
        #region ATTRIBUTES

        private WarpController m_warpCon;

        #endregion

        #region ACCESSOR

        public WarpController WarpGate
        {
            get { return m_warpCon; }
            set { m_warpCon = value; }
        }

        #endregion

        #region PUBLIC INTERACTION

        public void InitializeWarpGate(NetworkInstanceId netID)
        {
            // retreive and store controller
            GameObject GO = 
                ClientScene.FindLocalObject(netID);

            m_warpCon = GO.GetComponent<WarpController>();
        }

        #endregion
    }
}
