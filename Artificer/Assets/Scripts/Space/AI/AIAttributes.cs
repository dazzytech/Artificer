using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using UnityEngine.Networking;
using Data.Space;
using Space.Spawn;
using Space.Teams;

namespace Space.AI
{ 
    /// <summary>
    /// Stores all attributes pertaining to ai systems
    /// e.g. ai ships etc for use by ai manager
    /// </summary>
    public class AIAttributes : MonoBehaviour
    {
        /// <summary>
        /// A reference to an agent template used
        /// for spawning an AI agent
        /// </summary>
        public Dictionary<string, AgentData> AgentLibrary;

        public GameParameters Param;

        #region SPAWN MANAGERS
        
        public RaiderSpawnManager RaiderSpawn;

        public List<TeamController> Teams;

        #endregion

        #region HUD ELEMENTS

        [Header("HUD Elements")]
        
        public Transform TeamHUD;

        #endregion

        #region PREFABS

        [Header("Prefab")]

        public GameObject TeamPrefab;

        #endregion
    }
}
