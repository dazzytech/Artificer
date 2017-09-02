using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Space.Ship.Components.Attributes;
using Networking;
using Space.Segment;
using Data.UI;
using Data.Space;

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
                SystemManager.UIMsg.DisplayPrompt("Not in build out of range of FOB or Home Base");
                SystemManager.UI.Invoke("ClearPrompt", 3f);
                return;
            }

            WalletData temp = SystemManager.Wallet;

            if (!temp.Withdraw(station.Cost))
            {
                return;
            }

            // retrive a spawn point away from ship using scalar
            float distance = Random.Range(3f, 5f) * 
                Mathf.Sign(Random.Range(-1f,1f));

            Vector3 deployPoint = transform.position;
            deployPoint.x += distance;
            deployPoint.y += distance;

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
