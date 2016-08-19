using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;

using Data.Space;
using Space.Projectiles;
using Space.Segment;

namespace Space.GameFunctions
{
    #region NETWORK MESSAGE OBJECTS 

    /// <summary>
    /// Message containig the ID of the client
    /// and the team selected
    /// </summary>
    public class TeamSelectionMessage : MessageBase
    {
        public int Selected;
        public int ID;
    }

    public class SpawnSelectionMessage : MessageBase
    {
        public int PlayerID;
        public int SpawnID;
        public int ShipID;
    }

    public class ProjectileBuildMessage: MessageBase
    {
        public Vector3 Position;
        public int PrefabIndex, shooterID;
        public WeaponData WData;
    }

    public class ShipColliderHitMessage:MessageBase
    {
        public int[] HitComponents;
        public NetworkInstanceId ShipID;
        public HitData HitD;
    }

    public class SOColliderHitMessage : MessageBase
    {
        public NetworkInstanceId SObjectID;
        public HitData HitD;
    }

    #endregion

    /// <summary>
    /// Handles messages sent from other parts of the program
    /// Has a refence to other parts 
    /// </summary>
    public class GameMessageHandler : NetworkBehaviour
    {
        public GameController Con;

        void Awake()
        {
            // For team selection
            NetworkServer.RegisterHandler(MsgType.Highest + 7, OnAssignToTeam);
            NetworkServer.RegisterHandler(MsgType.Highest + 8, OnSpawnPlayerAt);
            NetworkServer.RegisterHandler(MsgType.Highest + 10, OnShipHit);
            NetworkServer.RegisterHandler(MsgType.Highest + 12, OnBuildProjectile);
            NetworkServer.RegisterHandler(MsgType.Highest + 15, OnObjectHit);
        }

        /// <summary>
        /// Adds the new player 
        /// called by game manager
        /// </summary>
        /// <returns>The new player.</returns>
        /// <param name="conn">Conn.</param>
        /// <param name="client">Client.</param>
        [Server]
        public void AddNewPlayer
            (short playerControllerId, NetworkConnection conn)
        {
            Con.AddNewPlayer(playerControllerId, conn);
        }

        /// <summary>
        /// Called when the player 
        /// chooses a spawn point to spawn their ship
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="SpawnID"></param>
        /// <param name="shipID"></param>
        [Server]
        public void OnSpawnPlayerAt
            (NetworkMessage netMsg)
        {
            // Retreive variables and display options
            SpawnSelectionMessage ssm = netMsg.ReadMessage<SpawnSelectionMessage>();
            Con.SpawnPlayer(ssm.PlayerID, ssm.SpawnID, ssm.ShipID);
        }

        /// <summary>
        /// Called when player picked a side
        /// </summary>
        /// <param name="teamID"></param>
        /// <param name="playerID"></param>
        [Server]
        public void OnAssignToTeam(NetworkMessage netMsg)
        {
            // Retreive variables and display options
            TeamSelectionMessage tsm = netMsg.ReadMessage<TeamSelectionMessage>();
            Con.AssignToTeam(tsm.Selected, tsm.ID);
        }

        /// <summary>
        /// Initialize from gamemanager 
        /// </summary>
        [Server]
        public void InitializeGameParameters(/*GameParameters param*/)
        {
            Con.Initialize();
        }

        [Server]
        public void OnBuildProjectile(NetworkMessage msg)
        {
            ProjectileBuildMessage projMsg = msg.ReadMessage<ProjectileBuildMessage>();

            Con.BuildProjectile(projMsg.PrefabIndex, projMsg.shooterID, projMsg.Position, projMsg.WData);
        }

        [Server]
        public void OnShipHit(NetworkMessage msg)
        {
            Con.ShipHit(msg.ReadMessage<ShipColliderHitMessage>());
        }

        [Server]
        public void OnObjectHit(NetworkMessage msg)
        {
            Con.ObjectHit(msg.ReadMessage<SOColliderHitMessage>());
        }
    }
}