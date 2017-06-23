using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System;
using System.Collections;
using Serializer;
using Data.Shared;
using Data.Space.Library;
using Space.Segment.Generator;
using Space.UI;
using Networking;

namespace Space.Ship
{
    /// <summary>
    /// When a ship is initialized then 
    /// </summary>
    public class ShipInitializer : NetworkBehaviour
    {
        #region EVENTS

        public delegate void ShipEvent(ShipAttributes Ship);

        public static event ShipEvent OnShipCreated;

        #endregion

        #region ATTRIBUTES

        public ShipAttributes ShipAtt;

        // Store a reference to the ships data
        [SyncVar]
        public string Ship;
        [SyncVar]
        public bool hasSpawned;

        #endregion

        #region ONSTART OVERRIDES

        /// <summary>
        /// Builds the ship if it was build on server already
        /// </summary>
        public override void OnStartClient()
        {
            // This player may have spawned before this client joined the game, 
            // so if that's the case, spawn it now. Otherwise, just wait for the RpcSpawnMe call.
            if (hasSpawned)
            {
                SetUpPlayer(SystemManager.Space.TeamID);
                // Add this item to local UI list
                // p.s should be performed on each client without cmd
                SystemManager.UI.AddUIPiece(transform);

                if (OnShipCreated != null)
                    OnShipCreated(ShipAtt);
            }
        }

        #endregion

        #region MONO BEHAVIOUR

        void Awake()
        {
            NetworkManager.singleton.client.RegisterHandler((short)MSGCHANNEL.SPAWNME, OnSpawnMe);
        }

        #endregion

        #region SHIP INITIALIZATION

        /// <summary>
        /// Assigns the passed ship data
        /// to the player's own ship data variable
        /// </summary>
        /// <param name="a_data">ship data to pass</param>
        private void OnSpawnMe(NetworkMessage netMsg)
        {
            string shipName = netMsg.
                ReadMessage<StringMessage>().value;
            
            CmdSpawnMe(shipName, SystemManager.Space.TeamID);
        }

        [Command]
        private void CmdSpawnMe(string shipName, int teamID)
        {
            if (hasSpawned)
                return; 

            hasSpawned = true;
            Ship = shipName;
            RpcSpawnMe(shipName, teamID);

            SystemManager.UI.RpcAddRemotePlayer(netId);
        }

        /// <summary>
        /// Sent to each client to instruct them to build the ship
        /// </summary>
        /// <param name="a_data">ship data to pass</param>
        [ClientRpc]
        private void RpcSpawnMe(string shipName, int teamID)
        {
            //spawnData will be synced by the server automatically,
            //but I don't trust it to arrive before this call, so I pass it into
            //this function anyway to be sure.
            Ship = shipName;
            SetUpPlayer(teamID);
            

            if(OnShipCreated != null)
                OnShipCreated(ShipAtt);
        }

        /// <summary>
        /// Construct the ship object for the player
        /// </summary>
        private void SetUpPlayer(int teamID)
        {
            ShipAtt = GetComponent<ShipAttributes>();
            ShipAtt.instID = netId;
            ShipAtt.TeamID = teamID;

            //Build the object with spawnData
            ShipGenerator.GenerateShip(ShipLibrary.GetShip(Ship), this.gameObject);

            // add network proximity checker
            NetworkProximityChecker npc = gameObject.AddComponent<NetworkProximityChecker>();
            npc.checkMethod = NetworkProximityChecker.CheckMethod.Physics2D;

            // we will add player interaction if this is our ship
            if (isLocalPlayer)
            {
                // we will need to check this is ours in future
                GameObject.Find("PlayerCamera").SendMessage("SetFollowObj",
                                                            transform.GetChild(0));

                name = "PlayerShip";
                tag = "PlayerShip";

                gameObject.AddComponent<ShipPlayerInputController>();
                ShipAtt.AlignmentLabel = "Player";

                SendMessage("BuildColliders");
            }
            // Any ships spawned before we pick team will need
            // assigning when team is assigned
            else if (SystemManager.Space.Team)
            {
                // Determine tag based on our reference to team
                if (SystemManager.Space.Team.PlayerOnTeam(netId))
                {
                    name = "AllyShip";
                    tag = "Friendly";
                }
                else
                {
                    name = "EnemyShip";
                    tag = "Enemy";
                }
            }
        }

        [Command]
        private void CmdUpdateHUD()
        {
            SystemManager.UI.RpcAddRemotePlayer(netId);
            RpcFinishCreate();
        }

        [ClientRpc]
        private void RpcFinishCreate()
        {
            if (OnShipCreated != null)
                OnShipCreated(ShipAtt);
        }

        #endregion
    }
}

