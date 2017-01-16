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
        public List<int> AddedIDs = new List<int>();

        #region HUD ELEMENTS

        public Transform StationList;

        #endregion

        #region PREFABS

        public GameObject StationPrefab;

        #endregion

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Adds station to the HUD display
        /// </summary>
        /// <param name="piece"></param>
        public void AddUIPiece(StationController controller)
        {
            if (AddedIDs.Contains(controller.ID))
                return;

            GameObject newStation = Instantiate(StationPrefab);
            newStation.transform.SetParent(StationList, false);

            StationTracker tracker = newStation.GetComponent<StationTracker>();
            tracker.DefineStation(controller);

            AddedIDs.Add(controller.ID);
        }

        #endregion
    }
}