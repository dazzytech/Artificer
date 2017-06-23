using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Data.Shared;
using Data.Space;
using Space.Segment;
using Stations;
using Game;

namespace Space.Teams.SpawnManagers
{
    #region SPAWN POINT

    /// <summary>
    /// Single container for a list of spawns
    /// </summary>
    public class SpawnPointInformation: IndexedObject
    {
        public List<Vector2> Spawns;

        // Accessor for the origin point
        public Transform SpawnOrigin;

        public SpawnPointInformation()
        {
            Spawns = new List<Vector2>();
        }
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

        // temp list of spawn points (stations)
        private IndexedList<SpawnPointInformation> _spawns;

        #endregion

        #region PUBLIC INTERACTION

        [Server]
        public GameObject AddStation(Vector2 station, 
            string StationPrefab = "Placeholder_Station")
        {
            GameObject newStation = Instantiate((Resources.Load("Space/Stations/" + StationPrefab) as GameObject),
                station, Quaternion.identity) as GameObject;

            NetworkServer.Spawn(newStation);

            // Retrieve behaviour for making changes
            StationController stationCon =
                newStation.GetComponent<StationController>();

            // init with team object
            stationCon.Initialize(netId);
            
            // if station is a type that we spawn then add to spawn list
            if(stationCon.Type != STATIONTYPE.HOME)
            {
                // Set the station to non- spawning and return
                stationCon.SpawnID = -1;
                return newStation;
            }

            // add to spawn if correct type
            if (_spawns == null)
                _spawns = new IndexedList<SpawnPointInformation>();

            // Spawn point information initialisation
            SpawnPointInformation sPInfo = new SpawnPointInformation();

            //generate the five spawns near the middle
            for (int i = 0; i < 5; i++)
            {
                // track if too close to another point
                bool tooClose = true;

                // how close is too close
                float minDistance = 5;

                // keep counter to avoid too many loops
                int loops = 0;

                // create vector around center
                Vector2 pos = new Vector2
                    (Random.Range((station.x - 5), (station.x + 5)),
                    Random.Range((station.y - 5), (station.y + 5)));

                while (tooClose)
                {
                    tooClose = false;

                    // go through each point previously added
                    foreach (Vector2 prev in sPInfo.Spawns)
                    {
                        // make sure this has actually been assigned
                        if (prev != Vector2.zero)
                        {
                            // check distance, but alsojust accept if we have checked 10 times
                            if (Vector2.Distance(pos, prev) < minDistance && loops < 10)
                            {
                                // we are too close
                                tooClose = true;
                                pos = new Vector2(Random.Range((station.x - 5), (station.x + 5)),
                                    Random.Range((station.y - 5), (station.y + 5)));
                                break;
                            }
                        }
                    }

                    // Iterate counter
                    loops++;
                }

                // Assign to our spawn list
                sPInfo.Spawns.Add(pos);
            }

            _spawns.Add(sPInfo);

            // Log our spawn info
            stationCon.SpawnID = sPInfo.ID;

            return newStation;
        }

        #endregion

        #region PLAYER SPAWNING

        /// <summary>
        /// Spawns player at random spawnpoint for first time
        /// </summary>
        /// <param name="info"></param>
        [Server]
        public GameObject SpawnPlayer(PlayerConnectionInfo info, int spawnID)
        {
            SpawnPointInformation toSpawn = (SpawnPointInformation)_spawns.Item(spawnID);

            // used to track if the immediate vicinity for spawning is clear
            bool areaClear = false;

            // Space in Units in a radius we want to be clear
            float minDistance = 2;

            // take position from spawn point we want
            Vector2 newPosition = toSpawn.Spawns[Random.Range(0, toSpawn.Spawns.Count)];

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

            GameObject playerObject = Instantiate(SystemManager.singleton.playerPrefab);
            // apply position
            playerObject.transform.position = newPosition;

            NetworkServer.ReplacePlayerForConnection
                 (info.mConnection, playerObject, info.mController);

            return playerObject;
        }



        #endregion
    }
}
