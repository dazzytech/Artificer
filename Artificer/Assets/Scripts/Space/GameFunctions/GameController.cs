using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;
using System;
using System.Collections.Generic;
using Data.Space;
using Data.Space.Library;
using Space.Ship;
using Space.UI;
using Space.Teams;
using Space.Ship.Components.Listener;
using Space.Projectiles;
using Networking;

namespace Space.GameFunctions
{
    public enum GameState{Completed, Failed}

    /// <summary>
    /// Responsible for game elements such as 
    /// teams, players etc
    /// </summary>
    public class GameController:NetworkBehaviour
    {
        #region EVENTS

        public delegate void ChangeGameState(GameState newState);
        public event ChangeGameState OnChangeState;

        #endregion

        #region ATTRIBUTES

        private GameAttributes _att;

        #endregion

        #region NETWORK BEHAVIOUR

        #endregion

        #region MONO BEHAVIOUR

        void Awake()
        {
            _att = GetComponent<GameAttributes>();

            _att.Builder = GetComponent<GameBuilder>();
        }

        /*
        ContractData Contract;
        public ContractBuilder Builder;
        public ContractState ContractStatus;
        public RewardInfo Rewards;

        // Mission Tracker List
        public List<MissionData> PrimaryTracker;
        public List<MissionData> SecondaryTracker;
        public List<MissionData> Ended;*/

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Retreives game parameters and 
        /// initializes the gameplay logic
        /// </summary>
        [Server]
        public void Initialize(/*GameParameters param*/)
        {
            // Move parameters from param to member variables

            // Initialize Teams
            bool teamsCompleted = false;

            //UnityEngine.Random.seed = DateTime.Now.Millisecond;

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

            // Team objects should already be assigned
            _att.TeamA.Initialize(teamAcon);
            _att.TeamB.Initialize(teamBcon);

            // Generated stations for the teams
            _att.Builder.GenerateStations(_att.TeamA, _att.TeamB);


            // Initialize trackers
            /*
            PrimaryTracker = new List<MissionData>();
            SecondaryTracker = new List<MissionData>();
            Ended = new List<MissionData>();

            Contract = param.Contract;

            ContractStatus = ContractState.Pending;

            Builder.InitializeSpawners();

            // Build space objects
            Builder.GenerateContractObjects(Contract);

            GameObject.Find("_gui").
                SendMessage("BuildContractData", Contract);

            foreach (MissionData mission in Contract.PrimaryMissions)
            {
                mission.Begin();
                PrimaryTracker.Add(mission);
            }

            foreach (MissionData mission in Contract.OptionalMissions)
            {
                mission.Begin();
                SecondaryTracker.Add(mission);
            }*/
        }


        [Server]
        public void AddNewPlayer
            (short playerControllerId, NetworkConnection conn)
        {
            if (_att.PlayerInfoList == null)
                _att.PlayerInfoList = new List<PlayerConnectionInfo>();

            // store connection to the ship
            PlayerConnectionInfo info = new PlayerConnectionInfo();
            info.mID = _att.PlayerInfoList.Count;
            info.mController = playerControllerId;
            info.mConnection = conn;
            info.mTeam = -1;

            // add player to tracking list
            _att.PlayerInfoList.Add(info);

            // Assign our intial ID to the system
            IntegerMessage iMsg = new IntegerMessage(info.mID);
            NetworkServer.SendToClient(conn.connectionId,
                (short)MSGCHANNEL.NEWID, iMsg);

            // prompt player to pick team
            // Send this to single client via a message
            TeamPickerMessage msg = new TeamPickerMessage();
            msg.teamOne = _att.TeamA.ID;
            msg.teamTwo = _att.TeamB.ID;
            NetworkServer.SendToClient(conn.connectionId,
                (short)MSGCHANNEL.TEAMPICKER, msg);
        }

        /// <summary>
        /// Player has picked a team
        /// store that team with our info
        /// </summary>
        /// <param name="TeamID"></param>
        /// <param name="conn"></param>
        [Server]
        public void AssignToTeam(int TeamID, int playerID)
        {
            PlayerConnectionInfo pInfo = GetPlayer(playerID);

            if (pInfo == null)
            {
                Debug.Log("Error: Game Controller - Assign To Team: Player not found");
                return;             // something is wrong
            }

            // Assign that player to the team
            pInfo.mTeam = TeamID;
        }

        [Server]
        public void SpawnPlayer(int playerID, int stationID, int shipID)
        {
            PlayerConnectionInfo info = GetPlayer(playerID);
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
            PlayerConnectionInfo info = GetPlayer(playerID);

            GameObject Prefab = NetworkManager.singleton.spawnPrefabs[prefabIndex];

            GameObject GO = Instantiate(Prefab, position, Quaternion.identity) as GameObject;

            // Projectile can run command to display self
            NetworkServer.SpawnWithClientAuthority(GO, info.mConnection);

            GO.transform.GetComponent<WeaponController>().Init();

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

        #endregion

        #region UTILITIES

        /// <summary>
        /// Utility that returns the player via ID
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        private PlayerConnectionInfo GetPlayer(int playerID)
        {
            // Find the connection that assigned to team
            foreach (PlayerConnectionInfo info in _att.PlayerInfoList)
            {
                if (info.mID.Equals(playerID))
                {
                    return info;
                }
            }

            return null;
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
                    /*if(GameManager.GetPlayer.Completedlevels == null)
                        GameManager.GetPlayer.Completedlevels = new List<int>();

                    // test contract is not already completed
                    if(!GameManager.GetPlayer.Completedlevels.Contains(Contract.ID))
                    {
                        GameManager.GetPlayer.Completedlevels.Add(Contract.ID);
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
                                    GameManager.GetPlayer.AddMaterial(mission.Reward.Materials);

                                // Transfer components
                                if(mission.Reward.Components != null)
                                    GameManager.GetPlayer.AddComponent(mission.Reward.Components);

                                GameManager.GetPlayer.AddXP(mission.Reward.Xp);
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

