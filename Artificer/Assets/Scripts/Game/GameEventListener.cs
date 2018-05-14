using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

using Space.Ship;
using Space.Teams;
using Stations;

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
            //SystemManager.Events.EventShipDestroyed -= ProcessShipDestroyed;
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
            SystemManager.Events.EventStationCreated += ProcessStationCreated;
        }

        #endregion

        #region EVENT LISTENERS

        public void ProcessShipCreated(CreateDispatch CD)
        {

        }

        /// <summary>
        /// Updates all teams on the destroyed ship
        /// </summary>
        /// <param name="DD"></param>
        public void ProcessShipDestroyed(DestroyDespatch DD)
        {
            // Send destroy messege to our list of
            // teams
            foreach (TeamController team in m_att.Teams)
                team.ProcessDestroyed(DD);

            // send to more in future
        }

        /// <summary>
        /// Find the station in attributes and destroy it
        /// </summary>
        /// <param name="destroyed"></param>
        public void ProcessStationDestroyed(DestroyDespatch destroyed)
        {
            // Find the station in our list and remove it
            for(int i = 0; i < m_att.GlobalStations.Count; i++)
            {
                if (m_att.GlobalStations[i] == null)
                {
                    m_att.GlobalStations.RemoveAt(i--);
                    continue;
                }

                if (m_att.GlobalStations[i].netId == destroyed.SelfID)
                {
                    m_att.GlobalStations.RemoveAt(i);
                    break;
                }
            }

            // detect win condition

            // only test with player team
            if (destroyed.SelfTeamID > 1)
                return;

            // end game if a home station 
            // for the player team is destroyed
            bool hasHomeBase = false;

            foreach(uint stationID in 
                m_att.Teams[destroyed.SelfTeamID].Stations)
            {
                StationAccessor station = ClientScene.FindLocalObject
                    (new NetworkInstanceId(stationID)).GetComponent<StationAccessor>();

                if (station.Type == STATIONTYPE.HOME)
                    hasHomeBase = true;
            }

            if(!hasHomeBase)
            {
                // game is ended
                SystemManager.EndMatch();
            }
        }

        /// <summary>
        /// Adds the station to our segment station 
        /// reference
        /// </summary>
        /// <param name="station"></param>
        public void ProcessStationCreated(CreateDispatch CD)
        {
            StationAccessor station = 
                ClientScene.FindLocalObject(new NetworkInstanceId(CD.Self))
                    .GetComponent<StationAccessor>();

            if (station == null)
            {
                return;
            }
            if (!m_att.GlobalStations.Contains(station))
            {
                m_att.GlobalStations.Add(station);
            }
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
