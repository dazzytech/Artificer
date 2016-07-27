using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;
using Data.Space;
using Data.Space.Library;
using Space.GameFunctions;
using Space.SpawnManagers;

// Add classes for data
// that can be serialized and saved

// e.g Player_Data
namespace Space
{
    /// <summary>
    /// Central attributes container for space manager
    /// stores player state.
    /// </summary>
    public class SpaceAttributes : MonoBehaviour
    {
        #region PLAYER TRACKING

        // track if the player is currently on stage
        public bool PlayerOnStage;

        // store shipdata for playership
        public ShipData PlayerShip;

        // store local client controller ID
        public short playerControllerID;

        #endregion

        //
        // Contract
        //public ContractTracker Contract;

        // temp game builderr
        public GameStateController GameController;

        // Spawn Managers
        //public EnemySpawnManager EnemySpawn;

        //public FriendlySpawnManager FriendlySpawn;

        // Add game parameters here
    }
}
