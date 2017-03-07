using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;

using Data.Space;
using Space.Projectiles;
using Space.Segment;
using Networking;
using Data.UI;

namespace Game
{
    /// <summary>
    /// Handles messages sent from other parts of the program
    /// Has a refence to other parts 
    /// </summary>
    public class GameMessageHandler : NetworkBehaviour
    {
        #region ATTRIBUTES

        [Header("References")]

        [SerializeField]
        private GameManager m_con;

        [SerializeField]
        private GameServerEvents m_event;

        #endregion

        #region MONO BEHAVIOUR

        void Awake()
        {
            DontDestroyOnLoad(this);

            #region SPACE MESSAGES

            NetworkServer.RegisterHandler((short)MSGCHANNEL.TEAMSELECTED, OnAssignToTeam);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.SPAWNPLAYER, OnSpawnPlayerAt);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.SHIPHIT, OnShipHit);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.BUILDPROJECTILE, OnBuildProjectile);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.OBJECTHIT, OnObjectHit);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.INTEGRITYCHANGE, OnIntegrityChanged);

            #endregion

            #region SPACE EVENTS

            NetworkServer.RegisterHandler((short)MSGCHANNEL.SHIPDESTROYED, OnShipDestroyed);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.STATIONDESTROYED, OnStationDestroyed);

            #endregion
        }

        #endregion

        #region PUBLIC INTERACTION

        #region SYSTEM MESSAGES

        /// <summary>
        /// Initialize from SystemManager 
        /// </summary>
        [Server]
        public void Initialize()
        {
            m_con.Initialize();
        }

        [Server]
        public void SceneChanged(string scene)
        {
            switch(scene)
            {
                case "lobby":
                    m_con.InitializeLobbyScene();
                    break;
                case "play":
                    m_con.InitializeSpaceScene();
                    break;
                
            }
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
            m_con.AddNewPlayer(playerControllerId, conn);
        }

        /// <summary>
        /// Deletes player that left
        /// </summary>
        /// <param name="conn"></param>
        [Server]
        public void RemovePlayer
            (NetworkConnection conn)
        {
            m_con.RemovePlayer(conn);
        }

        #endregion

        #region SYSTEM MESSAGES PLAYER

        #endregion

        #region SPACE PLAYER MESSAGES

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
            m_con.SpawnPlayer(ssm.PlayerID, ssm.SpawnID, ssm.ShipID);
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
            m_con.AssignToTeam(tsm.Selected, tsm.ID);
        }

        #endregion

        #region SPACE MESSAGES

        /// <summary>
        /// builds a projectile on the server
        /// and sends spawn info to each client
        /// </summary>
        /// <param name="msg"></param>
        [Server]
        public void OnBuildProjectile(NetworkMessage msg)
        {
            ProjectileBuildMessage projMsg = msg.ReadMessage<ProjectileBuildMessage>();
            m_con.BuildProjectile(projMsg.PrefabIndex, projMsg.shooterID, projMsg.Position, projMsg.WData);
        }

        /// <summary>
        /// Called when ship has taken damage 
        /// to update all clients
        /// </summary>
        /// <param name="msg"></param>
        [Server]
        public void OnShipHit(NetworkMessage msg)
        {
            m_con.ShipHit(msg.ReadMessage<ShipColliderHitMessage>());
        }

        /// <summary>
        /// Called when an object in space is hit
        /// </summary>
        /// <param name="msg"></param>
        [Server]
        public void OnObjectHit(NetworkMessage msg)
        {
            m_con.ObjectHit(msg.ReadMessage<SOColliderHitMessage>());
        }

        /// <summary>
        /// Called when ship or station takes damage
        /// to display on clients
        /// </summary>
        /// <param name="msg"></param>
        [Server]
        public void OnIntegrityChanged(NetworkMessage msg)
        {
            m_con.OnIntegrityChanged(msg.ReadMessage<IntegrityChangedMsg>());
        }

        #endregion

        #region SERVER EVENTS

        [Server]
        public void OnShipDestroyed(NetworkMessage msg)
        {
            m_event.ShipDestroyed(msg.ReadMessage<ShipDestroyMessage>());
        }

        [Server]
        public void OnStationDestroyed(NetworkMessage msg)
        {
            m_event.StationDestroyed(msg.ReadMessage<StationDestroyMessage>());

        }

        #endregion

        #endregion
    }
}