using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;
using System.Collections.Generic;
using Data.Space;
using Data.Space.Library;
using Space.Ship;
using Space.UI;
using Space.Teams;

namespace Space.GameFunctions
{
    public enum GameState{Completed, Failed}

    #region NETWORK MESSAGE OBJECTS 

    /// <summary>
    /// Message containig the IDs of 
    /// both teams for the client
    /// </summary>
    public class TeamPickerMessage: MessageBase
    {
        public int teamOne;
        public int teamTwo;
    }

    #endregion

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

            _att.Builder.GenerateSpawners();

            // Initialize Teams
            bool teamsCompleted = false;

            // pick our two teams first
            FactionData teamAcon = FactionLibrary.ReturnFaction(Random.Range(0, 3));

            FactionData teamBcon = new FactionData();

            // we dont want two of the same teams
            while(!teamsCompleted)
            {
                teamBcon = FactionLibrary.ReturnFaction(Random.Range(0, 3));
                if (!teamAcon.Equals(teamBcon))
                    teamsCompleted = true;
            }

            // Team objects should already be assigned
            _att.TeamA.Initialize(teamAcon);
            _att.TeamB.Initialize(teamBcon);

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
            info.mSpawned = false;

            // add player to tracking list
            _att.PlayerInfoList.Add(info);

            // Assign our intial ID to the system
            IntegerMessage iMsg = new IntegerMessage(info.mID);
            NetworkServer.SendToClient(conn.connectionId, MsgType.Highest + 6, iMsg);

            // prompt player to pick team
            // Send this to single client via a message
            TeamPickerMessage msg = new TeamPickerMessage();
            msg.teamOne = _att.TeamA.ID;
            msg.teamTwo = _att.TeamB.ID;
            NetworkServer.SendToClient(conn.connectionId, MsgType.Highest + 5, msg);
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

            if (info.mTeam == 0)
            {
                // spawn with team A
                // add station id in future
                GO = _att.TeamA.Spawner.SpawnPlayer(info);
            }
            else
            {
                GO = _att.TeamB.Spawner.SpawnPlayer(info);
            }

            // assign ship info
            // e.g. ship name 
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

