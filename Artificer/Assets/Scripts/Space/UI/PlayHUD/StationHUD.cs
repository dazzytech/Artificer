using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Artificer
using Space.Segment;

namespace Space.UI.Ship
{
    /// <summary>
    /// Resposible for adding station information
    /// to the HUD
    /// </summary>
    public class StationHUD : BasePanel
    {
        #region ATTRIBUTES

        // Stop duplicate HUD elements
        private List<int> m_addedIDs = new List<int>();

        #region HUD ELEMENTS

        [Header("Station List Panel")]
        [SerializeField]
        private Transform m_stationList;

        #endregion

        #region PREFABS

        [Header("Station HUD Prefab")]
        [SerializeField]
        private GameObject m_stationPrefab;

        #endregion

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Adds station to the HUD display
        /// </summary>
        /// <param name="piece"></param>
        public void AddUIPiece(StationController controller)
        {
            if (m_addedIDs.Contains(controller.ID))
                return;

            GameObject newStation = Instantiate(m_stationPrefab);
            newStation.transform.SetParent(m_stationList, false);

            StationTracker tracker = newStation.GetComponent<StationTracker>();
            tracker.DefineStation(controller);

            m_addedIDs.Add(controller.ID);
        }

        #endregion
    }
}