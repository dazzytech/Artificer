using Data.UI;
using Data.Space;
using Data.Space.Library;
using Networking;
using Space.Ship.Components.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Space.AI;
using Space.AI.Agent;

namespace Space.Ship.Components.Listener
{
    public class ControlListener : ComponentListener
    {
        #region ATTRIBUTES

        private ControlAttributes m_att;

        #endregion

        #region ACCESSOR

        /// <summary>
        /// Library of generated prefabs that the ship can
        /// spawn
        /// </summary>
        private ShipSpawnData[] Prefabs
        {
            get
            {
                return SystemManager.PlayerShips;
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        public override void Activate()
        {
            //open ui
            // Only do if ship is not docked or in combat
            if (!m_att.Ship.InCombat)
            {
                m_att.Ship.DisableShip();

                SystemManager.UIState.SetState(UIState.Edit);

                SystemManager.UI.InitializeIDE(m_att.Ship);
            }
        }

        /// <summary>
        /// Builds the NPC and assigns the agent
        /// </summary>
        /// <param name="index"></param>
        public void SpawnNPC(ICustomScript customScript)
        {
            if (customScript == null)
                return;

            m_att.Script = customScript;

            ShipSpawnData npc = Prefabs[0];

            #region CHECK AND APPLY COST

            WalletData temp = SystemManager.Wallet;

            if (!temp.Withdraw(npc.Ship.Cost))
            {
                SystemManager.UIPrompt.DisplayPrompt
                    ("Insufficient funds to deploy npc", 3f);
                return;
            }

            SystemManager.Wallet = temp;

            #endregion

            #region SEND COMMAND SPAWN

            NetworkManager.singleton.client.RegisterHandler
                                ((short)MSGCHANNEL.PROCESSNPC, BuildPrefabListener);

            Vector3 location = Math.RandomWithinRange
                                (transform.position, 5, 10);

            CmdSpawnNPC(SystemManager.Space.ID, netId.Value, location);

            #endregion
        }

        /// <summary>
        /// Generates the ship NPC on the server
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="prefabID"></param>
        /// <param name="selfID"></param>
        /// <param name="location"></param>
        public void CmdSpawnNPC(int playerID, uint selfID,
            Vector3 location)
        {
            #region GENERATE SHIP

            GameObject GO = Instantiate
               (SystemManager.singleton.playerPrefab);

            GO.transform.position = location;

            NetworkConnection playerConn =
                SystemManager.GameMSG.GetPlayerConn(playerID);

            // Spawn the ship on the network
            NetworkServer.SpawnWithClientAuthority
                (GO, playerConn);

            // Assign the information while on the server
            GO.GetComponent<ShipGenerator>().
                AssignShipData((SystemManager.PlayerShips[0].Ship), 
                m_att.Ship.TeamID, playerConn);

            #endregion

            #region SEND TO CLIENT

            SpawnNPCMessage snm = new SpawnNPCMessage();
            snm.SpawnID = selfID;
            snm.TargetID = 0;
            snm.HomeID = 0;
            snm.AgentID = GO.GetComponent<NetworkIdentity>().netId.Value;
            snm.AgentType = "";

            // Send message for client to proceed registering the NPC
            NetworkServer.SendToClient(playerConn.connectionId,
                (short)MSGCHANNEL.PROCESSNPC, snm);

            #endregion
        }

        #endregion

        #region PRIVATE UTILITIES

        protected override void InitializeComponent()
        {
            base.InitializeComponent();

            ComponentType = "Control";
            m_att = GetComponent<ControlAttributes>();

            if (hasAuthority)
            {
                // what is needed?
                m_att.Agent = new List<AI.FSM>();
            }
        }

        /// <summary>
        /// Retrieves the data from the the server 
        /// and initialises the NPC component
        /// </summary>
        /// <param name="agentID"></param>
        /// <param name="index"></param>
        /// <param name="homeID"></param>
        private void BuildPrefab(uint agentID, uint index, uint homeID)
        {
            GameObject GO = ClientScene.FindLocalObject
               (new NetworkInstanceId(agentID));

            if (GO == null)
                return;

            // assign FSM to custom FSM
            UserAgent userFSM = GO.AddComponent<UserAgent>();

            // Initialize FSM
            userFSM.SetNPC(m_att.Script, this);

            m_att.Agent.Add(userFSM);
        }

        #endregion

        #region EVENT

        protected void BuildPrefabListener
           (NetworkMessage netMsg)
        {
            SpawnNPCMessage snm = netMsg.ReadMessage<SpawnNPCMessage>();

            if (snm.SpawnID == netId.Value)
            {
                // target ID contains the index of the prefab
                BuildPrefab(snm.AgentID, snm.TargetID, snm.HomeID);

                NATTraversal.NetworkManager.singleton.client.
                    UnregisterHandler((short)MSGCHANNEL.PROCESSNPC);
            }
        }

        #endregion
    }
}