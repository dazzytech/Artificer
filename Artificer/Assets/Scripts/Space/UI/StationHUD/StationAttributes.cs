using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//Artificer
using Space.Ship;
using Space.UI.Station.Viewer;

namespace Space.UI.Station
{
    public class StationAttributes : MonoBehaviour
    {
        #region ATTRIBUTES

        #region HUD ELEMENTS

        public GameObject SelectionListPrefab;
        public GameObject SelectionListPanel;
        public Scrollbar SelectionListScroll;

        #endregion

        #region SHIP REFERENCE

        [HideInInspector]
        // reference to player ship data
        public ShipAttributes Ship;

        [HideInInspector]
        // list of components selected to repair
        public List<int> SelectedIDs;

        public ShipViewer Viewer;

        public bool Busy;

        #endregion

        // contain reference to inner ship panel controller here

        #endregion
    }
}
