using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;
using Data.Space;
using Data.Space.Library;
using Space.Segment;
using Stations;
using Space.Teams;

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
        //public ShipData PlayerShip;

        // store local client ID on server
        public int playerID;

        // Network instance ID for player's ship
        public uint netID;

        // if the player is within vicinity of station
        public bool overStation;

        // if player is within construction Range
        public bool buildRange;

        // refence to the station we are currently at
        public StationController station;

        public bool docked;

        // Reference to player team
        public TeamController Team;

        #endregion

        //
        // Contract
        //public ContractTracker Contract;

        // Spawn Managers
        //public EnemySpawnManager EnemySpawn;

        //public FriendlySpawnManager FriendlySpawn;

        // Add game parameters here
    }
}
