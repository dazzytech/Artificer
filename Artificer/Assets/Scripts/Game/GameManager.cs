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

        private static GameManager m_singleton;

        #endregion

        #region NETWORK BEHAVIOUR

        #endregion


        #region PUBLIC INTERACTION

        public bool Build()
        {
            if (m_singleton == null)
                m_singleton = this;
            else
            {
                return false;
            }

            return true;
        }

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
            _att.currentState = GameState.Lobby;
        }

        /// <summary>
        /// Retreives game parameters and 
        /// initializes the gameplay logic
        /// </summary>
        [Server]
        public void InitializeSpaceScene()
        {
            // Change the state on the game
            _att.currentState = GameState.Play;

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

            _att.TeamA.Initialize(teamAcon);
            _att.TeamB.Initialize(teamBcon);

            // Generated stations for the teams
            _att.Builder.GenerateStations(_att.TeamA, _att.TeamB);

            if(_att.PlayerInfoList != null)
                // go through each connected player and send messages
                foreach (PlayerConnectionInfo info in _att.PlayerInfoList)
                    InitializePlayer(info);
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
            if (_att.currentState == GameState.Play)
                InitializePlayer(info);
            else if (_att.currentState == GameState.Lobby)
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
        public void SpawnPlayer(int playerID, int stationID, int shipID)
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

            // assign ship info
            // e.g. ship name 
            StringMessage sMsg = new StringMessage("");
            NetworkServer.SendToClient(info.mConnection.connectionId, 
                (short)MSGCHANNEL.SPAWNME, sMsg);
        }

        [Server]
        public void BuildProjectile(int prefabIndex, int playerID, Vector3 position, WeaponData wData)
        {
            PlayerConnectionInfo info =
                _att.PlayerInfoList.Item(playerID);

            GameObject Prefab = NetworkManager.singleton.spawnPrefabs[prefabIndex];

            GameObject GO = Instantiate(Prefab, position, Quaternion.identity) as GameObject;

            // Projectile can run command to display self
            NetworkServer.SpawnWithClientAuthority(GO, info.mConnection);

            // currently down nothing..
            //GO.transform.GetComponent<WeaponController>().Init();

            ProjectileSpawnedMessage spwnMsg = new ProjectileSpawnedMessage();
            spwnMsg.WData = wData;
            spwnMsg.Projectile = GO.GetComponent<NetworkIdentity>().netId;

            NetworkServer.SendToClient(info.mConnection.connectionId, 
                (short)MSGCHANNEL.CREATEPROJECTILE, spwnMsg);
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

        /*
        public void RunUpdate()
        {
            if (Contract != null)
            {
                bool CycleCompletion = true;
                bool CycleFailure = false;

                foreach (MissionData mission in PrimaryTracker)
                {
                    mission.UpdateStatus();
                    if (!mission.Success)
                    {
                        CycleCompletion = false;
                    }
                    else
                    {
                        // Display success messege and remove item
                        MessageHUD.DisplayMessege(new MsgParam("md-green", string.Format("Primary Mission Completed: {0}", mission.Title)));
                        Ended.Add(mission);
                    }
                    if (mission.Failure)
                    {
                        CycleFailure = true;
                        MessageHUD.DisplayMessege(new MsgParam("md-red", string.Format("Primary Mission Failed: {0}", mission.Title)));
                    }
                }

                foreach (MissionData mission in SecondaryTracker)
                {
                    mission.UpdateStatus();
                    if (mission.Success)
                    {
                        // Display success messege and remove item
                        MessageHUD.DisplayMessege(new MsgParam("md-yellow", string.Format("Optional Mission Completed: {0}", mission.Title)));
                        Ended.Add(mission);
                    }
                    if (mission.Failure)
                    {
                        MessageHUD.DisplayMessege(new MsgParam("md-yellow", string.Format("Optional Mission Failed: {0}", mission.Title)));
                        Ended.Add(mission);
                    }
                }

                foreach(MissionData mission in Ended)
                {
                    if(PrimaryTracker.Contains(mission))
                        PrimaryTracker.Remove(mission);
                }

                foreach(MissionData mission in Ended)
                {
                    if(SecondaryTracker.Contains(mission))
                        SecondaryTracker.Remove(mission);
                }

                if (CycleFailure)
                    ContractStatus = ContractState.Failed;
                else if (CycleCompletion)
                {
                    // Test that contract list exists
                    /*if(SystemManager.GetPlayer.Completedlevels == null)
                        SystemManager.GetPlayer.Completedlevels = new List<int>();

                    // test contract is not already completed
                    if(!SystemManager.GetPlayer.Completedlevels.Contains(Contract.ID))
                    {
                        SystemManager.GetPlayer.Completedlevels.Add(Contract.ID);
                    }

                    StoreRewards();

                    // cycle through each ended mission and transfer rewards to player
                    foreach(MissionData mission in Ended)
                    {
                        if(mission.Success)
                        {
                            if(mission.Reward != null)
                            {
                                // Transfer materials
                                if(mission.Reward.Materials != null)
                                    SystemManager.GetPlayer.AddMaterial(mission.Reward.Materials);

                                // Transfer components
                                if(mission.Reward.Components != null)
                                    SystemManager.GetPlayer.AddComponent(mission.Reward.Components);

                                SystemManager.GetPlayer.AddXP(mission.Reward.Xp);
                            }
                        }
                    }
                    

                    ContractStatus = ContractState.Completed;
                }
            }
        }

        public void StoreRewards()
        {
            Rewards = new RewardInfo();
            // cycle through each ended mission and transfer rewards to player
            foreach (MissionData mission in Ended)
            {
                if (mission.Success)
                {
                    if (mission.Reward != null)
                    {
                        Rewards.Xp += mission.Reward.Xp;

                        foreach(MaterialData key in mission.Reward.Materials.Keys)
                        {
                            if(Rewards.Materials.ContainsKey(key))
                                Rewards.Materials[key] += mission.Reward.Materials[key];
                            else
                                Rewards.Materials.Add(key, mission.Reward.Materials[key]);
                        }

                        foreach(string comp in mission.Reward.Components)
                        {
                            if(!Rewards.Components.Contains(comp))
                            {
                                Rewards.Components.Add(comp);
                            }
                        }
                    }
                }
            }
        }*/
    }
}

