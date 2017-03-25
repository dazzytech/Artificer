using Data.Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        #region HUD ELEMENTS

        [Header("HUD Elements")]

        public Transform ShipSelectList;

        public Button SpawnButton;

        public Text SpawnDelayText;

        #endregion

        #region PREFABS

        [Header("Prefabs")]
        public GameObject ShipSelectPrefab;

        #endregion
    }
}
