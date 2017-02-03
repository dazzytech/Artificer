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

        public delegate void ShipEvent();

        public event ShipEvent OnShipCreated;

        #endregion

        #region ATTRIBUTES

        public ShipAttributes _ship;

        // Store a reference to the ships data
        [SyncVar]
        public ShipData Ship;
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
                SetUpPlayer();
                // Add this item to local UI list
                // p.s should be performed on each client without cmd
                GameManager.GUI.AddUIPiece(transform);
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
            //MessageHUD.DisplayMessege(new MsgParam("bold", "Running Command: " + (a_data.GetComponents() == null).ToString()));
            CmdSpawnMe(Serializer.ByteSerializer.getBytes(ShipLibrary.GetShip("Mammoth XI")));
        }

        [Command]
        private void CmdSpawnMe(byte[] shipInfo)
        {
            if (hasSpawned)
                return; 

            hasSpawned = true;
            Ship = Serializer.ByteSerializer.fromBytes(shipInfo);
            RpcSpawnMe(shipInfo);

            GameManager.GUI.RpcAddRemotePlayer(netId);
        }

        /// <summary>
        /// Sent to each client to instruct them to build the ship
        /// </summary>
        /// <param name="a_data">ship data to pass</param>
        [ClientRpc]
        private void RpcSpawnMe(byte[] shipInfo)
        {
            //spawnData will be synced by the server automatically,
            //but I don't trust it to arrive before this call, so I pass it into
            //this function anyway to be sure.
            Ship = Serializer.ByteSerializer.fromBytes(shipInfo);
            SetUpPlayer();
            _ship.instID = this.netId;

            if(OnShipCreated != null)
                OnShipCreated();
        }

        /// <summary>
        /// Construct the ship object for the player
        /// </summary>
        private void SetUpPlayer()
        {
            if (!Ship.Initialized)
                return;

            //Build the object with spawnData
            ShipGenerator.GenerateShip(Ship, this.gameObject);

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
                GetComponent<ShipAttributes>().AlignmentLabel = "Player";

                SendMessage("BuildColliders");
            }
            // Any ships spawned before we pick team will need
            // assigning when team is assigned
            else if (GameManager.Space.Team)
            {
                // Determine tag based on our reference to team
                if (GameManager.Space.Team.PlayerOnTeam(netId))
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
            GameManager.GUI.RpcAddRemotePlayer(netId);
            RpcFinishCreate();
        }

        [ClientRpc]
        private void RpcFinishCreate()
        {
            if (OnShipCreated != null)
                OnShipCreated();
        }

        #endregion

        #region SERIALIZE OVERRIDES

        int byteSize;

        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            if (this.Ship.Initialized)
            {
                writer.Write(true);

                byte[] rawData = ByteSerializer.getBytes(this.Ship);

                int byteSize = rawData.Length;

                // Let us know in advance the size of array
                writer.WritePackedUInt32((uint)byteSize);

                // Write out the information for the head component
                writer.Write(rawData, byteSize);
            }
            else
                writer.Write(false);

            writer.Write(this.hasSpawned);

            return true;
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (reader.ReadBoolean())
            {
                this.byteSize = (int)reader.ReadPackedUInt32();

                this.Ship = ByteSerializer.fromBytes(reader.ReadBytes(this.byteSize));

                this.Ship.Initialized = true;
            }

            this.hasSpawned = reader.ReadBoolean();
        }

        #endregion
    }
}

