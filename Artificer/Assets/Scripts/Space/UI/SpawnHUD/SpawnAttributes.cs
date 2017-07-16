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
        #region SPAWN CONFIG

        // seconds to spawning
        public int SpawnDelay;

        // Ships that can be spawned
        public List<string> SpawnableShips;

        #endregion

        #region SHIP

        public ShipSelectItem SelectedShip;

        public List<ShipSelectItem> ShipList;

        #endregion

        #region SPAWN

        public SpawnSelectItem SelectedSpawn;

        public List<SpawnSelectItem> SpawnList;

        #endregion

        #region HUD ELEMENTS

        [Header("HUD Elements")]

        public Transform ShipSelectList;

        public Button SpawnButton;

        public Text SpawnDelayText;

        public MapViewer Map;

        #endregion

        #region PREFABS

        [Header("Prefabs")]
        public GameObject ShipSelectPrefab;

        public GameObject StationSelectPrefab;

        #endregion
    }
}
