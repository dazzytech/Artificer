using System.Collections;
using System.Collections.Generic;
using Space.Ship;
using UnityEngine;
using UnityEngine.Networking;
using Space.UI;

namespace Stations
{
    /// <summary>
    /// Extends most station functionality 
    /// however behaves differently when deployed
    /// </summary>
    [RequireComponent(typeof(WarpAttributes))]
    public class WarpController : StationController
    {
        #region ACCESSORS

        /// <summary>
        /// Returns a list of 
        /// other warp gates nearby
        /// </summary>
        public List<uint> Nearby
        {
            get
            {
                // Create container list for nearby warps
                List<uint> nearbyWarps =
                    new List<uint>();

                // find each within distance
                foreach (uint warpID in
                    m_att.Accessor.Team.WarpSyncList)
                {
                    // ignore self
                    if (warpID == netId.Value)
                        continue;
                    GameObject warpObj = ClientScene.FindLocalObject
                        (new NetworkInstanceId(warpID));

                    if (Vector2.Distance(transform.position,
                        warpObj.transform.position) <= m_att.WarpRadius)
                    {
                        // Within distance, keep reference
                        nearbyWarps.Add(warpID);
                    }
                }

                // return our retreived list
                return nearbyWarps;
            }
        }

        #endregion

        #region MONOBEHAVIOUR

        public override void Awake()
        {
            base.Awake();

            m_att.ProximityMessage = "Press Enter to enter Warp Map";

            m_att.Type = STATIONTYPE.WARP;
        }

        private void OnDestroy()
        {
            m_att.Accessor.Team.WarpSyncList.Remove(netId.Value);
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// overrides the initialize
        /// tailored to warp function
        /// </summary>
        /// <param name="newID"></param>
        /// <param name="newTeam"></param>
        [Server]
        public override void Initialize(NetworkInstanceId newTeam, bool ignore)
        {
            // For now call the base class till actions are different
            base.Initialize(newTeam, true);

            // Add ourselves to static reference list
            m_att.Accessor.Team.WarpSyncList.Add(netId.Value);
        }

        #region PLAYER 

        /// <summary>
        /// Display the station warp map
        /// </summary>
        /// <param name="ship"></param>
        public override void Dock(ShipAccessor ship)
        {
            if (ship != null)
            {
                ship.DisableShip();

                // Next is to update the HUD to display the
                // micro stationHUD
                SystemManager.UIState.SetState(UIState.Station);

                SystemManager.UI.InitializeWarpMap(Nearby, transform);
            }
        }

        /// <summary>
        /// Display a custom message
        /// </summary>
        public override void EnterRange()
        {
            SystemManager.UIMsg.DisplayPrompt(string.Format
                ("Press {0} to view warp map.", Control_Config.GetKey("dock", "sys")));
        }

        #endregion

        /// <summary>
        /// Takes player object and warps it to near the warp gate
        /// </summary>
        public void WarpPlayer()
        {
            // retrive player game object or quit
            // if not found
            GameObject playerGO = GameObject.FindGameObjectWithTag
                ("PlayerShip");

            // retreive position around the station
            // retrive a spawn point away from ship using scalar
            float distance = Random.Range(3f, 5f) *
                Mathf.Sign(Random.Range(-1f, 1f));

            Vector3 deployPoint = transform.position;
            deployPoint.x += distance;
            deployPoint.y += distance;

            // Now we have the deploy point just pull ship here
            distance = m_att.Accessor.Distance;

            playerGO.transform.position = deployPoint;
        }

        #endregion

        #region PRIVATE ACCESSORS

        /// <summary>
        /// Returns the attributes in warp type
        /// </summary>
        protected new WarpAttributes m_att
        {
            get
            {
                if (transform == null)
                    return null;
                else if (transform.GetComponent<WarpAttributes>() != null)
                    return transform.GetComponent<WarpAttributes>();
                else
                    return null;
            }
        }

        #endregion
    }
}