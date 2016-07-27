using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace Space.GameFunctions
{
    /// <summary>
    /// Handles messages sent from other parts of the program
    /// Has a refence to other parts 
    /// </summary>
    public class GameMessageHandler : NetworkBehaviour
    {
        public GameController _con;

        void Awake()
        {
            _con = GetComponent<GameController>();
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
            _con.AddNewPlayer(playerControllerId, conn);
        }
    }
}