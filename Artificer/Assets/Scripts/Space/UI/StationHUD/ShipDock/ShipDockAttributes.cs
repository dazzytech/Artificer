using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//Artificer
using Space.Ship;
using Space.UI.Station.Editor;
using Space.UI.Station.Viewer;

namespace Space.UI.Station
{
    #region SHIP DOCK STATE
    
    public enum DockState { MANAGE, EDIT };

    #endregion

    public class ShipDockAttributes : MonoBehaviour
    {
        #region HUD ELEMENTS

        public GameObject[] ManageGOs;
        public GameObject[] EditGOs;

        #endregion

        #region DOCK ATTRIUBTES

        public DockState State;

        public ShipViewer Viewer;

        public ShipEditor Editor;

        #endregion

        #region SHIP REFERENCE

        [HideInInspector]
        // reference to player ship data
        public ShipAttributes Ship;

        #endregion
    }
}
