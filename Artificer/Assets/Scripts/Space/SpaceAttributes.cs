using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Space;
using Data.Space.Library;
using Space.Segment;
using Stations;
using Space.Teams;
using Space.Map;
using Space.AI;

// Add classes for data
// that can be serialized and saved

// e.g Player_Data
namespace Space
{
    /// <summary>
    /// Central attributes container for space manager
    /// stores player state.
    /// </summary>
    public class SpaceAttributes : MonoBehaviour
    {
        #region PLAYER TRACKING

        // track if the player is currently on stage
        public bool PlayerOnStage;

        // store reference for playership
        public GameObject PlayerShip;

        // store local client ID on server
        public int playerID;

        // Network instance ID for player's ship
        public uint netID;

        #endregion

        #region STATION TRACKING

        // if the player is within vicinity of station
        public bool OverStation
        {
            get { return InRangeList.Count > 0; }
        }

        // if player is within construction Range
        public bool buildRange;

        /// <summary>
        /// Current list of stations that are within
        /// docking range
        /// </summary>
        public List<StationAccessor> InRangeList;

        /// <summary>
        /// The current station that we are 
        /// electing to dock at
        /// </summary>
        public StationAccessor DockingStation;

        /// <summary>
        /// The station we will interact with when prompted
        /// </summary>
        public StationAccessor InteractStation;

        /// <summary>
        /// Whether or not the player has docked at
        /// this station
        /// </summary>
        public bool Docked;

        #endregion

        #region SPACE ASSETS

        public MapController Map;

        public AIManager AI;

        public SegmentManager Segment;

        #region LOCAL OBJECT

        /// <summary>
        /// Every single station in segment
        /// </summary>
        public List<StationAccessor> GlobalStations;

        /// <summary>
        /// Reference to the team we belong to
        /// </summary>
        public TeamController Team;

        #endregion

        #endregion
    }
}
