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
    public class FOBController : StationController
    {
        #region MONOBEHAVIOUR

        void Awake()
        {
            Att.CurrentIntegrity = Att.Integrity;

            Att.Type = STATIONTYPE.FOB;
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
        }

        #endregion
    }
}