using Data.UI;
using Space.Ship;
using Space.Teams;
using Space.UI.Station.Viewer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Space.UI.Station
{
    /// <summary>
    /// Processes messages from external sources
    /// and initializes different parts of
    /// the HUD based on input
    /// </summary>
    public class StationMessageHandler : MonoBehaviour
    {
        #region ATTRIBUTES

        [Header("HUD Elements")]
        [SerializeField]
        private ShipDockController m_shipDock;

        [SerializeField]
        private TraderHubController m_tradeHub;

        #endregion

        #region PUBLIC INTERACTION

        public void InitializeShipViewer(ShipAccessor ship)
        {
            if (!m_shipDock.gameObject.activeSelf)
                m_shipDock.gameObject.SetActive(true);

            if (m_tradeHub.gameObject.activeSelf)
                m_tradeHub.gameObject.SetActive(false);

            m_shipDock.InitializeHUD(ship);
        }

        public void InitializeTradingHub(TeamController team)
        {
            if (m_shipDock.gameObject.activeSelf)
                m_shipDock.gameObject.SetActive(false);

            if (!m_tradeHub.gameObject.activeSelf)
                m_tradeHub.gameObject.SetActive(true);

            m_tradeHub.InitializeHUD(team);
        }

        #endregion
    }
}