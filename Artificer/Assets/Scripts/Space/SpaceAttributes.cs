using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;
using Data.Space;
using Data.Space.Library;
using Space.SpawnManagers;

// Add classes for data
// that can be serialized and saved

// e.g Player_Data
namespace Space
{
    public class SpaceAttributes : MonoBehaviour
    {
        // track if the player is currently on stage
        public bool PlayerOnStage;

        // store shipdata for playership
        public ShipData PlayerShip;

        // store local client controller ID
        public short playerControllerID;

        //
        // Contract
        //public ContractTracker Contract;
       
        // Spawn Managers
        //public EnemySpawnManager EnemySpawn;

        //public FriendlySpawnManager FriendlySpawn;

        // Add game parameters here
    }
}
