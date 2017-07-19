using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//Artificer
using Space.Ship;
using Space.UI.Station.Editor;
using Space.UI.Station.Viewer;
using Space.UI.Station.Viewer.Prefabs;

namespace Space.UI.Station
{
    #region SHIP DOCK STATE
    
    public enum DockState { MANAGE, EDIT };

    #endregion

    public class ShipDockAttributes : MonoBehaviour
    {
        #region HUD ELEMENTS

        [Header("State Groups")]
        public GameObject[] ManageGOs;
        public GameObject[] EditGOs;

        #region MANAGE

        //public GameObject NewItemPrefab;

        [Header("Manager HUD")]

        [HideInInspector]
        public List<ShipManagePrefab> ShipList;

        public Transform ShipManageList;
        public GameObject ShipManagePrefab;

        #endregion

        #region EDITOR

        [Header("Editor HUD")]

        public GameObject TabPrefab;
        public GameObject TabHeader;

        public GameObject ItemPrefab;
        public GameObject ItemPanel;
        public Scrollbar ItemScroll;

        public List<GameObject> ComponentList;

        #endregion

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

        /// <summary>
        /// Ship index for ship being edited
        /// -1 if is current ship
        /// </summary>
        public int ShipIndex;

        #endregion
    }
}
