using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

using Data.Space;

namespace Space.GameFunctions
{
    /// <summary>
    /// Handles messages sent from other parts of the program
    /// Has a refence to other parts 
    /// </summary>
    public class GameMessageHandler : NetworkBehaviour
    {
        public GameController Con;

        /// <summary>
        /// Adds the new player 
        /// called by game manager
        /// </summary>
        /// <returns>The new player.</returns>
        /// <param name="conn">Conn.</param>
        /// <param name="client">Client.</param>
        [Server]
        public void AddNewPlayer
            (short playerControllerId, NetworkConnection conn)
        {
            Con.AddNewPlayer(playerControllerId, conn);
        }

        /// <summary>
        /// Called when the player 
        /// chooses a spawn point to spawn their ship
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="SpawnID"></param>
        /// <param name="shipID"></param>
        [Command]
        public void CmdSpawnPlayerAt
            (int playerID, int SpawnID, int shipID)
        {
            Con.SpawnPlayer(playerID, SpawnID, shipID);
        }

        /// <summary>
        /// Called when player picked a side
        /// </summary>
        /// <param name="teamID"></param>
        /// <param name="playerID"></param>
        [Command]
        public void CmdAssignToTeam(int teamID, int playerID)
        {
            Con.AssignToTeam(teamID, playerID);
        }

        /// <summary>
        /// Initialize from gamemanager 
        /// </summary>
        [Server]
        public void InitializeGameParameters(/*GameParameters param*/)
        {
            Con.Initialize();
        }
    }
}