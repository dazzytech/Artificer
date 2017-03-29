using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;
using Data.Space;
using Space.Segment.Generator;
using Space.Teams;

namespace Game
{
    /// <summary>
    /// Build game specific space objects e.g.
    /// Starter bases and team spawns
    /// </summary>
    public class GameBuilder: NetworkBehaviour
    {
        #region ATTRIBUTES 

        // Generator Objects
        //public ShipGenerator _shipGen;
       // public AsteroidGenerator _asteroidGen;
        //public SpawnPointGenerator _spawnGen;
        //public NodeGenerator _nodeGen;

        #endregion

        // todo: public imports game parameters and then
        // GameBuilder will just create the game objects

        #region PUBLIC INTERACTION
        
        /// <summary>
        /// For now, builds a group of player spawners in close 
        /// proximity and sends this to the team spawner
        /// </summary>
        [Server]
        public void GenerateStations(TeamController teamA, TeamController teamB)
        {
            // place team A at the top half of the map
            Vector2 teamAPos = new Vector2(2500, 2400);

            // Place team B at the other half
            Vector2 teamBPos = new Vector2(2500, 2500);

            // Create a main station position within team As location
            Vector2 stationAPosition = new Vector2
                (Random.Range((teamAPos.x - 50), (teamAPos.x + 50)),
                 Random.Range((teamAPos.y - 50), (teamAPos.y + 50)));

            // Create a main station position within team Bs location
            Vector2 stationBPosition = new Vector2
                (Random.Range((teamBPos.x - 50), (teamBPos.x + 50)),
                 Random.Range((teamBPos.y - 50), (teamBPos.y + 50)));

            // Todo: Develop more parameters for station construction
            // e.g. prefab name

            // for now only send one vector position
            GameObject stationA = teamA.Spawner.AddStation(stationAPosition);
            teamA.AddStationObject(stationA.GetComponent<NetworkIdentity>().netId);

            GameObject stationB = teamB.Spawner.AddStation(stationBPosition);
            teamB.AddStationObject(stationB.GetComponent<NetworkIdentity>().netId);
        }

        [Server]
        public void GenerateStation(TeamController selectedTeam,
            string prefabName, Vector2 position)
        {
            GameObject station = selectedTeam.Spawner.AddStation(position, prefabName);
            selectedTeam.AddStationObject(station.GetComponent<NetworkIdentity>().netId);
        }

        #endregion

        #region INTERNAL BUILDING

        /// <summary>
        /// Builds a spawn point and returns the created spawn
        /// </summary>
        /// <param name="type"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private SpawnPointData BuildSpawn(string type, Vector2 position)
        {
            SpawnPointData spawn = new SpawnPointData();
            spawn.Position = position;
            spawn.SpawnType = type;

            return spawn;
        }

        #endregion

