using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Data.Shared;

using Space.UI;

/// <summary>
/// Mission tracker.
/// Set by the space manager and updates
/// missions with the player update listener
/// </summary>
/// 
namespace Space.GameFunctions
{
    public enum GameState{Running, Completed, Failed}

    public class GameStateTracker: MonoBehaviour
    {
        #region ATTRIBUTES

        // Store builder object for generating game object
        private GameBuilder _builder;

        #endregion

        /*
        ContractData Contract;
        public ContractBuilder Builder;
        public ContractState ContractStatus;
        public RewardInfo Rewards;

        // Mission Tracker List
        public List<MissionData> PrimaryTracker;
        public List<MissionData> SecondaryTracker;
        public List<MissionData> Ended;*/

        #region PUBLIC INTERACTION

        public void Initialize(/*GameParameters param*/)
        {
            // Move parameters from param to member variables


            // Firstly for now build the game spawnpoints
            _builder.GenerateSpawners();

            // Initialize Teams

            // Deal with GUI

            // Initialize trackers
            /*PrimaryTracker = new List<MissionData>();
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

        #endregion

        #region EVENT LISTENERS

        /*
        public void ProcessShipDestroyed(DestroyDespatch destroyed)
        {
            foreach (MissionData mission in PrimaryTracker)
            {
                mission.AddShipKilled(destroyed);
            }
            foreach (MissionData mission in SecondaryTracker)
            {
                mission.AddShipKilled(destroyed);
            }
        }

        public void ProcessStationReached(Transform ship)
        {
            foreach (MissionData mission in PrimaryTracker)
            {
                mission.StationEntered(ship);
            }
            foreach (MissionData mission in SecondaryTracker)
            {
                mission.StationEntered(ship);
            }
        }

        public void ProcessMaterials(Dictionary<MaterialData, float> newMat)
        {
            foreach (MissionData mission in PrimaryTracker)
            {
                mission.AddMaterial(newMat);
            }
            foreach (MissionData mission in SecondaryTracker)
            {
                mission.AddMaterial(newMat);
            }
        }*/

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

