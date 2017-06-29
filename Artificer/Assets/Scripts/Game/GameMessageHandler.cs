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

        private static GameMessageHandler m_singleton;

        #endregion

        #region MONO BEHAVIOUR

        void Awake()
        {
            if (m_singleton == null)
                m_singleton = this;
            else
            {
                Destroy(gameObject);
                NetworkServer.UnSpawn(gameObject);
                return;
            }

            DontDestroyOnLoad(this);

            #region SPACE MESSAGES

            NetworkServer.RegisterHandler((short)MSGCHANNEL.TEAMSELECTED, OnAssignToTeam);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.SPAWNPLAYER, OnSpawnPlayerAt);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.SPAWNAI, OnBuildAI);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.SHIPHIT, OnShipHit);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.BUILDPROJECTILE, OnBuildProjectile);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.BUILDSTATION, OnBuildStation);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.OBJECTHIT, OnObjectHit);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.INTEGRITYCHANGE, OnIntegrityChanged);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.CHATMESSAGESERVER, OnChatMessage);

            #endregion

            #region SPACE EVENTS

            NetworkServer.RegisterHandler((short)MSGCHANNEL.SHIPDESTROYED, OnShipDestroyed);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.STATIONDESTROYED, OnStationDestroyed);

            #endregion

            SystemManager.GameMSG = this;
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

        [Server]
        public void OnChatMessage(NetworkMessage msg)
        {
            NetworkServer.SendToAll((short)MSGCHANNEL.CHATMESSAGECLIENT,
                msg.ReadMessage<ChatParamMsg>());
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
            m_con.SpawnPlayer(ssm.PlayerID, ssm.SpawnID, ssm.ShipName);
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
            TeamSelectionMessage tsm = 
                netMsg.ReadMessage<TeamSelectionMessage>();
            m_con.AssignToTeam(tsm.Selected, tsm.ID);
        }

        #endregion

        #region SPACE MESSAGES

        /// <summary>
        /// message by ai manager to build an ai agent with the 
        /// agent properties
        /// </summary>
        /// <param name="msg"></param>
        public void OnBuildAI(NetworkMessage msg)
        {
            SpawnAIMessage spawnAI = msg.ReadMessage<SpawnAIMessage>();
            //m_con.SpawnAI(spawnAI.ID, spawnAI.Agent, spawnAI.Point);
        }

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
        /// Called when player builds a station
        /// </summary>
        /// <param name="msg"></param>
        [Server]
        public void OnBuildStation(NetworkMessage msg)
        {
            StationBuildMessage stationMsg =
                msg.ReadMessage<StationBuildMessage>();

            m_con.BuildStation(stationMsg.PrefabName,
                stationMsg.teamID, stationMsg.Position);
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