        /*// Use this for initialization
        public void InitializeSpawners()
        {
            // Spawner attributes
            //PlayerSpawner.Player = param.Player;
            /*PlayerSpawner.ShipGen = _shipGen;
            
            // Generate asteroid data
            AsteroidData aData = new AsteroidData();
            aData.SetProspect("low"); // always low for now
            aData.Type = "asteroid";
            AsteroidSpawner.Asteroid = aData;
            AsteroidSpawner.AstGen = _asteroidGen;
            
            FriendlySpawner.ShipGen = _shipGen;
            EnemySpawner.ShipGen = _shipGen;
        }

    /*
    public void GenerateContractObjects(ContractData contract)
    {
        List<SpawnPointData> spawns = new List<SpawnPointData>();

        // Design segment based on contract
        switch (contract.CType)
        {
            case ContractType.BaseDefense:
                {
                    Vector2 missionStation = 
                    BuildStation(new Vector2(2500f, 2500f), contract).transform.position;

                    // Build Player Spawns around the station
                    int pSP = Random.Range(2, 5);
                    for (int i = 0; i <= pSP; i++)
                    {
                        spawns.Add(BuildSpawn("playerSpawn", new Vector2
                                          (Random.Range(missionStation.x - 50f, missionStation.x + 50f),
                                            Random.Range(missionStation.y - 50f, missionStation.y + 50f))));
                    }

                    // Build asteroid spawners far from the station
                    AsteroidSpawner.Target = missionStation;

                    int aSP = Random.Range(5, 8);
                    for (int i = 0; i <= aSP; i++)
                    {
                        bool outOfRange = false;
                        Vector2 pos = new Vector2();
                        while (!outOfRange)
                        {
                            pos = new Vector2
                            (Random.Range(missionStation.x - 200f, missionStation.x + 200f),
                             Random.Range(missionStation.y - 200f, missionStation.y + 200f));

                            if (Vector2.Distance(pos, missionStation) >= 100f && Vector2.Distance(pos, missionStation) < 200f)
                            {
                                outOfRange = true;
                            }
                        }

                        spawns.Add(BuildSpawn("asteroidSpawn", pos));
                    }

                    // Create friendly spawn points
                    // Build Player Spawns around the station
                    int fSP = Random.Range(2, 5);
                    for (int i = 0; i <= fSP; i++)
                    {
                        spawns.Add(BuildSpawn("friendlySpawn", new Vector2
                                   (Random.Range(missionStation.x - 50f, missionStation.x + 50f),
                                    Random.Range(missionStation.y - 50f, missionStation.y + 50f))));
                    }

                    int eSP = Random.Range(4, 8);
                    for (int i = 0; i <= eSP; i++)
                    {

                        bool outOfRange = false;
                        Vector2 pos = new Vector2();
                        while (!outOfRange)
                        {
                            pos = new Vector2
                            (Random.Range(missionStation.x - 150f, missionStation.x + 150f),
                             Random.Range(missionStation.y - 150f, missionStation.y + 150f));

                            if (Vector2.Distance(pos, missionStation) >= 75f && Vector2.Distance(pos, missionStation) < 150f)
                            {
                                outOfRange = true;
                            }
                        }
                        spawns.Add(BuildSpawn("enemySpawn", pos));
                    }

                    break;
                }
            case ContractType.Attrition:
                {
                    // Calculate two spawn points in the middle of segment
                    // First point 
                    Vector2 FriendlySpawn = new Vector2(2500, 2500);

                    Vector2 EnemySpawn = Vector2.zero;

                    bool outOfRange = false;
                    while (!outOfRange)
                    {
                        EnemySpawn = new Vector2
                        (Random.Range(FriendlySpawn.x - 100f, FriendlySpawn.x + 100f),
                         Random.Range(FriendlySpawn.y - 100f, FriendlySpawn.y + 100f));

                        if (Vector2.Distance(FriendlySpawn, EnemySpawn) >= 50f && Vector2.Distance(EnemySpawn, FriendlySpawn) < 100f)
                        {
                            outOfRange = true;
                        }
                    }

                    // Build Player Spawns around the station
                    int pSP = Random.Range(2, 5);
                    for (int i = 0; i <= pSP; i++)
                    {
                        spawns.Add(BuildSpawn("playerSpawn", new Vector2
                                          (Random.Range(FriendlySpawn.x - 50f, FriendlySpawn.x + 50f),
                     Random.Range(FriendlySpawn.y - 50f, FriendlySpawn.y + 50f))));
                    }

                    // Create friendly spawn points
                    // Build Player Spawns around the station
                    int fSP = Random.Range(3, 6);
                    for (int i = 0; i <= fSP; i++)
                    {
                        spawns.Add(BuildSpawn("friendlySpawn", new Vector2
                                          (Random.Range(FriendlySpawn.x - 50f, FriendlySpawn.x + 50f),
                     Random.Range(FriendlySpawn.y - 50f, FriendlySpawn.y + 50f))));
                    }

                    int eSP = Random.Range(4, 8);
                    for (int i = 0; i <= eSP; i++)
                    {

                        spawns.Add(BuildSpawn("enemySpawn", new Vector2
                                          (Random.Range(EnemySpawn.x - 50f, EnemySpawn.x + 50f),
                        Random.Range(EnemySpawn.y - 50f, EnemySpawn.y + 50f))));
                    }

                    // Build asteroid spawners far from player
                    int aSP = Random.Range(5, 8);
                    for (int i = 0; i <= aSP; i++)
                    {
                        outOfRange = false;
                        Vector2 pos = new Vector2();
                        while (!outOfRange)
                        {
                            pos = new Vector2
                            (Random.Range(FriendlySpawn.x - 200f, FriendlySpawn.x + 200f),
                             Random.Range(FriendlySpawn.y - 200f, FriendlySpawn.y + 200f));

                        if (Vector2.Distance(pos, FriendlySpawn) >= 100f && Vector2.Distance(pos, FriendlySpawn) < 200f)
                            {
                                outOfRange = true;
                            }
                        }

                        spawns.Add(BuildSpawn("asteroidSpawn", pos));
                    }
                    break;
                }

            case ContractType.Targets:
                {
                    // Calculate two spawn points in the middle of segment
                    // First point 
                    Vector2 FriendlySpawn = new Vector2(2500, 2500);

                    Vector2 EnemySpawn = Vector2.zero;

                    bool outOfRange = false;
                    while (!outOfRange)
                    {
                        EnemySpawn = new Vector2
                            (Random.Range(FriendlySpawn.x - 100f, FriendlySpawn.x + 100f),
                             Random.Range(FriendlySpawn.y - 200f, FriendlySpawn.y + 100f));

                        if (Vector2.Distance(FriendlySpawn, EnemySpawn) >= 50f && Vector2.Distance(EnemySpawn, FriendlySpawn) < 100f)
                        {
                            outOfRange = true;
                        }
                    }

                    // Build Player Spawns around the station
                    int pSP = Random.Range(2, 5);
                    for (int i = 0; i <= pSP; i++)
                    {
                        spawns.Add(BuildSpawn("playerSpawn", new Vector2
                                              (Random.Range(FriendlySpawn.x - 50f, FriendlySpawn.x + 50f),
                         Random.Range(FriendlySpawn.y - 50f, FriendlySpawn.y + 50f))));
                    }

                    // Create friendly spawn points
                    // Build Player Spawns around the station
                    int fSP = Random.Range(3, 6);
                    for (int i = 0; i <= fSP; i++)
                    {
                        spawns.Add(BuildSpawn("friendlySpawn", new Vector2
                                              (Random.Range(FriendlySpawn.x - 50f, FriendlySpawn.x + 50f),
                         Random.Range(FriendlySpawn.y - 50f, FriendlySpawn.y + 50f))));
                    }

                    int eSP = Random.Range(4, 8);
                    for (int i = 0; i <= eSP; i++)
                    {

                        spawns.Add(BuildSpawn("enemySpawn", new Vector2
                                              (Random.Range(EnemySpawn.x - 50f, EnemySpawn.x + 50f),
                         Random.Range(EnemySpawn.y - 50f, EnemySpawn.y + 50f))));
                    }

                    int aSP = Random.Range(5, 8);
                    for (int i = 0; i <= aSP; i++)
                    {
                        outOfRange = false;
                        Vector2 pos = new Vector2();
                        while (!outOfRange)
                        {
                            pos = new Vector2
                                (Random.Range(FriendlySpawn.x - 200f, FriendlySpawn.x + 200f),
                                 Random.Range(FriendlySpawn.y - 200f, FriendlySpawn.y + 200f));

                            if (Vector2.Distance(pos, FriendlySpawn) >= 100f && Vector2.Distance(pos, FriendlySpawn) < 200f)
                            {
                                outOfRange = true;
                            }
                        }

                        spawns.Add(BuildSpawn("asteroidSpawn", pos));
                    }
                    break;
                }
            case ContractType.Escort:
            {
               // Create two stations with 100KM distance between them
                Vector2 homeStation = new Vector2(2500f, 2500f);
                    //BuildStation(new Vector2(2500f, 2500f), contract, false).transform.position;

                Vector2 targetStation = new Vector2();
                bool outOfRange = false;
                while (!outOfRange)
                {
                    targetStation = new Vector2
                        (Random.Range(homeStation.x - 1000f, homeStation.x + 1000f),
                         Random.Range(homeStation.y - 1000f, homeStation.y + 1000f));

                    if (Vector2.Distance(homeStation, targetStation) >= 700f && Vector2.Distance(homeStation, targetStation) < 1000f)
                    {
                        outOfRange = true;
                    }
                }

                BuildStation(targetStation, contract);

                // Create enemy and player spawns 
                // Build Player Spawns around the station
                // Follow So spawner has follow script
                int pSP = Random.Range(2, 5);
                for (int i = 0; i <= pSP; i++)
                {
                    spawns.Add(BuildSpawn("playerSpawnFollow", new Vector2
                                          (Random.Range(homeStation.x - 50f, homeStation.x + 50f),
                     Random.Range(homeStation.y - 50f, homeStation.y + 50f))));
                }

                // Create friendly spawn points
                // Build Player Spawns around the station
                int fSP = Random.Range(2, 5);
                for (int i = 0; i <= fSP; i++)
                {
                    spawns.Add(BuildSpawn("friendlySpawnFollow", new Vector2
                                          (Random.Range(homeStation.x - 50f, homeStation.x + 50f),
                     Random.Range(homeStation.y - 50f, homeStation.y + 50f))));
                }

                int eSP = Random.Range(4, 8);
                for (int i = 0; i <= eSP; i++)
                {

                    outOfRange = false;
                    Vector2 pos = new Vector2();
                    while (!outOfRange)
                    {
                        pos = new Vector2
                            (Random.Range(homeStation.x - 200f, homeStation.x + 200f),
                             Random.Range(homeStation.y - 200f, homeStation.y + 200f));

                        if (Vector2.Distance(pos, homeStation) >= 100f && Vector2.Distance(pos, homeStation) < 200f)
                        {
                            outOfRange = true;
                        }
                    }
                    spawns.Add(BuildSpawn("enemySpawnFollow", pos));
                }
                int aSP = Random.Range(5, 8);
                for (int i = 0; i <= aSP; i++)
                {
                    outOfRange = false;
                    Vector2 pos = new Vector2();
                    while (!outOfRange)
                    {
                        pos = new Vector2
                            (Random.Range(homeStation.x - 200f, homeStation.x + 200f),
                             Random.Range(homeStation.y - 200f, homeStation.y + 200f));

                        if (Vector2.Distance(pos, homeStation) >= 100f && Vector2.Distance(pos, homeStation) < 200f)
                        {
                            outOfRange = true;
                        }
                    }

                    spawns.Add(BuildSpawn("asteroidSpawnFollow", pos));
                }
                break;


            }
        }

        BuildSpawners(spawns.ToArray());
    }

    private void BuildSpawners(SpawnPointData[] spawns)
    {
        // Build one time segments
        foreach(SpawnPointData spawn
                in spawns)
        {
            _spawnGen.GenerateSpawn(spawn);
        }
    }

    /// <summary>
    /// Builds the station.
    /// in future this will be 
    /// done in another class with defined vars
    /// </summary>
    /// <returns>The station.</returns>
    private GameObject BuildStation(Vector2 position, ContractData contract)
    {
        // Build Station in middle of segment
        // and set station as defense goal
        StationData station = new StationData();
        station.Position = position;

        station.SetAttributes("New Station", "Station_External_Small_01");
        station.Type = "station";

        GameObject statObj = _stationGen.GenerateStation(station);

        // Create nodes
        _nodeGen.GenerateNodes(5, "PathNode", 
                               station.Position.x-25f, station.Position.y-25f, 50f, 50f
                               );

        // Set enemy target
        switch(contract.CType)
        {
            case ContractType.BaseDefense:
            {
                // Implement target bool if multiple stations created
                EnemySpawnManager.Target = statObj.transform;

                foreach (MissionData mission in contract.PrimaryMissions)
                {
                    // in future do tests for right station
                    if(mission is DefendMission) ((DefendMission)mission).Station =
                        statObj.GetComponent<StationBehaviour>();
                }

                foreach (MissionData mission in contract.OptionalMissions)
                {
                    // in future do tests for right station
                    if(mission is DefendMission) ((DefendMission)mission).Station =
                        statObj.GetComponent<StationBehaviour>();
                }
                break;
            }
            case ContractType.Escort:
            {
                    FriendlySpawnManager.Travel = statObj.transform;
                statObj.GetComponent<StationBehaviour>().Enter = true;
                break;
            }
        }

        return statObj;
    }*/

    }
}

