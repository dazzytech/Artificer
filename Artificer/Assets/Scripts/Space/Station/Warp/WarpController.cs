﻿using System.Collections;
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

        #endregion
    }
}