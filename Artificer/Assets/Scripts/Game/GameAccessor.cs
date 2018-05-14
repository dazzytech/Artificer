using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Space.Teams;
using Stations;

namespace Game
{
    public class GameAccessor : NetworkBehaviour
    {
        #region ATTRIBUTES

        private SyncListUInt m_teamIDs = new SyncListUInt();

        private SyncListUInt m_stationIDs = new SyncListUInt();

        #endregion

        #region MONOBEHAVIOUR

        public void Awake()
        {
            SystemManager.Accessor = this;
        }

        #endregion

        #region PUBLIC INTERACTION

        #region SERVER INTERACTION

        [Server]
        public void AddTeam(uint id)
        {
            m_teamIDs.Add(id);
        }

        [Server]
        public void AddStation(uint id)
        {
            m_stationIDs.Add(id);
        }

        #endregion

        #region CLIENT ACCESSOR

        public int PlayerTeamCount(int Team)
        {
            TeamController team = ClientScene.FindLocalObject
                (new NetworkInstanceId(m_teamIDs[Team])).GetComponent<TeamController>();

            return team.Players;
        }

        public TeamController[] Teams
        {
            get
            {
                TeamController[] teams = new TeamController[m_teamIDs.Count];
                int i = 0;

                foreach (uint teamID in m_teamIDs)
                {
                    TeamController team = ClientScene.FindLocalObject
                        (new NetworkInstanceId(teamID)).GetComponent<TeamController>();
                    teams[i++] = team;
                }

                return teams;
            }
        }

        public StationAccessor[] GlobalStations
        {
            get
            {
                StationAccessor[] stations = new StationAccessor[m_stationIDs.Count];
                int i = 0;

                foreach (uint stationID in m_stationIDs)
                {
                    StationAccessor station = ClientScene.FindLocalObject
                        (new NetworkInstanceId(stationID)).GetComponent<StationAccessor>();
                    stations[i++] = station;
                }

                return stations;
            }
        }

        #endregion

        #endregion
    }
}
