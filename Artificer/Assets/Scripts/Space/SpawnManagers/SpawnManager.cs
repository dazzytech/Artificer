using Data.Space;
using Data.Space.Library;
using Space.AI;
using Space.AI.Agent;
using Space.Ship;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Space.Spawn
{
    /// <summary>
    /// Base spawn object used to spawn AI object
    /// </summary>
    public class SpawnManager : NetworkBehaviour
    {
        #region INLINE CLASS

        /// <summary>
        /// Stores information about
        /// possible agent spawns that can
        /// be spawned from this spawner
        /// </summary>
        [System.Serializable]
        public class AgentSpawn
        {
            /// <summary>
            /// The agent that we will spawn
            /// </summary>
            public string AgentName;

            /// <summary>
            /// numeric value that determines if
            /// we have access to this agent or not
            /// </summary>
            public int SpawnRequirements;

            /// <summary>
            /// chance that the agent will be 
            /// chosen to spawn
            /// </summary>
            public float SpawnChance;
        }

        /// <summary>
        /// Logs information about a tracked object
        /// and any agents associated with that object
        /// </summary>
        public class AgentGroupTracker: IndexedObject
        {
            #region ATTRIBUTES

            /// <summary>
            /// A central focus point for
            /// the ai agents
            /// </summary>
            private Transform m_focus;

            /// <summary>
            /// Reference to all agents within this 
            /// group
            /// </summary>
            private List<NetworkInstanceId> m_agents;

            #endregion

            #region CONSTRUCTORS

            public AgentGroupTracker()
            {
                m_agents = new List<NetworkInstanceId>();

                SystemManager.Events.EventShipDestroyed
                    += ShipDestroyedEvent;
            }

            /// <summary>
            /// Initializes the object with 
            /// reference to a focal object
            /// </summary>
            /// <param name="focus"></param>
            public AgentGroupTracker(Transform focus)
            {
                m_focus = focus;
                m_agents = new List<NetworkInstanceId>();

                SystemManager.Events.EventShipDestroyed
                    += ShipDestroyedEvent;
            }

            #endregion

            #region PUBLIC INTERACTION

            /// <summary>
            /// Adds a reference to an agent in group
            /// </summary>
            /// <param name="ship"></param>
            public void AddAgent(NetworkInstanceId agent)
            {
                if (agent != null)
                    m_agents.Add(agent);
            }

            #endregion

            #region EVENTS

            /// <summary>
            /// Deletes reference to the agent
            /// ship if it is destroyed
            /// </summary>
            /// <param name="DD"></param>
            private void ShipDestroyedEvent(DestroyDespatch DD)
            {
                for (int i = 0; i < m_agents.Count; i++)
                {
                    if (m_agents[i] == DD.Self)
                    {
                        m_agents.RemoveAt(i);
                        i--;
                    }
                }

            }

            #endregion

            #region ACCESSORS
            
            /// <summary>
            /// Returns the amount of raiders targeting
            /// </summary>
            public int AgentCount
            {
                get
                {
                    return m_agents.Count;
                }
            }

            /// <summary>
            /// Stores an accessible reference 
            /// to the group focus
            /// </summary>
            public Transform Focus
            {
                get { return m_focus; }
            }

            /// <summary>
            /// network instance if of the focus
            /// object
            /// </summary>
            public NetworkInstanceId FocusNetID
            {
                get { return Focus.GetComponent<NetworkIdentity>().netId; }
            }

            #endregion
        }

        #endregion

        #region ATTRIBUTES

        /// <summary>
        /// Used to access ai list
        /// </summary>
        [SerializeField]
        private AIManager m_AI;

        /// <summary>
        /// possible agents that can spawn
        /// from this spawner
        /// </summary>
        [SerializeField]
        private AgentSpawn[] m_spawn;

        #endregion

        #region ACCESSOR

        /// <summary>
        /// If this is a player team then
        /// we dont spawn AI
        /// </summary>
        protected bool PlayerTeam
        {
            get { return m_AI == null; }
        }

        /// <summary>
        /// Allows ai manager to set self
        /// </summary>
        public AIManager AI
        {
            set { m_AI = value; }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Spawns the object with this player's
        /// authority and sets the spawn process message
        /// </summary>
        [Command]
        public void CmdSpawnShip(string agent,
            uint playerNetID, Vector3 location, uint targetID)
        {
            GameObject GO = Instantiate
                (SystemManager.singleton.playerPrefab);

            GO.transform.position = location;

            NetworkConnection playerConn = SystemManager.Space.PlayerConn
                (new NetworkInstanceId(playerNetID));

            // Spawn the ship on the network
            NetworkServer.SpawnWithClientAuthority
                (GO, playerConn);

            // Retrieve the agent info
            AgentData agentData = m_AI.AgentLibrary[agent];

            // Assign the information while on the server
            GO.GetComponent<ShipGenerator>().
                AssignShipData(ShipLibrary.GetShip
                (agentData.Ship[UnityEngine.Random.Range
                (0, agentData.Ship.Count())]), -1, playerConn);

            // Build the agent info in the correct client
            RpcBuildAgent(playerNetID,
                GO.GetComponent<NetworkIdentity>().netId.Value,
                targetID, agent);
        }

        /// <summary>
        /// Builds the ship item and then 
        /// calls the command to network spawn the object
        /// and to send the spawn me message
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        [ClientRpc]
        public void RpcBuildAgent
            (uint selfID, uint agentID, uint targetID, string agent)
        {
            if (SystemManager.Space.NetID == selfID)
            {
                BuildAgent(selfID, agentID, targetID, agent);
            }
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Returns a random raider agent
        /// based on the threat level of the target
        /// higher threat means more dangerous agent
        /// </summary>
        /// <param name="threatLevel"></param>
        /// <returns></returns>
        protected string RandomAgent(int requirement)
        {
            // retreive list of raiderinfo based on threat
            // ordered by threat
            AgentSpawn[] spawns = (AgentSpawn[])m_spawn.Where
                (x => x.SpawnRequirements <= requirement).
                OrderBy(x => x.SpawnRequirements).ToArray().Clone();

            // range contains the max value for the random.range
            float range = 0;

            // loop through each spawn, add the current range to 
            // the chance, and the chance to the current range seperately
            for (int i = 0; i < spawns.Count(); i++)
            {
                range += spawns[i].SpawnChance;
                spawns[i].SpawnChance = range;
            }

            float result = UnityEngine.Random.Range(0, range);

            // pick the first number that is bigger than the random number
            for (int i = 0; i < spawns.Count(); i++)
            {
                if (spawns[i].SpawnChance >= result)
                    return spawns[i].AgentName;
            }

            // we shouldn't be here
            Debug.Log("Error: Spawn Manager - Random Agent: result failed to pick agent.");
            return "";
        }

        protected virtual void BuildAgent
            (uint selfID, uint agentID, uint targetID, string agent)
        {
            // Access the target given to us
            GameObject target = ClientScene.FindLocalObject
                (new NetworkInstanceId(targetID));

            if (target == null)
                return;

            GameObject GO = ClientScene.FindLocalObject
                (new NetworkInstanceId(agentID));

            if (GO == null)
                return;

            // Retrieve the agent info
            AgentData agentData = m_AI.AgentLibrary[agent];

            // Use template to determine agent class
            FSM agentFSM = null;

            switch (agentData.Template)
            {
                case "assault":
                    agentFSM = GO.AddComponent<AssaultAgent>();
                    ((AssaultAgent)agentFSM).DefineTarget(target.transform);
                    break;
                case "guard":
                    agentFSM = GO.AddComponent<GuardAgent>();
                    break;
                case "resource":
                    agentFSM = GO.AddComponent<ResourceAgent>();
                    break;
                case "travel":
                    agentFSM = GO.AddComponent<TravelAgent>();
                    break;
            }

            agentFSM.EstablishAgent(agentData);
        }

        #endregion
    }
}
