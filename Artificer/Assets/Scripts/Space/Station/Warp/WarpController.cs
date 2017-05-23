using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Stations
{
    /// <summary>
    /// Extends most station functionality 
    /// however behaves differently when deployed
    /// </summary>
    [RequireComponent(typeof(WarpAttributes))]
    public class WarpController : StationController
    {
        #region MONOBEHAVIOUR

        void Awake()
        {
            Att.CurrentIntegrity = Att.Integrity;

            Att.Type = STATIONTYPE.WARP;
        }

        private void OnDestroy()
        {
            Att.Team.WarpSyncList.Remove(netId.Value);
        }

        #endregion

        #region PUBLIC INTERACTION

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
            distance = Distance;

            playerGO.transform.position = deployPoint;
        }

        #endregion

        #region OVERRIDE PARENT

        /// <summary>
        /// overrides the initialize
        /// tailored to warp function
        /// </summary>
        /// <param name="newID"></param>
        /// <param name="newTeam"></param>
        public override void Initialize(NetworkInstanceId newTeam, bool ignore)
        {
            // For now call the base class till actions are different
            base.Initialize(newTeam, true);

            // Add ourselves to static reference list
            Att.Team.WarpSyncList.Add(netId.Value);
        }

        /// <summary>
        /// Returns the attributes in warp type
        /// </summary>
        public new WarpAttributes Att
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

        #region ACCESSORS

        public List<uint> Nearby
        {
            get
            {
                // Create container list for nearby warps
                List<uint> nearbyWarps = 
                    new List<uint>();

                // find each within distance
                foreach(uint warpID in
                    Att.Team.WarpSyncList)
                {
                    // ignore self
                    if (warpID == netId.Value)
                        continue;
                    GameObject warpObj = ClientScene.FindLocalObject
                        (new NetworkInstanceId(warpID));

                    if(Vector2.Distance(transform.position, 
                        warpObj.transform.position) <= Att.WarpRadius)
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
    }
}