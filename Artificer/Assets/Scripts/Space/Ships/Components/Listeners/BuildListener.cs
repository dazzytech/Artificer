using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Space.Ship.Components.Attributes;
using Networking;
using Space.Segment;
using Data.UI;
using Data.Space;
using Stations;

namespace Space.Ship.Components.Listener
{
    public delegate void Deploy(int deployID);

    public class BuildListener : ComponentListener
    {
        #region ATTRIBUTES

        BuildAttributes m_att;

        #endregion

        #region PUBLIC INTERACTION  

        /// <summary>
        /// When activated triggers the deploy
        /// animation and sends to server when finished
        /// </summary>
        public override void Activate()
        {
            if (!m_att.ReadyToDeploy)
                return;

            // Display info on this component to wheel
            SystemManager.UI.DisplayBuildWheel(DeployStation,
                m_att.SpawnableStations);
        }

        public void SetTriggerKey(string key)
        {
            m_att.TriggerKey = Control_Config
                .GetKey(key, "ship");
        }

        public void SetCombatKey(string key)
        {
            m_att.CombatKey = Control_Config
                .GetKey(key, "combat");
        }

        public void DeployStation(int stationID)
        {
            // retreive name and check can deploy
            DeployData station = 
                m_att.SpawnableStations[stationID];

            if (!SystemManager.Space.CanBuild && !station.Name.Contains("FOB"))
            {
                SystemManager.UIPrompt.DisplayPrompt("Not in build out of range of FOB or Home Base", 3f);
                return;
            }


            // Attempt to find placement position
            // quit and prompt if unsuccessful
            // retrive a spawn point away from ship using scalar
            Vector2 deployPoint = GetPoint(station.Radius);

            if(deployPoint == Vector2.zero)
            {
                SystemManager.UIPrompt.DisplayPrompt
                    ("There is no room to place station", 3f);
                return;
            }

            WalletData temp = SystemManager.Wallet;

            if (!temp.Withdraw(station.Cost))
            {
                SystemManager.UIPrompt.DisplayPrompt
                    ("Insufficient funds to deploy station", 3f);
                return;
            }

            SystemManager.Wallet = temp;

            // Create message for game controller
            StationBuildMessage sbm = new StationBuildMessage();
            sbm.Position = deployPoint;
            sbm.teamID = SystemManager.Space.Team.ID;
            sbm.PrefabName = station.Name; 

                // send to server
            SystemManager.singleton.client.Send((short)MSGCHANNEL.BUILDSTATION, sbm);

            m_att.ReadyToDeploy = false;

            StartCoroutine("EngageDelay");
        }

        #endregion

        #region PRIVATE UTILITIES

        protected override void InitializeComponent()
        {
            base.InitializeComponent();

            ComponentType = "Builder";
            m_att = GetComponent<BuildAttributes>();

            if(hasAuthority)
                m_att.ReadyToDeploy = true;
        }

        private Vector2 GetPoint(float radius)
        {
            Vector2 returnPoint = Vector2.zero;

            int attempts = 10;
            while(attempts-- > 0)
            {
                returnPoint = Math.RandomWithinRange
                    (transform.position, m_att.MinRange, m_att.MaxRange);

                if (!ConflictingRange(returnPoint, radius))
                    break;
                else
                    returnPoint = Vector2.zero;
            }

            return returnPoint;
        }

        /// <summary>
        /// Returns if there are any stations
        /// that are within conflicting range
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private bool ConflictingRange(Vector2 position, float radius)
        {
            foreach(StationAccessor station in SystemManager.Accessor.GlobalStations)
            {
                if (Math.WithinRange(position, station.transform.position,
                    0, station.Radius + radius))
                    return true;
            }

            return false;
        }

        #endregion

        #region COROUTINE

        private IEnumerator EngageDelay()
        {
            yield return new WaitForSeconds(m_att.StationDelay);
            m_att.ReadyToDeploy = true;
            yield return null;
        }

        #endregion
    }
}
