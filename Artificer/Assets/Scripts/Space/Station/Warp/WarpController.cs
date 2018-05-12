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
    public class WarpController : StationController
    {
        #region ACCESSORS

        #endregion

        #region MONOBEHAVIOUR

        public override void Awake()
        {
            base.Awake();

            m_att.Type = STATIONTYPE.WARP;
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
        }

        #region PLAYER

        public override void Dock(ShipAccessor ship)
        {
            // Do nothing
        }

        public override void UnDock(ShipAccessor ship)
        {
            // do nothing
        }

        public override void Interact(ShipAccessor ship)
        {
            // do nothing
        }

        public override void Idle(ShipAccessor ship)
        {
            // do nothing
        }

        public override void EnterRange(ShipAccessor ship)
        {
            // do nothing 
        }

        public override void ExitRange(ShipAccessor ship)
        {
            // do nothing
        }


        #endregion

        #endregion
    }
}