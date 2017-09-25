using Data.Space;
using Data.UI;
using Space.Teams;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.UI.Station
{
    /// <summary>
    /// Interface with trader station or team assets
    /// allows player to trade
    /// </summary>
    [RequireComponent(typeof(TraderHubAttributes))]
    public class TraderHubController : HUDPanel
    {
        #region ATTRIBUTES

        [SerializeField]
        private TraderHubAttributes m_att;

        [SerializeField]
        private TraderHubEventListener m_event;

        #endregion

        #region PUBLIC INTERACTION

        #region INITIALIZE

        /// <summary>
        /// Initializes the station HUD.
        /// for now only pass ship atts
        /// Entry Point
        /// </summary>
        /// <param name="param">Parameter.</param>
        public void InitializeHUD()
        {
            m_att.Player = SystemManager.Player;

            m_att.Team = SystemManager.Space.Team;
        }

        #endregion

        #endregion
    }
}