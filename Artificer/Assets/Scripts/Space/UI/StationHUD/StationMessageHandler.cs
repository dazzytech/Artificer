using Space.Ship;
using Space.UI.Station.Map;
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
        private ShipViewerController m_shipViewer;

        [SerializeField]
        private WarpMapController m_warpMap;

        #endregion

        #region PUBLIC INTERACTION

        public void InitializeShipViewer(ShipAttributes ship)
        {
            if (m_warpMap.gameObject.activeSelf)
                m_warpMap.gameObject.SetActive(false);

            if (!m_shipViewer.gameObject.activeSelf)
                m_shipViewer.gameObject.SetActive(true);

            m_shipViewer.InitializeHUD(ship);
        }

        public void InitializeWarpMap(List<uint> warpList, Transform home)
        {
            if (!m_warpMap.gameObject.activeSelf)
                m_warpMap.gameObject.SetActive(true);

            if (m_shipViewer.gameObject.activeSelf)
                m_shipViewer.gameObject.SetActive(false);

            m_warpMap.BuildMap(warpList, home);
        }

        #endregion
    }
}