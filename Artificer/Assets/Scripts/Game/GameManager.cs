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
        private GameAttributes _att;

        #endregion

        #region PUBLIC INTERACTION

        #region SYSTEM MESSAGES

        /// <summary>
        /// Does nothing atm
        /// </summary>
        [Server]
        public void Initialize()
        {

        }

        /// <summary>
        /// Build the Lobby scene for the game
        /// </summary>
        [Server]
        public void InitializeLobbyScene()
        {
            _att.CurrentState = GameState.Lobby;
        }

        /// <summary>
        /// Retreives game parameters and 
        /// initializes the gameplay logic
        /// </summary>
        [Server]
        public void InitializeSpaceScene()
        {
            // Change the state on the game
            _att.CurrentState = GameState.Play;

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
            _att.TeamA = GameObject.Find("Team_A").GetComponent<TeamController>();
            _att.TeamB = GameObject.Find("Team_B").GetComponent<TeamController>();

            _att.TeamA.Initialize(teamAcon, 0);
            _att.TeamB.Initialize(teamBcon, 1);

            // Generated stations for the teams
            _att.Builder.GenerateStations(_att.TeamA, _att.TeamB);
        }

        [Server]
        public void AddNewPlayer
            (short playerControllerId, NetworkConnection conn)
        {
            if (_att.PlayerInfoList == null)
                _att.PlayerInfoList = new IndexedList<PlayerConnectionInfo>();

            // Test If this player is already joined to the match
            PlayerConnectionInfo info = _att.PlayerInfoList
                .FirstOrDefault(o => o.mConnection == conn);

            // if not match then create new info
            if (info == null)
            {
                info = new PlayerConnectionInfo();
                info.mController = playerControllerId;
                info.mConnection = conn;
                info.mTeam = -1;

                // add player to tracking list
                _att.PlayerInfoList.Add(info);
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
            if (_att.CurrentState == GameState.Play)
                InitializePlayer(info);
            else if (_att.CurrentState == GameState.Lobby)
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
            if (_att.PlayerInfoList == null)
                return;

            // Test If this player is already joined to the match
            PlayerConnectionInfo info = _att.PlayerInfoList
                .FirstOrDefault(o => o.mConnection == conn);

            if (info != null)
            {
                _att.PlayerInfoList.Remove(info);
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
            if (_att.PlayerInfoList == null)
                return;

            PlayerConnectionInfo pInfo = 
                _att.PlayerInfoList.Item(playerID);

            // network instance ID of our team object
            NetworkInstanceId teamNetID;

            if (pInfo == null)
            {
                Debug.Log("Error: Game Controller - Assign To Team: Player not found");
                return;             // something is wrong
            }

            // Assign that player to the team
            pInfo.mTeam = TeamID;

            // Pass that player team to an instance ID
            if (TeamID == 0)
                teamNetID = _att.TeamA.netId;
            else
                teamNetID = _att.TeamB.netId;

            // Create netID message
            NetMsgMessage netMsg = new NetMsgMessage();

            // assign team netID to message
            netMsg.SelfID = teamNetID;

            // Send message for clients space obj
            NetworkServer.SendToClient(pInfo.mConnection.connectionId,
                (short)MSGCHANNEL.ASSIGNTEAM, netMsg);
        }

        [Server]
        public void SpawnPlayer(int playerID, int stationID, string shipName)
        {
            PlayerConnectionInfo info = 
                _att.PlayerInfoList.Item(playerID);

            GameObject GO = null;

            // Spawn player using correct team
            if (info.mTeam == 0)
            {
                GO = _att.TeamA.Spawner.SpawnPlayer(info, stationID);
                _att.TeamA.AddPlayerObject(GO.GetComponent<NetworkIdentity>().netId);
            }
            else
            {
                GO = _att.TeamB.Spawner.SpawnPlayer(info, stationID);
                _att.TeamB.AddPlayerObject(GO.GetComponent<NetworkIdentity>().netId);
            }

            GO.GetComponent<ShipInitializer>()
                .AssignShipData(shipName, info.mTeam);
        }

        /*
        [Server]
        public void SpawnAI(int playerID, AgentData agent, Vector2 spawnPoint)
        {
            PlayerConnectionInfo info =
                _att.PlayerInfoList.Item(playerID);

            GameObject Prefab = SystemManager.singleton.playerPrefab;

            GameObject GO = Instantiate(Prefab, spawnPoint, Quaternion.identity) as GameObject;

            // Projectile can run command to display self
            NetworkServer.SpawnWithClientAuthority(GO, info.mConnection);

            // assign ship info
            // e.g. ship name 
            StringMessage sMsg = new StringMessage(agent.Ship);
            NetworkServer.SendToClient(info.mConnection.connectionId,
                (short)MSGCHANNEL.SPAWNME, sMsg);
        }*/

        [Server]
        public void BuildProjectile(int prefabIndex, int playerID, Vector3 position, WeaponData wData)
        {
            PlayerConnectionInfo info =
                _att.PlayerInfoList.Item(playerID);

            GameObject Prefab = NetworkManager.singleton.spawnPrefabs[prefabIndex];

            GameObject GO = Instantiate(Prefab, position, Quaternion.identity) as GameObject;

            // Projectile can run command to display self
            NetworkServer.SpawnWithClientAuthority(GO, info.mConnection);

            ProjectileSpawnedMessage spwnMsg = new ProjectileSpawnedMessage();
            spwnMsg.WData = wData;
            spwnMsg.Projectile = GO.GetComponent<NetworkIdentity>().netId;

            NetworkServer.SendToClient(info.mConnection.connectionId, 
                (short)MSGCHANNEL.CREATEPROJECTILE, spwnMsg);
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
                selectedTeam = _att.TeamA;
            else
                selectedTeam = _att.TeamB;

            // send message to builder
            _att.Builder.GenerateStation(selectedTeam,
                prefabName, position);
        }

        [Server]
        public void ShipHit(ShipColliderHitMessage hitMsg)
        {
            foreach(PlayerConnectionInfo info in _att.PlayerInfoList)
            {
                NetworkServer.SendToClient(info.mConnection.connectionId,
                    (short)MSGCHANNEL.PROCESSSHIPHIT, hitMsg);
            }
        }

        [Server]
        public void ObjectHit(SOColliderHitMessage hitMsg)
        {
            foreach (PlayerConnectionInfo info in _att.PlayerInfoList)
            {
                NetworkServer.SendToClient(info.mConnection.connectionId,
                    (short)MSGCHANNEL.PROCESSOBJECTHIT, hitMsg);
            }
        }

        [Server]
        public void OnIntegrityChanged(IntegrityChangedMsg intMsg)
        {
            foreach (PlayerConnectionInfo info in _att.PlayerInfoList)
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
            msg.teamOne = _att.TeamA.ID;
            msg.teamTwo = _att.TeamB.ID;
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

