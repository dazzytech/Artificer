using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

using Space.Teams;

namespace Game
{
    #region PLAYER INFO
    /// <summary>
    /// Server only object, central tracker that 
    /// controls game rules and assigns teams
    /// </summary>
    public class PlayerConnectionInfo: IndexedObject
    {
        public short mController;
        public NetworkConnection mConnection;
        public int mTeam;
    }

    #endregion

    public class GameAttributes : NetworkBehaviour
    {
        #region ATTRIBUTES

        // Store builder object for generating game object
        public GameBuilder Builder;

        // Store a list of all connected players
        public IndexedList<PlayerConnectionInfo> PlayerInfoList;

        public GameState currentState;

        // Teams, currently on supports two teams
        public TeamController TeamA;

        public TeamController TeamB;

        #endregion
    }
}
