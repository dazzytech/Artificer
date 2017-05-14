using Space.Map;
using Stations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Space.UI.Station.Map
{
    /// <summary>
    /// Manages Warp Map for warp target selection etc
    /// 
    /// </summary>
    [RequireComponent(typeof(WarpMapAttributes))]
    [RequireComponent(typeof(WarpMapEventListener))]
    public class WarpMapController : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private WarpMapAttributes m_att;

        [SerializeField]
        private WarpMapEventListener m_event;

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// populates the warp HUD with maps
        /// </summary>
        /// <param name="warpList"></param>
        public void BuildMap(List<NetworkInstanceId> warpList)
        {
            m_att.Map.InitializeMap(new MapObjectType[] { MapObjectType.SHIP });

            // transfer network instances to transforms
            m_att.NearbyWarpGates = new List<SelectGateItem>();

            foreach(NetworkInstanceId netGate in warpList)
            {
                GameObject GO = ClientScene.FindLocalObject(netGate);

                if (GO != null)
                {
                    // change to Warp gate storage in future
                    m_att.NearbyWarpGates.Add(BuildWarpGate(GO.transform));
                }
                else
                    Debug.Log("Error: WarpMapController - BuildMap: Warp Gate not found");
            }

            m_att.WarpButton.interactable = false;
        }

        /// <summary>
        /// Set gate to selected and deselect all others
        /// </summary>
        /// <param name="gate"></param>
        public void SelectGate(SelectGateItem gate)
        {
            m_att.SelectedGate = gate;

            foreach (SelectGateItem wgp
                in m_att.NearbyWarpGates)
                if (!m_att.SelectedGate.Equals(wgp))
                    wgp.Deselect();
                else
                    wgp.Select();

            m_att.WarpButton.interactable = true;
        }

        public void WarpToGate()
        {
            if(m_att.SelectedGate == null)
                return;

            SystemManager.Space.LeaveStation();

            m_att.SelectedGate.WarpGate.WarpPlayer();
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Add Warp gate to list and map
        /// </summary>
        /// <param name="gate"></param>
        private SelectGateItem BuildWarpGate(Transform gate)
        {
            GameObject warpGateObj= Instantiate(m_att.WarpGatePrefab);

            // init behaviour and return
            SelectGateItem warpGateCon =
                warpGateObj.GetComponent<SelectGateItem>();

            warpGateCon.WarpGate = gate.gameObject.
                GetComponent<WarpController>();

            warpGateCon.Initialize(m_event.WarpSelected);

            MapObject mObj = SystemManager.Space.GetMapObject(gate);
            m_att.Map.DeployPrefab(mObj, warpGateObj);

            return warpGateCon;
        }

        #endregion
    }
}
