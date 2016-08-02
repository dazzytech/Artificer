using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Data.Shared;
using Data.Space;
using Space.Segment.Generator;
using Space.GameFunctions;

namespace Space.Teams.SpawnManagers
{
    public struct SpawnPointInformation
    {

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
        #region ATTRIBUTES

        // stores the station generator to return stations
        //public StationGenerator stationGen;
        // builds the team spawn points
        public SpawnPointGenerator spawnGen;

        // temp list of spawn points
        private SpawnPointData[] _spawns;

        #endregion

        #region PUBLIC INTERACTION

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
        private void SpawnPlayerShip(/*PlayerTrackInfo info*/)
        {
            //NetworkServer.ReplacePlayerForConnection
               // (info.mConnection, info.mGO, info.mController);
        }

        /// <summary>
        /// Spawns player at random spawnpoint for first time
        /// </summary>
        /// <param name="info"></param>
        [Server]
        public GameObject SpawnPlayer(PlayerConnectionInfo info)
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

            GameObject playerObject = Instantiate(GameManager.singleton.playerPrefab);
            // apply position
            playerObject.transform.position = newPosition;

            // spawn player
            if (!info.mSpawned)
            {
                if (NetworkServer.AddPlayerForConnection
                    (info.mConnection, playerObject, info.mController))
                {
                    GameManager.GUI.RpcAddRemotePlayer
                         (playerObject.GetComponent<NetworkIdentity>().netId);
                }
            }
            else
            {
                if(NetworkServer.ReplacePlayerForConnection
                 (info.mConnection, playerObject, info.mController))
                {
                    GameManager.GUI.RpcAddRemotePlayer
                         (playerObject.GetComponent<NetworkIdentity>().netId);
                }
            }

            return playerObject;
        }

        #endregion
    }
}
