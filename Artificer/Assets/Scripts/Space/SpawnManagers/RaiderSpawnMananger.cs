using Data.Space;
using Game;
using Networking;
using Space.AI;
using Space.AI.Agent;
using Space.Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace Space.SpawnManager
{
    /// <summary>
    /// Tracks all possible raid targets and 
    /// every heartbeat will have a random
    /// raid party spawn
    /// </summary>
    public class RaiderSpawnMananger : NetworkBehaviour
    {
        #region TARGET TRACKER

        /// <summary>
        /// Logs information about ship
        /// that is being tracked
        /// </summary>
        public class RaiderTarget
        {
            #region ATTRIBUTES

            /// <summary>
            /// The suitable target 
            /// for raider ships to spawn on
            /// </summary>
            private Transform m_target;

            /// <summary>
            /// Reference to all the raiders
            /// attacking this target
            /// </summary>
            private List<NetworkInstanceId> m_raiders;

            #endregion

            /// <summary>
            /// Initializes the object with 
            /// reference to the ship
            /// </summary>
            /// <param name="target"></param>
            public RaiderTarget(Transform target)
            {
                m_target = target;
                m_raiders = new List<NetworkInstanceId>();

                SystemManager.Events.EventShipDestroyed
                    += ShipDestroyedEvent;
            }

            #region PUBLIC INTERACTION

            /// <summary>
            /// Adds a reference to a ship
            /// pursuing this one
            /// </summary>
            /// <param name="ship"></param>
            public void AddPersuit(NetworkInstanceId ship)
            {
                if (ship != null)
                    m_raiders.Add
                        (ship);
            }

            #endregion

            #region PRIVATE UTILITIES

            /// <summary>
            /// Returns a value based on 
            /// distance from station and 
            /// number of friendlies
            /// </summary>
            private int DetectThreat(int startThreat = 10)
            {
                // decrement depending on distance to
                // statons
                if (!WithinRangeStation(400))
                {
                    startThreat -= 4;

                    if (!WithinRangeStation(1000))
                    {
                        startThreat -= 4;
                        if (!WithinRangeStation(2000))
                            startThreat -= 2;
                    }
                }

                // now increment value depending on friendly ships
                // maxing at 3
                startThreat += ShipsWithinRange();

                // 10 is max we can have
                return Mathf.Min(startThreat, 10);
            }

            /// <summary>
            /// Returns true is any stations are within
            /// the defined circle radiusofthe ship
            /// </summary>
            /// <param name="distance"></param>
            /// <returns></returns>
            private bool WithinRangeStation(float distance)
            {
                RaycastHit2D[] withinRange;

                withinRange = Physics2D.CircleCastAll
                    (Self.position, distance, Vector2.zero);

                foreach (RaycastHit2D hit in withinRange)
                {
                    if (hit.transform.tag == "Station")
                        return true;
                }

                return false;
            }

            /// <summary>
            /// Increment return by one for every 
            /// ship within 5 km of the ship
            /// max return = 10
            /// </summary>
            /// <returns></returns>
            private int ShipsWithinRange()
            {
                int ships = 0;

                RaycastHit2D[] withinRange;

                withinRange = Physics2D.CircleCastAll
                    (Self.position, 50, Vector2.zero);

                foreach (RaycastHit2D hit in withinRange)
                {
                    if (hit.transform.tag == "Friendly"
                        || hit.transform.tag == "Enemy")
                        ships += ships == 3 ? 0 : 1;
                }

                return ships;
            }

            #endregion

            #region EVENTS

            /// <summary>
            /// Deletes reference to the persuit
            /// ship if it is destroyed
            /// </summary>
            /// <param name="DD"></param>
            private void ShipDestroyedEvent(DestroyDespatch DD)
            {
                for (int i = 0; i < m_raiders.Count; i++)
                {
                    if (m_raiders[i] == DD.Self)
                    {
                        m_raiders.RemoveAt(i);
                        i--;
                    }
                }

            }

            #endregion

            #region ACCESSORS

            /// <summary>
            /// calls func to detect threat level
            /// </summary>
            public int ThreatLevel
            {
                get
                {
                    return DetectThreat();
                }
            }

            /// <summary>
            /// Returns the amount of raiders targeting
            /// </summary>
            public int PersuitCount
            {
                get
                {
                    return m_raiders.Count;
                }
            }

            /// <summary>
            /// Stores an accessible reference 
            /// to self
            /// </summary>
            public Transform Self
            {
                get { return m_target; }
            }

            /// <summary>
            /// network instance if of the tracked
            /// object
            /// </summary>
            public NetworkInstanceId SelfNetID
            {
                get { return Self.GetComponent<NetworkIdentity>().netId; }
            }

            #endregion
        }

        #endregion

        #region ATTRIBUTES

        [SerializeField]
        private AIManager m_AI;

        /// <summary>
        /// Targets for raiding parties, tracks target
        /// and threat level and raiding ships
        /// </summary>
        private List<RaiderTarget> m_raidTargets;

        #endregion

        #region MONO BEHAVIOUR

        private void Start()
        {
            m_raidTargets = new List<RaiderTarget>();

            StartCoroutine("SeekTargets");

            StartCoroutine("HeartbeatRaiderSpawn");
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Builds the ship item and then 
        /// calls the command to network spawn the object
        /// and to send the spawn me message
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private NetworkInstanceId BuildAgent(Transform target)
        {
            // Build an instance of the agent
            GameObject newAgent =
                Instantiate(SystemManager.singleton.playerPrefab);

            // Find the location the agent will spawn at
            Vector2 spawnPoint = Math.RandomWithinRange
                (target.position, 100, 200);

            newAgent.transform.position = spawnPoint;

            // Retrieve the agent info
            AgentData agent = m_AI.AgentLibrary["raider_small"];

            CmdSpawnShip(newAgent, agent.Ship);

            AssaultAgent agentFSM = newAgent.AddComponent<AssaultAgent>();

            // Apply variables
            agentFSM.EstablishRanges
                (Convert.ToInt32(agent.EngageDistance),
                 Convert.ToInt32(agent.PursuitDistance),
                 Convert.ToInt32(agent.AttackDistance),
                 Convert.ToInt32(agent.PullOffDistance));

            agentFSM.DefineTarget(target);

            // Return the network ID of our new raider
            return newAgent.GetComponent<NetworkIdentity>().netId;
        }

        /// <summary>
        /// Spawns the object with this player's
        /// authority and sets the spawn process message
        /// </summary>
        [Command]
        private void CmdSpawnShip(GameObject agent,
            string ship)
        {
            // Spawn the ship on the network
            NetworkServer.SpawnWithClientAuthority
                (agent, SystemManager.Space.PlayerConn);

            // Assign the information while on the server
            agent.GetComponent<ShipInitializer>().
                AssignShipData(ship, -1);
        }

        #endregion

        #region COROUTINE

        /// <summary>
        /// Searches through each ship 
        /// that we have authority over 
        /// (runs on each client and not the server)
        /// and add that ship information to the 
        /// our tracker list
        /// /// </summary>
        /// <returns></returns>
        private IEnumerator SeekTargets()
        {
            // Retrieve the container
            Transform shipContainer = null;

            while (true)
            {
                if (shipContainer == null)
                {
                    GameObject GO = GameObject.Find("_ships");

                    if (GO != null)
                        shipContainer = GO.transform;
                    else
                    {
                        yield return null;
                        continue;
                    }
                }

                if (shipContainer.childCount == 0)
                {
                    yield return null;
                    continue;
                }

                // Iterate through each ship to check if
                // its appropriate for us to monitor
                foreach (Transform ship in shipContainer.transform)
                {
                    // change in future so can target other ai ships
                    if (ship.GetComponent<NetworkIdentity>
                        ().isLocalPlayer)
                    {
                        if (ship.GetComponent<ShipAttributes>().TeamID != -1)
                        {
                            // If this is a ship that this client
                            // manages then we want to track it
                            RaiderTarget state = m_raidTargets.
                                FirstOrDefault(x => x.Self == ship);

                            // Check if we are already tracking this ship
                            if (state == null)
                            {
                                // We can create a tracker for this ship
                                m_raidTargets.Add(new RaiderTarget(ship));
                            }
                        }
                    }

                    yield return null;
                }

                yield return null;
            }
        }

        /// <summary>
        /// After an alloted time
        /// perform client side e.g. 
        /// Ship tracking
        /// </summary>
        /// <returns></returns>
        private IEnumerator HeartbeatRaiderSpawn()
        {
            while (true)
            {
                // set the timer before repeating 
                yield return new WaitForSeconds
                    (UnityEngine.Random.Range(10, 30));

                // Loop through each ship tracking item

                int i = 0;
                while (i < m_raidTargets.Count)
                {
                    RaiderTarget target = m_raidTargets[i];

                    if (m_raidTargets[i].Self == null)
                    {
                        // Ship is destroyed stop tracking
                        m_raidTargets.RemoveAt(i);
                        continue;
                    }
                    else
                        i++;

                    int level = target.ThreatLevel;

                    // Skip random for now

                    // 5% - 50% chance of spawning
                    if (UnityEngine.Random.Range(-10, 10) >= level 
                        && SystemManager.Space.PlayerConn != null
                        /*Add additional check for if currently pursued*/)
                    {
                        // we are able to spawn a ship

                        // determine spawn size on threat level 
                        int shipCount = level < 4 ? 3 : level < 8 ? 2 : 1;

                        // add a ship we have no reached cap
                        if (target.PersuitCount < shipCount)
                        {
                            // We have the space
                            // Spawn the ship
                            target.AddPersuit(BuildAgent(target.Self));
                        }
                    }

                    yield return null;
                }
            }
        }

        #endregion
    }
}
