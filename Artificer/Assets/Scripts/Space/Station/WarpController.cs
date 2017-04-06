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
        #region EVENTS

        // When player gets close enough to 
        // warp then enable warping

        public static event StationEvent EnterWarpRadius;
        public static event StationEvent ExitWarpRadius;

        #endregion

        #region MONOBEHAVIOUR
        
        // overwrite these functions for different behaviours

        void Awake()
        {
            Att.CurrentIntegrity = Att.Integrity;

            Att.Type = STATIONTYPE.WARP;
        }

        void Start()
        {
            if (!Att.Interactive)
                StartCoroutine("CheckForActivity");
        }

        void OnDestroy()
        {

        }

        #endregion

        #region OVERRIDE PARENT

        /// <summary>
        /// overrides the initialize
        /// tailored to warp function
        /// </summary>
        /// <param name="newID"></param>
        /// <param name="newTeam"></param>
        public override void Initialize(int newID, NetworkInstanceId newTeam)
        {
            // Store our ID for when the station is destroyed
            Att.ID = newID;

            // Just set interactive to true for now
            Att.Interactive = false;

            // reference to our team
            Att.TeamID = newTeam;

            // place station under correct parent
            transform.SetParent(Att.Team.transform);
        }

        /// <summary>
        /// 
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
    }
}