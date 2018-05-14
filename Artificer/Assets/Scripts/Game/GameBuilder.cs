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
            Vector2 ATraderPosition = Math.RandomWithinRange(teamAPos, 10, 30);

            // Create a main station position within team Bs location
            Vector2 BTraderPosition = Math.RandomWithinRange(teamBPos, 10, 30);

            // Create a main station position within team As location
            Vector2 AWarpPosition = Math.RandomWithinRange(teamAPos, 10, 30);

            // Create a main station position within team Bs location
            Vector2 BWarpPosition = Math.RandomWithinRange(teamBPos, 10, 30);

            // Todo: Develop more parameters for station construction
            // e.g. prefab name

            teamA.Spawner.AddStation(teamAPos);
            teamA.Spawner.AddStation(ATraderPosition, "Trader_Station");
            teamA.Spawner.AddStation(AWarpPosition, "Warp Gate");

            teamB.Spawner.AddStation(teamBPos);
            teamB.Spawner.AddStation(BTraderPosition, "Trader_Station");
            teamB.Spawner.AddStation(BWarpPosition, "Warp Gate");
        }

        [Server]
        public NetworkInstanceId GenerateStation(TeamController selectedTeam,
            string prefabName, Vector2 position)
        {
            GameObject station = selectedTeam.Spawner.AddStation(position, prefabName);
            selectedTeam.AddStationObject(station.GetComponent<NetworkIdentity>().netId);

            return station.GetComponent<NetworkIdentity>().netId;
        }

        #endregion
    }
}

