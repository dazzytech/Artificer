using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Space.Ship;
using Data.Space.DataImporter;
using Data.Space;
using UnityEngine.Networking.NetworkSystem;
using Networking;

namespace Space.AI
{
    /// <summary>
    /// Centralized system for managing ai states
    /// </summary>
    public class AIManager : NetworkBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private AIAttributes m_att;

        #endregion

        #region MONO BEHAVIOUR

        /// <summary>
        /// Begin heatbeats and 
        /// object seeking
        /// </summary>
        void Start()
        {
            StartCoroutine("SeekShips");

            StartCoroutine("HeartbeatClient");

            // Begin the process of importing agent data
            m_att.Agents = AgentDataImporter.BuildAgents();

            m_att.TrackedShips = new List<ShipState>();
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
        private NetworkInstanceId BuildAgent(Transform target, AgentData agent)
        {
            // Build an instance of the agent
            GameObject newAgent = 
                Instantiate(SystemManager.singleton.playerPrefab);
            
            // Find the location the agent will spawn at
            Vector2 spawnPoint = Math.RandomWithinRange
                (target.position, 100, 200);

            newAgent.transform.position = spawnPoint;

            // Here we would build and init the actual agent behaviour

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
            NetworkServer.SpawnWithClientAuthority
                (agent, connectionToClient);

            // assign ship info
            // e.g. ship name 
            StringMessage sMsg = new StringMessage(ship);
            NetworkServer.SendToClient(connectionToClient.connectionId,
                (short)MSGCHANNEL.SPAWNME, sMsg);
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
        private IEnumerator SeekShips()
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
            
                if(shipContainer.childCount == 0)
                {
                    yield return null;
                    continue;
                }

                // Iterate through each ship to check if
                // its appropriate for us to monitor
                foreach (Transform ship in shipContainer.transform)
                {
                    if (ship.GetComponent<NetworkIdentity>
                        ().hasAuthority)
                    {
                        if (ship.GetComponent<ShipAttributes>().TeamID != -1)
                        {
                            // If this is a ship that this client
                            // manages then we want to track it
                            ShipState state = m_att.TrackedShips.
                                FirstOrDefault(x => x.Self == ship);

                            // Check if we are already tracking this ship
                            if (state == null)
                            {
                                // We can create a tracker for this ship
                                m_att.TrackedShips.Add(new ShipState(ship));
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
        private IEnumerator HeartbeatClient()
        {
            while (true)
            {
                // set the timer before repeating 
                yield return new WaitForSeconds
                    (Random.Range(10, 30));

                // Loop through each ship tracking item

                int i = 0;
                while (i < m_att.TrackedShips.Count)
                {
                    
                    ShipState ship = m_att.TrackedShips[i];

                    if (m_att.TrackedShips[i].Self == null)
                    {
                        // Ship is destroyed stop tracking
                        m_att.TrackedShips.RemoveAt(i);
                        continue;
                    }
                    else
                        i++;


                    int level = ship.ThreatLevel;

                    // 5% - 50% chance of spawning
                    if(Random.Range(-10, 10) >= level)
                    {
                        // we are able to spawn a ship

                        // determine a max of persuers from
                        // Threat level
                        int shipCount = level < 4 ? 3 : level < 8 ? 2 : 1;

                        // add a ship we have no reached cap
                        if(ship.PersuitCount < shipCount)
                        {
                            // Send the ai message 
                            SpawnAIMessage ssm = new SpawnAIMessage();
                            ssm.Agent = m_att.Agents["raider_small"];
                            ssm.ID = SystemManager.Space.ID;
                            ssm.Point = Math.RandomWithinRange
                                (ship.Self.position, 100, 200);

                            SystemManager.singleton.client.Send((short)MSGCHANNEL.SPAWNAI, ssm);

                            // We have the space
                            // Spawn the ship
                            //ship.AddPersuit(BuildAgent(ship.Self, ));
                        }
                    }

                    yield return null;
                }
            }
        }

        #endregion
    }
}
