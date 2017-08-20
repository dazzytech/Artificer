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
        #region ATTRIBUTES

        /// <summary>
        /// Used to access ai list
        /// </summary>
        [SerializeField]
        private AIManager m_AI;

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
