using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

using Space.Ship;

namespace Game
{
    public class GameEventListener : NetworkBehaviour
    {
        #region ATTRIBUTES

        private GameAttributes m_att;

        #endregion

        #region MONO BEHAVIOUR

        void OnDisable()
        {
            // De-assign events
           // SystemManager.Events.EventShipDestroyed -= ProcessShipDestroyed;
            //SystemManager.Events.EventStationDestroyed -= ProcessStationDestroyed;
        }

        // Use this for initialization
        void Awake()
        {
            m_att = GetComponent<GameAttributes>();
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Assigns server side event listeners
        /// </summary>
        public void InitSpaceScene()
        {
            if (SystemManager.Events == null)
                return;

            SystemManager.Events.EventShipDestroyed += ProcessShipDestroyed;
            SystemManager.Events.EventStationDestroyed += ProcessStationDestroyed;

            SystemManager.Events.EventShipCreated += ProcessShipCreated;
        }

        #endregion

        #region EVENT LISTENERS

        public void ProcessShipCreated(CreateDispatch CD)
        {
            PlayerConnectionInfo info = m_att.PlayerInfoList.Item(CD.PlayerID);

            /*if (info.mTeam == 0)
            {
                m_att.TeamA.RemovePlayerObject(DD.Self);
            }
            else
            {
                m_att.TeamB.RemovePlayerObject(DD.Self);
            }*/
        }

        public void ProcessShipDestroyed(DestroyDespatch DD)
        {
            PlayerConnectionInfo info = m_att.PlayerInfoList.Item(DD.MiscID);

            if (info.mTeam == 0)
            {
                m_att.TeamA.RemovePlayerObject(DD.Self);
            }
            else
            {
                m_att.TeamB.RemovePlayerObject(DD.Self);
            }
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
