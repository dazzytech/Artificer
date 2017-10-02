using Space.Ship;
using Space.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Stations
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(TraderAttributes))]
    public class TraderController : StationController
    {
        #region ACCESSORS

        /// <summary>
        /// Returns the attributes 
        /// </summary>
        protected new TraderAttributes m_att
        {
            get
            {
                if (transform == null)
                    return null;
                else if (transform.GetComponent<TraderAttributes>() != null)
                    return transform.GetComponent<TraderAttributes>();
                else
                    return null;
            }
        }

        #endregion

        #region MONOBEHAVIOUR

        public override void Awake()
        {
            base.Awake();

            m_att.Type = STATIONTYPE.TRADER;
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
        public override void Initialize(NetworkInstanceId newTeam, bool build)
        {
            // For now call the base class till actions are different
            base.Initialize(newTeam, build);

            m_att.ProximityMessage = string.Format
                ("Press {0} to enter trader hub.", Control_Config.GetKey("dock", "sys"));
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

                SystemManager.UI.InitializeTradeHub(m_att.Accessor.Team);
            }
        }

        #endregion

        #endregion
    }
}
