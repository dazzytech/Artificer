using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

using Space.Teams;
using Space.Segment;
using Space.Spawn;
using Space.AI;
using Stations;

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
        #region SPACE ASSET

        /// <summary>
        /// Builds assets into space, e.g. player teams
        /// </summary>
        public GameBuilder Builder;

        /// <summary>
        /// Controls the generation of segment objects in space
        /// </summary>
        public SegmentManager Segment;

        /// <summary>
        /// spawns and manages npcs in space
        /// </summary>
        public AIManager AI;

        /// <summary>
        /// Teams added to space 0 and 1 are player teams
        /// </summary>
        public List<TeamController> Teams;

        /// <summary>
        /// Every single station reference within the space segment
        /// </summary>
        public List<StationAccessor> GlobalStations;

        #endregion

        #region GAME ASSETS

        /// <summary>
        /// list of all players connected to the match
        /// </summary>
        public IndexedList<PlayerConnectionInfo> PlayerInfoList;

        /// <summary>
        /// whether the match is in lobby, playing or ended
        /// </summary>
        public GameState CurrentState;

        #endregion
    }
}
