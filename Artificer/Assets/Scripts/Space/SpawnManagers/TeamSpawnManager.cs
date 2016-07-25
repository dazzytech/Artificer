using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Data.Shared;
using Data.Space;
using Space.Segment.Generator;

namespace Space.SpawnManagers
{
    #region SPAWN INFO
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

    #endregion

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
        #region ATTRIBUTES

        // stores the station generator to return stations
        public StationGenerator stationGen;
        // builds the team spawn points
        public SpawnPointGenerator spawnGen;

        // Store a list of all connected players
        private List<PlayerSpawnInfo> _playerSpawnInfoList;

        // temp list of spawn points
        private SpawnPointData[] _spawns;

        #endregion

        #region PUBLIC INTERACTION

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

        #endregion

        #region PLAYER SPAWNING

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
            // used to track if the immediate vicinity for spawning is clear
            bool areaClear = false;

            // Space in Units in a radius we want to be clear
            float minDistance = 2;

            // take position from spawn point we want (TODO:player will select team spawn)
            Vector2 newPosition = _spawns[Random.Range(0, _spawns.Length)].Position;

            // Check area is clear, if not then shift away and repeat
            while (!areaClear)
            {
                // Only need one to know we will collide
                RaycastHit2D hit = Physics2D.CircleCast(newPosition, minDistance, Vector2.zero);

                if (hit.transform == null)
                {
                    // Vicinity is cleared for spawning
                    areaClear = true;
                }
                else
                {
                    // move our start position out of the way
                    newPosition += new Vector2
                        (Random.Range(-1f, 1f), Random.Range(-1f, 1f))
                        * minDistance;
                }
            }

            // apply position
            info.mGO.transform.position = newPosition;

            // spawn player
            if (NetworkServer.AddPlayerForConnection
                (info.mConnection, info.mGO, info.mController))
            {
               GameManager.GUI.RpcAddRemotePlayer
                    (info.mGO.GetComponent<NetworkIdentity>().netId);
            }
        }

        #endregion
    }
}
