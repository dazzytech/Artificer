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

        private void OnDisable()
        {
            WarpAttributes.WarpList.Add(netId);
        }

        #endregion

        #region OVERRIDE PARENT

        /// <summary>
        /// overrides the initialize
        /// tailored to warp function
        /// </summary>
        /// <param name="newID"></param>
        /// <param name="newTeam"></param>
        public override void Initialize(int newID, NetworkInstanceId newTeam, bool ignore)
        {
            // For now call the base class till actions are different
            base.Initialize(newID, newTeam, true);

            if (WarpAttributes.WarpList == null)
                WarpAttributes.WarpList = 
                    new List<NetworkInstanceId>();

            // Add ourselves to static reference list
            WarpAttributes.WarpList.Add(netId);
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

        public List<NetworkInstanceId> Nearby
        {
            get
            {
                // Create container list for nearby warps
                List<NetworkInstanceId> nearbyWarps = 
                    new List<NetworkInstanceId>();

                // find each within distance
                foreach(NetworkInstanceId warpID in
                    WarpAttributes.WarpList)
                {
                    // ignore self
                    if (warpID == netId)
                        continue;
                    GameObject warpObj = ClientScene.FindLocalObject(warpID);

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