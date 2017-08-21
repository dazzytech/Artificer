using Data.Space;
using Data.Space.Library;
using Game;
using Networking;
using Space.AI;
using Space.AI.Agent;
using Space.Ship;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace Space.Spawn
{
    /// <summary>
    /// Tracks all possible raid targets and 
    /// every heartbeat will have a random
    /// raid party spawn
    /// </summary>
    public class RaiderSpawnManager : SpawnManager
    {
        #region INLINE CLASS

        /// <summary>
        /// Logs information about ship
        /// that is being tracked
        /// </summary>
        public class RaiderTargetTracker: AgentGroupTracker
        {
            public RaiderTargetTracker(Transform tracked): base(tracked) { }

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
                if (!WithinRangeStation(500))
                {
                    startThreat -= 2;

                    if (!WithinRangeStation(1000))
                    {
                        startThreat -= 2;

                        if (!WithinRangeStation(1500))
                        {
                            startThreat -= 2;
                            if (!WithinRangeStation(2000))
                            {
                                startThreat -= 2;
                                if (!WithinRangeStation(2500))
                                    startThreat -= 2;
                            }
                        }
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
                    (Focus.position, distance, Vector2.zero);

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
                    (Focus.position, 50, Vector2.zero);

                foreach (RaycastHit2D hit in withinRange)
                {
                    if (hit.transform.tag == "Friendly"
                        || hit.transform.tag == "Enemy")
                        ships += ships == 3 ? 0 : 1;
                }

                return ships;
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

            #endregion
        }

        #endregion

        #region ATTRIBUTES

        /// <summary>
        /// Targets for raiding parties, tracks target
        /// and threat level and raiding ships
        /// </summary>
        private List<RaiderTargetTracker> m_raidTargets;

        #endregion

        #region MONO BEHAVIOUR

        private void Start()
        {
            m_raidTargets = new List<RaiderTargetTracker>();

            StartCoroutine("SeekTargets");

            StartCoroutine("HeartbeatRaiderSpawn");
        }

        #endregion

        #region PRIVATE UTILITIES

        protected override void BuildAgent(uint selfID, uint agentID, uint targetID, string agent)
        {
            base.BuildAgent(selfID, agentID, targetID, agent);

            // Access the target given to us
            GameObject target = ClientScene.FindLocalObject
                (new NetworkInstanceId(targetID));

            if(target != null)
                AddToTargetList(target.transform, new NetworkInstanceId(agentID));
        }

        /// <summary>
        /// Discover if we are tracking a transform
        /// and begin track if not
        /// </summary>
        /// <param name="target"></param>
        /// <param name="agent"></param>
        private void AddToTargetList(Transform target, 
            NetworkInstanceId agent)
        {
            // Find the raider target stored on our 
            // current target
            RaiderTargetTracker state = m_raidTargets.
                FirstOrDefault(x => x.Focus == target);

            if(state == null)
            {
                // we shouldn't reach here
                Debug.Log("Error: Raider Spawn Manager - Add Targets List:"
                    + "Unable to find the right Raider Target.");
                return;
            }

            // Add reference to our new agent
            state.AddAgent(agent);
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
                        if (ship.GetComponent<ShipAccessor>().TeamID != -1)
                        {
                            // If this is a ship that this client
                            // manages then we want to track it
                            RaiderTargetTracker state = m_raidTargets.
                                FirstOrDefault(x => x.Focus == ship);

                            // Check if we are already tracking this ship
                            if (state == null)
                            {
                                // We can create a tracker for this ship
                                m_raidTargets.Add(new RaiderTargetTracker(ship));
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
                    RaiderTargetTracker target = m_raidTargets[i];

                    if (m_raidTargets[i].Focus == null)
                    {
                        // Ship is destroyed stop tracking
                        m_raidTargets.RemoveAt(i);
                        continue;
                    }
                    else
                        i++;

                    int level = target.ThreatLevel;

                    // 5% - 50% chance of spawning
                    if (UnityEngine.Random.Range(-10, 10) >= level
                       && SystemManager.Space.NetID != 0)
                    {
                        // determine spawn size on threat level 
                        int shipCount = level < 4 ? 3 : level < 8 ? 2 : 1;

                        // add a ship we have no reached cap
                        if (target.AgentCount < shipCount)
                        {
                            // postion within range of target
                            Vector3 location = Math.RandomWithinRange
                                (target.Focus.position, 100, 200);

                            // Here we will build the message 
                            // to create a raider
                            // add agent based on threat level
                            CmdSpawnShip(RandomAgent(10 - level), 
                                SystemManager.Space.NetID, location, 
                                target.FocusNetID.Value);
                        }
                    }

                    yield return null;
                }
            }
        }

        #endregion
    }
}
