using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Data.Space.DataImporter;
using Data.Space;
using Space.Spawn;
using Space.Teams;

namespace Space.AI
{
    /// <summary>
    /// Centralized system for managing ai states
    /// </summary>
    public class AIManager : NetworkBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private AIAttributes m_att;

        #endregion

        #region ACCESSORS

        /// <summary>
        /// Getter for agent ships for spawner objects
        /// </summary>
        public Dictionary<string, AgentData> AgentLibrary
        {
            get
            {
                return m_att.AgentLibrary;
            }
        }

        #endregion

        #region MONO BEHAVIOUR

        /// <summary>
        /// Begin heatbeats and 
        /// object seeking
        /// </summary>
        void Awake()
        {
            // Begin the process of importing agent data
            m_att.AgentLibrary = AgentDataImporter.BuildAgents();

            m_att.Teams = new List<TeamController>();
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Called by game manager
        /// create a random number of teams and initailize
        /// </summary>
        /// <param name="param"></param>
        [Server]
        public void Initialize(GameParameters param)
        {
            m_att.Param = param;

            // get a random number of stations
            int teamCount = Random.Range(6, 12);

            for (int i = 0; i < teamCount; i++)
            {
                int fortify = Random.Range(1, 4);

                // this is a default asteroid
                // Spawn them closer to station
                // first 5 at team A and rest near team B
                Vector2 focus;
                if (i < teamCount/2)
                    focus = m_att.Param.TeamASpawn;
                else
                    focus = m_att.Param.TeamBSpawn;

                m_att.Teams.Add(BuildTeam(fortify, focus, i));
            }
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Builds a team controller and populates with 
        /// stations fortify = 1 - 3
        /// </summary>
        [Server]
        private TeamController BuildTeam(int fortify, Vector2 pos, int index)
        {
            // build and position the game objects
            GameObject teamGO = Instantiate(m_att.TeamPrefab);
            teamGO.transform.SetParent(m_att.TeamHUD); 
            NetworkServer.Spawn(teamGO);

            // create randomized position
            Vector2 position = Math.RandomWithinRange
                (pos, 400f * fortify + (50 * index), 750f * fortify + (50 * index));

            // Clamp within the bounds
            position.x = Mathf.Clamp(position.x, 0, SystemManager.Size.width);
            position.y = Mathf.Clamp(position.y, 0, SystemManager.Size.height);

            teamGO.transform.position = position;

            // Initialize team
            TeamController team =
                teamGO.GetComponent<TeamController>();
            team.Initialize(index + 2,fortify);
            team.Spawner.AI = this;

            // spawn number of stations
            int stationCount = fortify + Random.Range(0, 2);

            for (int i = 0; i < stationCount; i++)
            {
                team.AddStationObject(CreateStation(team, position));
            }

            return team;
        }

        /// <summary>
        /// Creates a station and assigns it to
        /// the team spawner and returns the netID
        /// </summary>
        /// <param name="team"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        [Server]
        private NetworkInstanceId CreateStation
            (TeamController team, Vector2 pos)
        {
            bool inRange = true;

            Vector2 stationPos = Vector2.zero;

            // maintain certain distance
            while (inRange)
            {
                inRange = false;

                stationPos = Math.RandomWithinRange(pos, 100f, 500f);

                foreach (uint other in team.Stations)
                {
                    if (Vector3.Distance(stationPos, ClientScene.FindLocalObject
                        (new NetworkInstanceId(other)).transform.position) < 5f)
                    {
                        inRange = true;
                        break;
                    }
                }
            }

            GameObject stationGO = team.Spawner.AddStation(stationPos);
            NetworkServer.Spawn(stationGO);

            return stationGO.GetComponent<NetworkIdentity>().netId;
        }

        #endregion

    }
}
