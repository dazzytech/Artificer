using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Data.Shared;
using Data.Space;
using Space.Segment;
using Space.GameFunctions;

namespace Space.Teams.SpawnManagers
{
    public class SpawnPointInformation
    {
        public int ID;
        public Vector2[] Spawns;
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

        // temp list of spawn points (stations)
        private List<SpawnPointInformation> _spawns;

        #endregion

        #region PUBLIC INTERACTION

        [Server]
        public GameObject AddStation(Vector2 station, string StationPrefab = "Placeholder_Station")
        {
            GameObject newStation = Instantiate((Resources.Load("Space/Stations/" + StationPrefab) as GameObject),
                station, Quaternion.identity) as GameObject;

            NetworkServer.Spawn(newStation);

            Vector2[] spawns = new Vector2[5];

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
                    foreach (Vector2 prev in spawns)
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
                spawns[i] = pos;
            }

            if (_spawns == null)
                _spawns = new List<SpawnPointInformation>();

            // Spawn point information initialisation
            SpawnPointInformation sPInfo = new SpawnPointInformation();
            sPInfo.ID = _spawns.Count;
            sPInfo.Spawns = spawns;

            // Pass ID to station for later access
            newStation.GetComponent<StationController>().Initialize
                (sPInfo.ID, GetComponent<TeamController>());

            _spawns.Add(sPInfo);

            return newStation;
        }

        #endregion

        // Maybe combine regions?
        #region PLAYER SPAWNING

        /// <summary>
        /// Spawns player at random spawnpoint for first time
        /// </summary>
        /// <param name="info"></param>
        [Server]
        public GameObject SpawnPlayer(PlayerConnectionInfo info, int spawnID)
        {
            SpawnPointInformation toSpawn = GetSPInfo(spawnID);

            // used to track if the immediate vicinity for spawning is clear
            bool areaClear = false;

            // Space in Units in a radius we want to be clear
            float minDistance = 2;

            // take position from spawn point we want
            Vector2 newPosition = toSpawn.Spawns[Random.Range(0, toSpawn.Spawns.Length)];

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

            NetworkServer.ReplacePlayerForConnection
                 (info.mConnection, playerObject, info.mController);

            return playerObject;
        }

        #endregion

        #region INTERNAL UTILITES

        /// <summary>
        /// returns correct spawn info or raises
        /// error if invalid id
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private SpawnPointInformation GetSPInfo(int index)
        {
            foreach(SpawnPointInformation spawn in _spawns)
            { 
                if(spawn.ID == index)
                    return spawn;
            }

            Debug.Log("Error: Team Spawn Manager - GetSPInfo: Spawn index not found!");
            return null;
        }

        #endregion
    }
}
