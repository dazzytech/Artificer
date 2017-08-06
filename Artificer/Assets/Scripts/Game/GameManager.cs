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

namespace Game
{
    public enum GameState{Lobby, Play}

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

        #endregion

        #region PUBLIC INTERACTION

        #region SYSTEM MESSAGES

        /// <summary>
        /// Takes the game parameters and
        /// initializes the game
        /// </summary>
        [Server]
        public void Initialize(GameParameters param)
        {
            // Assign the spawnable ships for each team
            foreach(ShipSpawnData spawn in param.SpawnableShips)
            {
                m_att.TeamA.AddSpawnableShip(spawn);
                m_att.TeamB.AddSpawnableShip(spawn);
            }

            m_att.TeamA.DefineTeamAssets(param.Wallet);
            m_att.TeamB.DefineTeamAssets(param.Wallet);
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

            // Retieve Team Items
            m_att.TeamA = GameObject.Find("Team_A").GetComponent<TeamController>();
            m_att.TeamB = GameObject.Find("Team_B").GetComponent<TeamController>();

            m_att.TeamA.Initialize(teamAcon, 0);
            m_att.TeamB.Initialize(teamBcon, 1);

            // Generated stations for the teams
            m_att.Builder.GenerateStations(m_att.TeamA, m_att.TeamB);

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

            // Pass that player team to an instance ID
            if (TeamID == 0)
            {
                teamNetID = m_att.TeamA.netId;
                starterFund = m_att.TeamA.FundPlayer(1000000);
            }
            else
            {
                teamNetID = m_att.TeamB.netId;
                starterFund = m_att.TeamB.FundPlayer(1000000);
            }

            // Create netID message
            IntegerMessage netMsg = new IntegerMessage((int)teamNetID.Value);

            // Create Transaction fund
            TransactionMessage transaction
                = new TransactionMessage();

            transaction.Recipiant = -1;
            transaction.CurrencyAmount = starterFund;

            // Send message for clients space obj
            NetworkServer.SendToClient(pInfo.mConnection.connectionId,
                (short)MSGCHANNEL.ASSIGNTEAM, netMsg);

            // Send message for clients space obj
            NetworkServer.SendToClient(pInfo.mConnection.connectionId,
                (short)MSGCHANNEL.TRANSACTIONCLIENT, transaction);
        }

        [Server]
        public void SpawnPlayer(int playerID, int stationID, ShipData ship)
        {
            PlayerConnectionInfo info = 
                m_att.PlayerInfoList.Item(playerID);

            GameObject GO = null;

            // Spawn player using correct team
            if (info.mTeam == 0)
            {
                GO = m_att.TeamA.Spawner.SpawnPlayer(info, stationID);
                m_att.TeamA.AddPlayerObject(GO.GetComponent<NetworkIdentity>().netId);
            }
            else
            {
                GO = m_att.TeamB.Spawner.SpawnPlayer(info, stationID);
                m_att.TeamB.AddPlayerObject(GO.GetComponent<NetworkIdentity>().netId);
            }

            GO.GetComponent<ShipGenerator>()
                .AssignShipData(ship, info.mTeam);
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
            // Retreive selected team
            TeamController selectedTeam;

            if(teamID == 0)
                selectedTeam = m_att.TeamA;
            else
                selectedTeam = m_att.TeamB;

            // send message to builder
            m_att.Builder.GenerateStation(selectedTeam,
                prefabName, position);
        }

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

        #endregion

        #region PRIVATE UTILITIES

        private void InitializePlayer(PlayerConnectionInfo info)
        {
            // prompt player to pick team
            // Send this to single client via a message
            TeamPickerMessage msg = new TeamPickerMessage();
            msg.teamOne = m_att.TeamA.ID;
            msg.teamTwo = m_att.TeamB.ID;
            NetworkServer.SendToClient(info.mConnection.connectionId,
                (short)MSGCHANNEL.TEAMPICKER, msg);
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

