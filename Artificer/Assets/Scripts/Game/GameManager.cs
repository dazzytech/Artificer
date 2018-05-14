using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using Data.Space;
using Data.Space.Library;
using Space.Ship;
using Space.UI;
using Space.Teams;
using Space.Ship.Components.Listener;
using Space.Projectiles;
using Networking;
using Data.UI;
using Space.Segment;
using Space.AI;
using Space.Spawn;
using Stations;

namespace Game
{
    public enum GameState{Lobby, Play, End}

    /// <summary>
    /// Responsible for game elements such as 
    /// teams, players etc
    /// </summary>
    public class GameManager:NetworkBehaviour
    {
        #region EVENTS

        public delegate void ChangeGameState(GameState newState);
        public event ChangeGameState OnChangeState;

        #endregion

        #region ATTRIBUTES

        [SerializeField]
        private GameAttributes m_att;

        [SerializeField]
        private GameEventListener m_event;

        [SerializeField]
        private GameAccessor m_access;

        #endregion

        #region PUBLIC INTERACTION

        #region SYSTEM MESSAGES

        /// <summary>
        /// Takes the game parameters and
        /// initializes the game
        /// </summary>
        [Server]
        public GameAttributes Initialize(GameParameters param)
        {
            // Assign the spawnable ships for each team
            foreach(ShipSpawnData spawn in param.SpawnableShips)
            {
                m_att.Teams[0].AddSpawnableShip(spawn);
                m_att.Teams[1].AddSpawnableShip(spawn);
            }

            m_att.Teams[0].DefineTeamAssets(param.Wallet);
            m_att.Teams[1].DefineTeamAssets(param.Wallet);

            // Initialize server spacesegment
            m_att.Segment.InitializeSegment(param);

            // Generated stations for the teams
            m_att.Builder.GenerateTeams(m_att.Teams[0], m_att.Teams[1], param);
            m_att.Teams[0].AddEnemyTeam(1);
            m_att.Teams[1].AddEnemyTeam(0);

            m_att.AI.Initialize(param);

            return m_att;
        }

        /// <summary>
        /// Build the Lobby scene for the game
        /// </summary>
        [Server]
        public void InitializeLobbyScene()
        {
            m_att.CurrentState = GameState.Lobby;
        }

        /// <summary>
        /// Retreives game parameters and 
        /// initializes the gameplay logic
        /// </summary>
        [Server]
        public void InitializeSpaceScene()
        {
            // Change the state on the game
            m_att.CurrentState = GameState.Play;

            #region RANDOM TEAM FACTION ASSIGNMENT

            // Initialize Teams
            bool teamsCompleted = false;

            // pick our two teams first
            FactionData teamAcon = FactionLibrary.ReturnFaction(UnityEngine.Random.Range(0, 3));

            FactionData teamBcon = new FactionData();

            // we dont want two of the same teams
            while(!teamsCompleted)
            {
                teamBcon = FactionLibrary.ReturnFaction(UnityEngine.Random.Range(0, 3));
                if (!teamAcon.Equals(teamBcon))
                    teamsCompleted = true;
            }

            #endregion

            #region INITIALIZE SPACE ASSETS

            m_att.GlobalStations = new List<StationAccessor>();
            m_att.Teams = new List<TeamController>();

            m_access = GameObject.Find("_event").GetComponent<GameAccessor>();

            InitializeTeam("Team_A", 0, teamAcon);
            InitializeTeam("Team_B", 1, teamBcon);

            m_att.Segment = GameObject.Find("segment").GetComponent<SegmentManager>();
            m_att.AI = GameObject.Find("ai").GetComponent<AIManager>();

            #endregion

            m_event.InitSpaceScene();
        }

