using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

using Space.Teams;

namespace Space.GameFunctions
{
    #region PLAYER INFO
    /// <summary>
    /// Server only object, central tracker that 
    /// controls game rules and assigns teams
    /// </summary>
    public class PlayerConnectionInfo
    {
        public int mID;
        public short mController;
        public NetworkConnection mConnection;
        public int mTeam;
        public bool mSpawned;
    }

    #endregion

    public class GameAttributes : NetworkBehaviour
    {
        #region ATTRIBUTES

        // Store builder object for generating game object
        public GameBuilder Builder;

        // Store a list of all connected players
        public List<PlayerConnectionInfo> PlayerInfoList;

        // Teams, currently on supports two teams
        public TeamController TeamA;

        public TeamController TeamB;

        #endregion
    }
}
