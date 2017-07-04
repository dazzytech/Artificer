using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//Artificer
using Space.Ship;

namespace Space.UI.Station.Viewer
{
    #region SHIP VIEWER STATE
    
    public enum ViewerState { MANAGE, EDIT };

    #endregion

    public class ShipViewerAttributes : MonoBehaviour
    {
        #region HUD ELEMENTS

        public GameObject SelectionListPrefab;
        public GameObject SelectionListPanel;
        public Scrollbar SelectionListScroll;

        public GameObject[] ManageGOs;
        public GameObject[] EditGOs;

        #endregion

        #region VIEWER ATTRIUBTES

        public bool Busy;

        public ViewerState State;

        #endregion

        #region SHIP REFERENCE

        [HideInInspector]
        // reference to player ship data
        public ShipAttributes Ship;

        [HideInInspector]
        // list of components selected to repair
        public List<int> SelectedIDs;

        public ShipViewer Viewer;

        [HideInInspector]
        public List<ComponentListItem> Items;

        #endregion
    }
}