        [Server]
        public void AddNewPlayer
            (short playerControllerId, NetworkConnection conn)
        {
            if (m_att.PlayerInfoList == null)
                m_att.PlayerInfoList = new IndexedList<PlayerConnectionInfo>();

            // Test If this player is already joined to the match
            PlayerConnectionInfo info = m_att.PlayerInfoList
                .FirstOrDefault(o => o.mConnection == conn);

            // if not match then create new info
            if (info == null)
            {
                info = new PlayerConnectionInfo();
                info.mController = playerControllerId;
                info.mConnection = conn;
                info.mTeam = -1;

                // add player to tracking list
                m_att.PlayerInfoList.Add(info);
            }
            else
            {
                info.mController = playerControllerId;
                info.mTeam = -1;
            }

            // Assign our intial ID to the system
            IntegerMessage iMsg = new IntegerMessage(info.ID);
            NetworkServer.SendToClient(info.mConnection.connectionId,
                (short)MSGCHANNEL.NEWID, iMsg);

            // If currently in play the initialize 
            if (m_att.CurrentState == GameState.Play)
                InitializePlayer(info);
            else if (m_att.CurrentState == GameState.Lobby)
                InitializeLobbyPlayer(info);
        }

        /// <summary>
        /// Deletes player if been dropped
        /// </summary>
        /// <param name="conn"></param>
        [Server]
        public void RemovePlayer
            (NetworkConnection conn)
        {
            if (m_att.PlayerInfoList == null)
                return;

            // Test If this player is already joined to the match
            PlayerConnectionInfo info = m_att.PlayerInfoList
                .FirstOrDefault(o => o.mConnection == conn);

            if (info != null)
            {
                m_att.PlayerInfoList.Remove(info);
                if(SystemManager.Server != null)
                    SystemManager.Server.
                        DeletePlayerFromServer(info.ID);

                m_att.Teams[info.mTeam].Players--;
            }
        }

        #endregion

        #region SPACE MESSAGES

        /// <summary>
        /// Player has picked a team
        /// store that team with our info
        /// </summary>
        /// <param name="TeamID"></param>
        /// <param name="conn"></param>
        [Server]
        public void AssignToTeam(int TeamID, int playerID)
        {
            if (m_att.PlayerInfoList == null)
                return;

            PlayerConnectionInfo pInfo = 
                m_att.PlayerInfoList.Item(playerID);

            // network instance ID of our team object
            NetworkInstanceId teamNetID;

            if (pInfo == null)
            {
                Debug.Log("Error: Game Controller - Assign To Team: Player not found");
                return;             // something is wrong
            }

            // Assign that player to the team
            pInfo.mTeam = TeamID;

            // Amount of starter cash
            int starterFund = 0;

            // Apply changes to team
            teamNetID = m_att.Teams[TeamID].netId;
            starterFund = m_att.Teams[TeamID].Expend(1000000);
            m_att.Teams[TeamID].Players++;

            // Create netID message
            IntegerMessage netMsg = new IntegerMessage((int)teamNetID.Value);

            // Create Transaction fund
            TransactionMessage transaction
                = new TransactionMessage();

            transaction.Recipiant = -1;
            transaction.CurrencyDir = 1;
            transaction.AssetDir = 0;
            transaction.CurrencyAmount = starterFund;

            // Send message for clients space obj
            NetworkServer.SendToClient(pInfo.mConnection.connectionId,
                (short)MSGCHANNEL.ASSIGNTEAM, netMsg);

            // Send message for clients space obj
            NetworkServer.SendToClient(pInfo.mConnection.connectionId,
                (short)MSGCHANNEL.TRANSACTIONCLIENT, transaction);
        }

        /// <summary>
        /// Allows team controller to build a team
        /// and store in game att
        /// </summary>
        /// <param name="team"></param>
        [Server]
        public void AddTeam(TeamController team)
        {
            m_att.Teams.Add(team);
            m_access.AddTeam(team.netId.Value);
        }

        #region SPAWNING

        [Server]
        public void SpawnPlayer(int playerID, int stationID, ShipData ship)
        {
            PlayerConnectionInfo info = 
                m_att.PlayerInfoList.Item(playerID);

            GameObject GO = null;

            GO = m_att.Teams[info.mTeam].Spawner.SpawnPlayer(info, stationID);
            m_att.Teams[info.mTeam].AddPlayerObject(GO.GetComponent<NetworkIdentity>().netId);

            GO.GetComponent<ShipGenerator>()
                .AssignShipData(ship, info.mTeam);
        }

