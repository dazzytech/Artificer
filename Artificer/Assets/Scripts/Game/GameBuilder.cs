using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Space;
using Space.Segment.Generator;
using Space.Teams;

namespace Game
{
    /// <summary>
    /// Build game specific space objects e.g.
    /// Starter bases and team spawns
    /// </summary>
    public class GameBuilder: NetworkBehaviour
    {
        #region PUBLIC INTERACTION
        
        /// <summary>
        /// For now, builds a group of player spawners in close 
        /// proximity and sends this to the team spawner
        /// </summary>
        [Server]
        public void GenerateTeams
            (TeamController teamA, TeamController teamB, GameParameters param)
        {
            // place team A at the top half of the map
            Vector2 teamAPos = param.TeamASpawn;

            // Place team B at the other half
            Vector2 teamBPos = param.TeamBSpawn;

            // Create a main station position within team As location
            Vector2 stationAPosition = new Vector2
                (Random.Range((teamAPos.x - 50), (teamAPos.x + 50)),
                 Random.Range((teamAPos.y - 50), (teamAPos.y + 50)));

            // Create a main station position within team Bs location
            Vector2 stationBPosition = new Vector2
                (Random.Range((teamBPos.x - 50), (teamBPos.x + 50)),
                 Random.Range((teamBPos.y - 50), (teamBPos.y + 50)));

            // Todo: Develop more parameters for station construction
            // e.g. prefab name

            // for now only send one vector position
            GameObject stationA = teamA.Spawner.AddStation(stationAPosition);
            teamA.AddStationObject(stationA.GetComponent<NetworkIdentity>().netId);

            GameObject stationB = teamB.Spawner.AddStation(stationBPosition);
            teamB.AddStationObject(stationB.GetComponent<NetworkIdentity>().netId);
        }

        [Server]
        public void GenerateStation(TeamController selectedTeam,
            string prefabName, Vector2 position)
        {
            GameObject station = selectedTeam.Spawner.AddStation(position, prefabName);
            selectedTeam.AddStationObject(station.GetComponent<NetworkIdentity>().netId);
        }

        #endregion
    }
}

