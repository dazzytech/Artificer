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

        /// <summary>
        /// if the playership is currently on stage/alive
        /// </summary>
        public bool Player_OnStage;

        /// <summary>
        /// Whether or not the player has docked at a station
        /// </summary>
        public bool Player_Docked;

        /// <summary>
        /// store reference to playership gameobject
        /// </summary>
        public GameObject Player_Ship;

        /// <summary>
        /// The world camera that follows the player
        /// </summary>
        public Transform Player_Camera;

        /// <summary>
        /// store local client ID on server
        /// </summary>
        public int Player_ID;
        
        /// <summary>
        /// Network instance ID for player's ship
        /// </summary>
        public uint Player_NetID;

        /// <summary>
        /// If the player is within docking radius of a station
        /// </summary>
        public bool Player_InStationRange
        {
            get { return Station_InRangeList.Count > 0; }
        }

        /// <summary>
        /// If the player is within radius of a lootable object
        /// </summary>
        public bool Player_InLootingRange
        {
            get { return Lootable_InRangeList.Count > 0; }
        }

        /// <summary>
        /// In the player is within the build radius of a station
        /// </summary>
        public bool Player_InBuildRange;

        #endregion

        #region STATION TRACKING

        /// <summary>
        /// Current list of stations that are within range
        /// </summary>
        public List<StationAccessor> Station_InRangeList;

        /// <summary>
        /// The current station that we are pending to dock at
        /// </summary>
        public StationAccessor Station_CurrentDocking;

        /// <summary>
        /// The station we will interact with when the player triggers the interact button
        /// </summary>
        public StationAccessor Station_CurrentInteract;

        #endregion

        #region LOOTING

        /// <summary>
        /// current objects that is within looting range
        /// </summary>
        public List<Lootable> Lootable_InRangeList;

        /// <summary>
        /// Object that the player can loot
        /// </summary>
        public Lootable Lootable_CurrentObject;

        #endregion

        #region SPACE ASSETS

        public MapController Map;

        public AIManager AI;

        public SegmentManager Segment;

        
        /// <summary>
        /// Every single station reference within the space segment
        /// </summary>
        public List<StationAccessor> GlobalStations;



        #endregion

        #region TEAM ASSETS

        /// <summary>
        /// Reference to the team that the player belongs to
        /// </summary>
        public TeamController Team;

        /// <summary>
        /// Counts how many players are aligned to Team A
        /// </summary>
        public int TeamACount;

        /// <summary>
        /// Counts how many players are aligned to Team B
        /// </summary>
        public int TeamBCount;

        #endregion
    }
}
