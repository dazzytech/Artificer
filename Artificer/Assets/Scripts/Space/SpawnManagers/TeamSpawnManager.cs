using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Data.Shared;
using Data.Space;
using Space.Segment.Generator;

namespace Space.SpawnManagers
{
    /// <summary>
    /// Information stored by the spawn mananger on each
    /// player. Each time a player joins it will create an 
    /// item and on diconnect will destroy it.
    /// </summary>
    public class PlayerSpawnInfo
    {
        public short mController;
        public NetworkConnection mConnection;
        public GameObject mGO;
    }

    /// <summary>
    /// template class for team spawn managers
    /// handles the creation of initial station and 
    /// specific team spawn points as well as managing 
    /// ships for that team. if player connects they will be given a ship
    /// and stored within spawnmanager memory linked to that ship
    /// ONLY EXISTS ON SERVER
    /// </summary>
    public class TeamSpawnManager: NetworkBehaviour
    {
        // stores the station generator to return stations
        public StationGenerator stationGen;
        // builds the team spawn points
        public SpawnPointGenerator spawnGen;

        // Store a list of all connected players
        private List<PlayerSpawnInfo> _playerSpawnInfoList;

        // temp list of spawn points
        private SpawnPointData[] _spawns;

        /// <summary>
        /// Adds the new player.
        /// called by game manager and returns the 
        /// ship the player wants.
        /// </summary>
        /// <returns>The new player.</returns>
        /// <param name="conn">Conn.</param>
        /// <param name="client">Client.</param>
        [Server]
        public void AddNewPlayer
            (short playerControllerId, NetworkConnection conn)
        {
            // store info for replacing ship when destroyed
            PlayerSpawnInfo info = new PlayerSpawnInfo();
            info.mController = playerControllerId;
            info.mConnection = conn;
            info.mGO = Instantiate(GameManager.singleton.playerPrefab); ;
    

            // add player to tracking list
            if(_playerSpawnInfoList == null)
                _playerSpawnInfoList = new List<PlayerSpawnInfo>();

            _playerSpawnInfoList.Add(info);

            // This will become invoke
            SpawnNewPlayer(info);
        }

        // called by clients when their ship is destroyed
        //[Command]
        //public void CmdSpawnNewPlayerShip(short conn)
        //{
            //Debug.Log("Received Spawn Request");
        //}

        [Server]
        public void ImportSpawnList(SpawnPointData[] spawns)
        {
            _spawns = spawns;
        }

        [Server]
        private void SpawnPlayerShip(PlayerSpawnInfo info)
        {
            NetworkServer.ReplacePlayerForConnection
                (info.mConnection, info.mGO, info.mController);
        }

        /// <summary>
        /// Spawns player at random spawnpoint for first time
        /// </summary>
        /// <param name="info"></param>
        [Server]
        private void SpawnNewPlayer(PlayerSpawnInfo info)
        {
            Vector2 newPosition = _spawns[Random.Range(0, _spawns.Length)].Position;

            info.mGO.transform.position = newPosition; ;

            NetworkServer.AddPlayerForConnection
                (info.mConnection, info.mGO, info.mController);
        }
    }
}
