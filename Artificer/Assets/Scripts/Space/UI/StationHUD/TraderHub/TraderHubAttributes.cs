using Data.Space;
using Data.Space.Collectable;
using Data.UI;
using Space.Teams;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Space.UI.Station
{
    public class TraderHubAttributes : MonoBehaviour
    {
        #region REFERENCES

        [HideInInspector]
        public TeamController Team;

        #endregion

        #region HUD ELEMENTS

        [Header("HUD Elements")]

        public Text Wallet;

        public Transform PlayerAssets;

        public Transform TeamAssets;

        [Header("Prefabs")]
        public GameObject AssetPrefab;

        #endregion

        #region ASSETS

        /// <summary>
        /// Reference to the player's current 
        /// asset display
        /// </summary>
        public Dictionary<ItemCollectionData, MaterialViewerPrefab> PlayerAssetList;

        /// <summary>
        /// Reference to the teams's current 
        /// asset display
        /// </summary>
        public Dictionary<ItemCollectionData, MaterialViewerPrefab> TeamAssetList;

        #endregion
    }
}