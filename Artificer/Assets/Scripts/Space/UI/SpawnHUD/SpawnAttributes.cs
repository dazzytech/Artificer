using Data.Space;
using Space.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UI;

namespace Space.UI.Spawn
{
    /// <summary>
    /// Stores attributes for spawn window
    /// </summary>
    public class SpawnAttributes : MonoBehaviour
    {
        #region SHIP

        [HideInInspector]
        public ShipUIPrefab SelectedShip;
        [HideInInspector]
        public List<ShipUIPrefab> ShipList;

        #endregion

        #region SPAWN

        [HideInInspector]
        public SpawnSelectItem SelectedSpawn;
        [HideInInspector]
        public List<SpawnSelectItem> SpawnList;

        #endregion

        #region HUD ELEMENTS

        [Header("HUD Elements")]

        public Transform ShipSelectList;

        public MapViewer Map;

        #region USER PANEL

        /// <summary>
        /// Expends currency to spawn the ship
        /// </summary>
        public Button SpawnButtonCurrency;

        public Text CurrentShipCost;

        public Text PlayerCurrency;

        #endregion

        #endregion

        #region PREFABS

        [Header("Prefabs")]

        public GameObject ShipSelectPrefab;

        public GameObject StationSelectPrefab;

        #endregion
    }
}
