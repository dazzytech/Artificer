using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;

using Data.Space;
using Space.Projectiles;
using Space.Segment;
using Networking;

namespace Space.GameFunctions
{
    /// <summary>
    /// Handles messages sent from other parts of the program
    /// Has a refence to other parts 
    /// </summary>
    public class GameMessageHandler : NetworkBehaviour
    {
        public GameController Con;
        public GameServerEvents Event;

        void Awake()
        {
            NetworkServer.RegisterHandler((short)MSGCHANNEL.TEAMSELECTED, OnAssignToTeam);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.SPAWNPLAYER, OnSpawnPlayerAt);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.SHIPHIT, OnShipHit);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.BUILDPROJECTILE, OnBuildProjectile);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.OBJECTHIT, OnObjectHit);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.INTEGRITYCHANGE, OnIntegrityChanged);

            // call Server events
            NetworkServer.RegisterHandler((short)MSGCHANNEL.SHIPDESTROYED, OnShipDestroyed);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.STATIONDESTROYED, OnStationDestroyed);
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

        [Server]
        public void OnIntegrityChanged(NetworkMessage msg)
        {
            Con.OnIntegrityChanged(msg.ReadMessage<IntegrityChangedMsg>());
        }

        // Server Event Calling

        [Server]
        public void OnShipDestroyed(NetworkMessage msg)
        {
            Event.ShipDestroyed(msg.ReadMessage<ShipDestroyMessage>());
        }

        [Server]
        public void OnStationDestroyed(NetworkMessage msg)
        {
            Event.StationDestroyed(msg.ReadMessage<StationDestroyMessage>());
        }
    }
}