using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;

using Data.Space;

namespace Space.GameFunctions
{
    #region NETWORK MESSAGE OBJECTS 

    /// <summary>
    /// Message containig the ID of the client
    /// and the team selected
    /// </summary>
    public class TeamSelectionMessage : MessageBase
    {
        public int Selected;
        public int ID;
    }

    public class SpawnSelectionMessage : MessageBase
    {
        public int PlayerID;
        public int SpawnID;
        public int ShipID;
    }

    #endregion

    /// <summary>
    /// Handles messages sent from other parts of the program
    /// Has a refence to other parts 
    /// </summary>
    public class GameMessageHandler : NetworkBehaviour
    {
        public GameController Con;

        void Awake()
        {
            // For team selection
            NetworkServer.RegisterHandler(MsgType.Highest + 7, OnAssignToTeam);
            NetworkServer.RegisterHandler(MsgType.Highest + 8, OnSpawnPlayerAt);
        }

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
        public void OnSpawnPlayerAt
            (NetworkMessage netMsg)
        {
            // Retreive variables and display options
            SpawnSelectionMessage ssm = netMsg.ReadMessage<SpawnSelectionMessage>();
            Con.SpawnPlayer(ssm.PlayerID, ssm.SpawnID, ssm.ShipID);
        }

        /// <summary>
        /// Called when player picked a side
        /// </summary>
        /// <param name="teamID"></param>
        /// <param name="playerID"></param>
        public void OnAssignToTeam(NetworkMessage netMsg)
        {
            // Retreive variables and display options
            TeamSelectionMessage tsm = netMsg.ReadMessage<TeamSelectionMessage>();
            Con.AssignToTeam(tsm.Selected, tsm.ID);
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