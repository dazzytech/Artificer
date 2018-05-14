using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;

using Data.Space;
using Space.Projectiles;
using Space.Segment;
using Networking;
using Data.UI;
using Space.Teams;
using UnityEngine.SceneManagement;

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

        private static GameMessageHandler m_singleton;

        #endregion

        #region MONO BEHAVIOUR

        void Awake()
        {

            if (NetworkManager.singleton == null)
            {
                Destroy(gameObject);
                SceneManager.LoadScene("TitleScene");
            }

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
            NetworkServer.RegisterHandler((short)MSGCHANNEL.SPAWNNPC, OnSpawnNpc);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.SPAWNPLAYER, OnSpawnPlayerAt);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.SHIPHIT, OnShipHit);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.BUILDSTATION, OnBuildStation);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.OBJECTHIT, OnObjectHit);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.INTEGRITYCHANGE, OnIntegrityChanged);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.CHATMESSAGESERVER, OnChatMessage);
            NetworkServer.RegisterHandler((short)MSGCHANNEL.TRANSACTIONSERVER, OnTransaction);

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
        public void InitializeGameParam(GameParameters param)
        {
            GameAttributes att = m_con.Initialize(param);
        }

        /// <summary>
        /// Initialize the game based on the current scene
        /// </summary>
        /// <param name="scene"></param>
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

        /// <summary>
        /// When all home bases are destroyed
        /// go to last scene, where the player can return to lobby
        /// </summary>
        /// <param name="winner">The ID of the winning team</param>
        [Server]
        public void OnEndGame(int winner)
        {

        }

        #endregion

        #region SYSTEM MESSAGES PLAYER

        #endregion

        #region SPACE MESSAGES

        /// <summary>
        /// called by AI manager to store a team
        /// </summary>
        /// <param name="team"></param>
        public void AddTeam(TeamController team)
        {
            m_con.AddTeam(team);
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

        [Server]
        public void OnTransaction(NetworkMessage msg)
        {
            m_con.OnTransaction(msg.ReadMessage<TransactionMessage>());
        }

        /// <summary>
        /// Spawns a raider object near target
        /// </summary>
        /// <param name="netMsg"></param>
        [Server]
        public void OnSpawnNpc(NetworkMessage netMsg)
        {
            SpawnNPCMessage snm = netMsg.ReadMessage<SpawnNPCMessage>();
            m_con.SpawnNpc(snm.SelfID, snm.TargetID, 
                snm.SpawnID, snm.AgentType, snm.Location, snm.HomeID);
        }

        #region PLAYER MESSAGES

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
            m_con.SpawnPlayer(ssm.PlayerID, ssm.SpawnID, ssm.Ship);
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

        #endregion

        #region SERVER EVENTS

        [Server]
        public void OnShipCreated(NetworkInstanceId Self, int ID, int Align)
        {
            SystemManager.Events.ShipCreated(Self, ID, Align);
        }

        [Server]
        public void OnStationCreated(NetworkInstanceId Station)
        {
            SystemManager.Events.StationCreated(Station);
        }

        [Server]
        public void OnShipDestroyed(NetworkMessage msg)
        {
            SystemManager.Events.ShipDestroyed(msg.ReadMessage<ShipDestroyMessage>());
        }

        [Server]
        public void OnStationDestroyed(NetworkMessage msg)
        {
            SystemManager.Events.StationDestroyed(msg.ReadMessage<StationDestroyMessage>());
        }

        #endregion

        #region ACCESS SERVER

        /// <summary>
        /// Used to retrieve the connection
        /// for the player on the server
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public NetworkConnection GetPlayerConn(int playerID)
        {
            return m_con.GetPlayerConn(playerID);
        }

        #endregion

        #endregion
    }
}