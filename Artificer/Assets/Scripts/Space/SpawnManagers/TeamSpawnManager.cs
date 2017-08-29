using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Data.Space;
using Space.Segment;
using Stations;
using Game;
using System.Collections;
using Space.AI;
using System.Linq;
using Networking;

namespace Space.Spawn
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
    public class TeamSpawnManager: SpawnManager
    {
        #region ATTRIBUTES

        private IndexedList<SpawnPointInformation> m_spawns;

        /// <summary>
        /// How close the player can be 
        /// before spawning ai
        /// </summary>
        [SerializeField]
        private float m_range;

        /// <summary>
        /// Synced list across network
        /// storing agent groups
        /// </summary>
        public SyncListAgentGroup Groups = new SyncListAgentGroup();

        [SyncVar]
        public int FortifyLevel = 0;

        #endregion

        #region MONOBEHAVIOUR

        private void OnEnable()
        {
            StartCoroutine("PlayerDistance");
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void Awake()
        {
            m_spawns = new IndexedList
                <SpawnPointInformation>();
        }

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

            // Retrieve behaviour for making changes
            StationAccessor stationAtt =
                newStation.GetComponent<StationAccessor>();

            // add segment object to segment data
            SegmentObjectData segObj = new SegmentObjectData();

            // assign segment data
            segObj.netID = stationCon.netId;
            segObj._visibleDistance = 300f;

            // init object SO
            stationCon.Create(segObj);

            // send to segment
            SystemManager.Space.SegObj.ImportSegmentObject(segObj);

            // if station is a type that we spawn then add to spawn list
            if (stationAtt.Type != STATIONTYPE.HOME)
            {
                // Set the station to non- spawning and return
                stationAtt.SpawnID = -1;
                return newStation;
            }

            // add to spawn if correct type
            if (m_spawns == null)
                m_spawns = new IndexedList<SpawnPointInformation>();

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

            m_spawns.Add(sPInfo);

            // Log our spawn info
            stationAtt.SpawnID = sPInfo.ID;

            Groups.Add(new AgentGroup(stationCon.netId.Value));

            return newStation;
        }

        /// <summary>
        /// Spawns player at random spawnpoint for first time
        /// </summary>
        /// <param name="info"></param>
        [Server]
        public GameObject SpawnPlayer(PlayerConnectionInfo info, int spawnID)
        {
            SpawnPointInformation toSpawn = m_spawns.Item(spawnID);

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

        /// <summary>
        /// Interates through each group and returns
        /// true if we find the destroyed ship
        /// </summary>
        /// <param name="DD"></param>
        /// <returns></returns>
        [Server]
        public bool ProcessDestroyed(DestroyDespatch DD)
        {
            foreach (AgentGroup group in Groups)
                if (group.ShipDestroyed(DD))
                    return true;

            return false;
        }

        #endregion

        #region PRIVATE UTILITIES

        [Server]
        protected override void AssignAgent(uint agentID, uint targetID)
        {
            int index = Groups.IndexOf
                (Groups.FirstOrDefault(x => x.m_focus == targetID));

            if (index != -1)
            {
                AgentGroup AG = Groups[index];
                AG.AddAgent(agentID);
                Groups[index] = AG;
            }
        }

        #endregion

        #region COROUTINES

        /// <summary>
        /// Detect the distance between the players ship 
        /// and this team object
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlayerDistance()
        {
            //Heart beat atts
            IEnumerator heartbeat = TeamHeartbeat();
            bool inRange = false;

            while (true)
            { 
                // retrieve player bject is spawned
                GameObject playerShip = GameObject.FindGameObjectWithTag("PlayerShip");      

                if(playerShip == null)
                {
                    yield return null;
                    continue;
                }

                if(Vector3.Distance(playerShip.transform.position, 
                    transform.position) < m_range)
                {
                    // Detect when we are able to add authority
                    if(!inRange)
                    {
                        StartCoroutine(heartbeat);
                        inRange = true;
                    }
                }
                else
                {
                    // stop any coroutines that are running
                    StopCoroutine("heartbeat");
                    inRange = false;
                }
                yield return null;
            }
        }

        /// <summary>
        /// If we have authority then run
        /// the heartbeat that creates agents
        /// </summary>
        /// <returns></returns>
        private IEnumerator TeamHeartbeat()
        {
            while (true)
            {
                // set the timer before repeating 
                yield return new WaitForSeconds
                    (UnityEngine.Random.Range(10, 30));

                // Loop through each ship tracking item
                int i = 0;
                while (i < Groups.Count)
                {
                    AgentGroup target = Groups[i];

                    GameObject Focus = target.Focus;

                    if (Focus == null)
                    {
                        // destroyed stop tracking
                        Groups.RemoveAt(i);
                        continue;
                    }
                    else
                        i++;

                    // Only handle spawn if ship is currently active
                    if (Focus.GetComponent<StationAccessor>().Active)
                    {
                        // determine spawn size on threat level 
                        int shipCount = FortifyLevel + 1;

                        // add a ship we have no reached cap
                        if (target.AgentCount < shipCount)
                        {
                            SpawnNPCMessage npcMsg = new SpawnNPCMessage();
                            npcMsg.AgentType = RandomAgent(FortifyLevel);
                            npcMsg.Location = Math.RandomWithinRange
                                (Focus.transform.position, 10, 25);
                            npcMsg.SelfID = SystemManager.Space.ID;
                            npcMsg.SpawnID = netId.Value;
                            npcMsg.TargetID = target.m_focus;

                            NetworkManager.singleton.client.RegisterHandler((short)MSGCHANNEL.PROCESSNPC, BuildAgentListener);

                            SystemManager.singleton.client.Send
                                ((short)MSGCHANNEL.SPAWNNPC, npcMsg);
                        }
                    }

                    yield return null;
                }
            }
        }

        #endregion
    }
}
