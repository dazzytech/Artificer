using Data.Space;
using Data.Space.Library;
using Networking;
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
    [System.Serializable]
    public struct AgentGroup
    {
        #region ATTRIBUTES

        public uint m_focus;
        public uint[] m_agents;
        public int AgentCount;

        #endregion

        #region ACCESSORS

        /// <summary>
        /// Stores an accessible reference 
        /// to the group focus
        /// </summary>
        public GameObject Focus
        {
            get
            {
                return ClientScene.FindLocalObject
                      (new NetworkInstanceId(m_focus));
            }
        }

        #endregion

        public AgentGroup(uint focus)
        {
            m_focus = focus;
            m_agents = new uint[0];
            AgentCount = 0;
        }

        #region PUBLIC INTERACTION

        /// <summary>
        /// Adds a reference to an agent in group
        /// </summary>
        /// <param name="ship"></param>
        public void AddAgent(uint agent)
        {
            List<uint> temp = new List<uint>(m_agents);
            temp.Add(agent);
            m_agents = (uint[])temp.ToArray().Clone();

            AgentCount++;
        }

        /// <summary>
        /// Removes a reference from the agent
        /// </summary>
        /// <param name="agent"></param>
        public void RemoveAgent(int agent)
        {
            List<uint> temp = new List<uint>(m_agents);
            temp.RemoveAt(agent);
            m_agents = (uint[])temp.ToArray().Clone();

            AgentCount--;
        }

        #endregion

        #region EVENTS

        /// <summary>
        /// Deletes reference to the agent
        /// ship if it is destroyed
        /// </summary>
        /// <param name="DD"></param>
        public bool ShipDestroyed(DestroyDespatch DD)
        {
            for (int i = 0; i < AgentCount; i++)
            {
                if (m_agents[i] == DD.SelfID.Value)
                {
                    RemoveAgent(i);
                    i--;
                    return true;
                }
            }
            return false;
        }

        #endregion
    }

    public class SyncListAgentGroup : SyncListStruct<AgentGroup>
    { }

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
    /// Base spawn object used to spawn AI object
    /// </summary>
    public class SpawnManager : NetworkBehaviour
    {
        #region ATTRIBUTES

        /// <summary>
        /// possible agents that can spawn
        /// from this spawner
        /// </summary>
        [SerializeField]
        private AgentSpawn[] m_spawn;

        public int TeamID = -1;

        #endregion

        #region MONOBEHAVIOUR

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Spawns the object with this player's
        /// authority and sets the spawn process message
        /// </summary>
        [Server]
        public void SpawnShip(int playerID, uint targetID, uint spawnID,
            Vector3 location, string agent, uint homeID)
        {
            GameObject GO = Instantiate
                (SystemManager.singleton.playerPrefab);

            GO.transform.position = location;
             
            NetworkConnection playerConn = 
                SystemManager.GameMSG.GetPlayerConn(playerID);

            // Spawn the ship on the network
            NetworkServer.SpawnWithClientAuthority
                (GO, playerConn);

            // Retrieve the agent info
            AgentData agentData = GameObject.Find("ai").
                GetComponent<AIManager>().AgentLibrary[agent];

            // Assign the information while on the server
            GO.GetComponent<ShipGenerator>().
                AssignShipData(ShipLibrary.GetShip
                (agentData.Ship[UnityEngine.Random.Range
                (0, agentData.Ship.Count())]), TeamID, playerConn);

            AssignAgent(GO.GetComponent<NetworkIdentity>().netId.Value, homeID);

            SpawnNPCMessage snm = new SpawnNPCMessage();
            snm.SpawnID = spawnID;
            snm.TargetID = targetID;
            snm.HomeID = homeID;
            snm.AgentID = GO.GetComponent<NetworkIdentity>().netId.Value;
            snm.AgentType = agent;

            // Send message
            NetworkServer.SendToClient(playerConn.connectionId,
                (short)MSGCHANNEL.PROCESSNPC, snm);
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

        /// <summary>
        /// Used to override children for
        /// assigning agents to lists
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="target"></param>
        [Server]
        protected virtual void AssignAgent(uint agentID, uint homeID)
        {
        }

        protected virtual FSM InitializeAgent
            (uint agentID, uint targetID, string agent, uint homeID)
        {
            GameObject GO = ClientScene.FindLocalObject
                (new NetworkInstanceId(agentID));

            if (GO == null)
                return null;

            // Retrieve the agent info
            AgentData agentData = GameObject.Find("ai").
                GetComponent<AIManager>().AgentLibrary[agent];

            // Use template to determine agent class
            FSM agentFSM = null;

            switch (agentData.Template)
            {
                case "assault":
                    agentFSM = GO.AddComponent<AssaultAgent>();
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

            // Access the target given to us
            GameObject target = ClientScene.FindLocalObject
                (new NetworkInstanceId(targetID));

            if (target != null)
                agentFSM.AssignTarget(target.transform);

            // Access the home object for the agent given to us
            GameObject home = ClientScene.FindLocalObject
                (new NetworkInstanceId(homeID));

            if (home != null)
                agentFSM.AssignHome(home.transform);

            return agentFSM;
        }

        #endregion

        #region EVENT

        /// <summary>
        /// Builds the ship item and then 
        /// calls the command to network spawn the object
        /// and to send the spawn me message
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected void BuildAgentListener
            (NetworkMessage netMsg)
        {
            SpawnNPCMessage snm = netMsg.ReadMessage<SpawnNPCMessage>();

            if (snm.SpawnID == netId.Value)
            {
                InitializeAgent(snm.AgentID, snm.TargetID, snm.AgentType, snm.HomeID);

                NetworkManager.singleton.client.UnregisterHandler((short)MSGCHANNEL.PROCESSNPC);
            }
        }

        #endregion
    }
}