        /// <summary>
        /// Simply forwards the NPC spawn to the 
        /// correct spawning object
        /// manager
        /// </summary>
        [Server]
        public void SpawnNpc
            (int playerID, uint targetID, uint spawnID, 
            string agent, Vector2 location, uint homeID)
        {
            GameObject spawn = ClientScene.FindLocalObject(new NetworkInstanceId(spawnID));

            if (spawn == null)
                return;

            spawn.GetComponent<SpawnManager>().
                SpawnShip(playerID, targetID, spawnID, location, agent, homeID);           
        }

        /// <summary>
        /// Sends station information to 
        /// game builder and assigns station to team
        /// </summary>
        /// <param name="prefabIndex"></param>
        /// <param name="teamID"></param>
        /// <param name="position"></param>
        [Server]
        public void BuildStation(string prefabName, int teamID, Vector3 position)
        {
            // add station to the IDed team
            m_att.Builder.GenerateStation(m_att.Teams[teamID],
                prefabName, position);
        }

        #endregion

        #region DAMAGE

        [Server]
        public void ShipHit(ShipColliderHitMessage hitMsg)
        {
            foreach(PlayerConnectionInfo info in m_att.PlayerInfoList)
            {
                NetworkServer.SendToClient(info.mConnection.connectionId,
                    (short)MSGCHANNEL.PROCESSSHIPHIT, hitMsg);
            }
        }

        [Server]
        public void ObjectHit(SOColliderHitMessage hitMsg)
        {
            foreach (PlayerConnectionInfo info in m_att.PlayerInfoList)
            {
                NetworkServer.SendToClient(info.mConnection.connectionId,
                    (short)MSGCHANNEL.PROCESSOBJECTHIT, hitMsg);
            }
        }

        [Server]
        public void OnIntegrityChanged(IntegrityChangedMsg intMsg)
        {
            foreach (PlayerConnectionInfo info in m_att.PlayerInfoList)
            {
                if(info.ID != intMsg.PlayerID)
                {
                    // Message would be sent to client here
                    NetworkServer.SendToClient(info.mConnection.connectionId,
                    (short)MSGCHANNEL.DISPLAYINTEGRITYCHANGE, intMsg);
                }
            }
        }

        #endregion

        /// <summary>
        /// Applies costs and additions
        /// to specified team
        /// </summary>
        /// <param name="tMsg"></param>
        [Server]
        public void OnTransaction(TransactionMessage tMsg)
        {
            if (tMsg.AssetDir == -1)
                m_att.Teams[tMsg.Recipiant].Expend(tMsg.Assets);
            if (tMsg.CurrencyDir == -1)
                m_att.Teams[tMsg.Recipiant].Expend(tMsg.CurrencyAmount);
            else if (tMsg.CurrencyDir == 1)
                m_att.Teams[tMsg.Recipiant].Deposit(tMsg.CurrencyAmount);
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
            if (m_att.PlayerInfoList == null)
                return null;

            PlayerConnectionInfo pInfo =
                m_att.PlayerInfoList.Item(playerID);

            return pInfo.mConnection;
        }

        #endregion

        #endregion

        #region PRIVATE UTILITIES

        private void InitializePlayer(PlayerConnectionInfo info)
        {
            // prompt player to pick team
            // Send this to single client via a message
            TeamPickerMessage msg = new TeamPickerMessage();
            msg.teamOne = m_att.Teams[0].ID;
            msg.teamTwo = m_att.Teams[1].ID;
            NetworkServer.SendToClient(info.mConnection.connectionId,
                (short)MSGCHANNEL.TEAMPICKER, msg);
        }

        private void InitializeTeam(string GOName, int id, FactionData data)
        {
            m_att.Teams.Add(GameObject.Find(GOName).GetComponent<TeamController>());
            m_att.Teams[id].Initialize(data, id);

            m_access.AddTeam(m_att.Teams[id].netId.Value);
        }

        /// <summary>
        /// client taken from client to server
        /// sends data tp lobby
        /// </summary>
        /// <param name="Player"></param>
        [Server]
        public void InitializeLobbyPlayer(PlayerConnectionInfo info)
        {
            // Get the connection info required for
            // spawning with player authority
            // Send message on server to lobby controller
            SystemManager.Server.AddPlayerToLobby(info.mConnection);
        }

        #endregion
    }
}

