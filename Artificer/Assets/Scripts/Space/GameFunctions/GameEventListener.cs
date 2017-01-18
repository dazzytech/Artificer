﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

using Space.Ship;

namespace Space.GameFunctions
{
    public class GameEventListener : NetworkBehaviour
    {
        #region ATTRIBUTES

        private GameAttributes m_att;

        #endregion

        #region MONO BEHAVIOUR

        void OnEnable()
        {
            // Assign Events
            GameServerEvents.OnShipDestroyed += ProcessShipDestroyed;
            GameServerEvents.OnStationDestroyed += ProcessStationDestroyed;
        }

        void OnDisable()
        {
            // De-assign events
            GameServerEvents.OnShipDestroyed -= ProcessShipDestroyed;
            GameServerEvents.OnStationDestroyed -= ProcessStationDestroyed;
        }

        // Use this for initialization
        void Awake()
        {
            m_att = GetComponent<GameAttributes>();
        }

        #endregion

        #region EVENT LISTENERS

        public void ProcessShipDestroyed(DestroyDespatch destroyed)
        {
            PlayerConnectionInfo info = m_att.PlayerInfoList.Item(destroyed.MiscID);

            if (info.mTeam == 0)
            {
                m_att.TeamA.RemovePlayerObject(destroyed.Self);
            }
            else
            {
                m_att.TeamB.RemovePlayerObject(destroyed.Self);
            }

            /*foreach (MissionData mission in PrimaryTracker)
                {
                    mission.AddShipKilled(destroyed);
                }
                foreach (MissionData mission in SecondaryTracker)
                {
                    mission.AddShipKilled(destroyed);
                }*/
        }

        public void ProcessStationDestroyed(DestroyDespatch destroyed)
        {

        }

        /*
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
    }
}
