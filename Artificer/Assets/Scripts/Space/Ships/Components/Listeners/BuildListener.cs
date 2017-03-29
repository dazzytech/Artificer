using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Space.Ship.Components.Attributes;
using Networking;
using Space.Segment;

namespace Space.Ship.Components.Listener
{
    public class BuildListener : ComponentListener
    {
        #region ATTRIBUTES

        BuildAttributes m_att;

        #endregion

        #region MONOBEHAVIOUR

        void Awake()
        {
            ComponentType = "Builder";
            m_att = GetComponent<BuildAttributes>();
        }

        void Start()
        {
            base.SetRB();
            m_att.StationDeployed = false;
        }

        #endregion

        #region PUBLIC INTERACTION  

        /// <summary>
        /// When activated triggers the deploy
        /// animation and sends to server when finished
        /// </summary>
        public override void Activate()
        {
            // For now send desploy immediately
            DeployStation();

            // self destruct this piece
            HitData hit = new HitData();
            hit.hitComponent = m_att.ID;

            transform.parent.SendMessage("DestroyComponent", hit,
                    SendMessageOptions.DontRequireReceiver);
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

        #endregion

        #region PRIVATE UTILITIES

        private void DeployStation()
        {
            Vector3 deployPoint = transform.position;

            // Test our prefab is contained with the 
            // spawn objects
            if(NetworkManager.singleton.spawnPrefabs.
                Contains(m_att.StationPrefab))
            {
                // Create message for game controller
                StationBuildMessage sbm = new StationBuildMessage();
                sbm.Position = deployPoint;
                sbm.teamID = SystemManager.Space.Team.ID;
                sbm.PrefabName = m_att.StationPrefab.name;

                // send to server
                SystemManager.singleton.client.Send((short)MSGCHANNEL.BUILDSTATION, sbm);
            }
            else
            {
                Debug.Log("Error: Build Listener - Deploy Station: Prefab not added to spawn list.");
            }
        }

        #endregion
    }
}
