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
    public class WarpMapController : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private WarpMapAttributes m_att;

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// populates the warp HUD with maps
        /// </summary>
        /// <param name="warpList"></param>
        public void BuildMap(List<NetworkInstanceId> warpList)
        {
            foreach (Transform child in m_att.WarpGateList)
                GameObject.Destroy(child.gameObject);

            // transfer network instances to transforms
            m_att.NearbyWarpGates = new List<WarpGatePrefab>();

            foreach(NetworkInstanceId netGate in warpList)
            {
                GameObject GO = ClientScene.FindLocalObject(netGate);

                if (GO != null)
                {
                    // change to Warp gate storage in future
                    m_att.NearbyWarpGates.Add(BuildWarpGate(netGate));
                }
                else
                    Debug.Log("Error: WarpMapController - BuildMap: Warp Gate not found");
            }

        }

        /// <summary>
        /// Set gate to selected and deselect all others
        /// </summary>
        /// <param name="gate"></param>
        public void SelectGate(WarpGatePrefab gate)
        {
            m_att.SelectedGate = gate;

            foreach (WarpGatePrefab wgp
                in m_att.NearbyWarpGates)
                if (!m_att.SelectedGate.Equals(wgp))
                    wgp.Deselect();
                else
                    wgp.Select();
        }

        public void WarpToGate()
        {
            if(m_att.SelectedGate == null)
                return;

            m_att.SelectedGate.WarpGate.WarpPlayer();

            SystemManager.Space.LeaveStation();
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Add Warp gate to list and map
        /// </summary>
        /// <param name="gate"></param>
        private WarpGatePrefab BuildWarpGate(NetworkInstanceId gate)
        {
            GameObject warpGateItem = Instantiate(m_att.WarpGatePrefab);
            warpGateItem.transform.SetParent(m_att.WarpGateList);

            // init behaviour and return
            WarpGatePrefab warpGateCon = 
                warpGateItem.GetComponent<WarpGatePrefab>();
            warpGateCon.InitializeWarpGate(gate);
            return warpGateCon;
        }

        #endregion
    }
}
