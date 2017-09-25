using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Space;
using Data.Space.Library;
using Space.UI;
using Space.Ship;
using Space.Teams;
using Stations;
using Space.Map;
using Space.AI;
using System.Linq;

namespace Space
{
    /// <summary>
    /// Manager class to handle background
    /// progresses within space
    /// (E.G Spawning, Objectives)
    /// </summary>
    [RequireComponent(typeof(SpaceAttributes))]
    public class SpaceManager : NetworkBehaviour
    {
        // External attributes class
        [SerializeField]
        private SpaceAttributes m_att;
        [SerializeField]
        private SpaceUtilities m_util;

        #region EVENTS 

        public delegate void KeyPress(KeyCode key);
        public event KeyPress OnKeyPress;
        public event KeyPress OnKeyRelease;

        public delegate void MouseScroll(float yDelta);
        public event MouseScroll OnMouseScroll;

        public delegate void SceneEvent();
        public event SceneEvent PlayerEnterScene;
        public event SceneEvent PlayerExitScene;
        public event SceneEvent TeamSelected;
        public event SceneEvent CameraRotated;

        public delegate void PlayerUpdate(Transform data);
        public static event PlayerUpdate OnPlayerUpdate;

        public delegate void ShipSpawnUpdate(int shipID);
        public event ShipSpawnUpdate OnShipSpawnUpdate;

        public delegate void CameraUpdate(Vector2 change);
        public event CameraUpdate OnOrientationChange;


        #endregion

        #region ACCESSORS

        public List<MapObject> Map
        {
            get { return m_att.Map.Map; }
        }

        public AIManager AI
        {
            get
            {
                return m_att.AI;
            }
        }

        public Segment.SegmentObjectManager SegObj
        {
            get
            {
                return m_att.Segment.Generator;
            }
        }

        public int ID
        {
            get
            {
                return SystemManager.Player.PlayerID;
            }
        }

        /// <summary>
        /// Network instance of 
        /// the player's ship
        /// </summary>
        public uint NetID
        {
            get
            {
                if (m_att == null)
                    return 0u;

                return m_att.netID;
            }
        }

        public TeamController Team
        {
            get
            {
                if (m_att == null)
                    return null;

                return m_att.Team;
            }
        }

        public int TeamID
        {
            get { return m_att.Team.ID; }
        }

        /// <summary>
        /// determines if able to build or not
        /// </summary>
        public bool CanBuild
        {
            get
            {
                return m_att.buildRange;
            }
        }

        /// <summary>
        /// Returns all the stations in this segment
        /// </summary>
        public StationAccessor[] GlobalStations
        {
            get { return m_att.GlobalStations.ToArray(); }
        }

        /// <summary>
        /// Quick access to ship spawn list
        /// </summary>
        private ShipSpawnData[] Spawns
        {
            get { return SystemManager.Player.ShipInventory; }
        }

        #endregion

        #region MONO BEHAVIOUR

        void Awake()
        {
            // init onstage
            m_att.PlayerOnStage = false;

            m_att.Docked = false;

            m_att.InRangeList = new List<StationAccessor>();

            m_att.GlobalStations = new List<StationAccessor>();
        }

        void Update()
        {
            ProcessSystemKeys();

            ProcessPlayerState();
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Handles exiting the level
        /// </summary>
        public void ExitLevel()
        {
            SystemManager.Disconnect();
        }

        /// <summary>
        /// Retreives the map object for a given transform
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public MapObject GetMapObject(Transform item)
        {
            return  m_att.Map.GetMapObject(item);
        }

        /// <summary>
        /// Called by the team selector once a team is
        /// selected to start the process of spawning a player
        /// </summary>
        public void InitializePlayer()
        {
            m_att.PlayerOnStage = true;

            // Trigger that team has been selected
            if(TeamSelected != null)
                TeamSelected();

            RefreshShipSpawnList();
        }

        #region STATION

        public void DockAtStation()
        { 
            // Only perform if we have a station
            if (!m_att.OverStation)
                return;

            // for now first task is to retrieve 
            // player ship and notify it to disable
            GameObject PlayerObj = GameObject.FindGameObjectWithTag
                ("PlayerShip");

            if (PlayerObj == null)
                return;                 // need to be alive

            // retrieve ship atts from player object
            ShipAccessor ship = PlayerObj.GetComponent<ShipAccessor>();

            m_att.Docked = true;

            m_att.DockingStation.Dock(true, ship);
        }

        /// <summary>
        /// Returns ship (newparameters)
        /// And changes space and hud settings to play
        /// </summary>
        public void LeaveStation()
        {
            if (!m_att.Docked)
                return;

            // for now first task is to retrieve 
            // player ship and notify it to disable
            GameObject PlayerObj = GameObject.FindGameObjectWithTag
                ("PlayerShip");

            if (PlayerObj == null)
                return;                 // need to be alive

            // retrieve ship atts from player object
            ShipAccessor ship = PlayerObj.GetComponent<ShipAccessor>();

            m_att.Docked = false;

            m_att.DockingStation.Dock(false, ship);
        }

        /// <summary>
        /// Called when player presses the key
        /// to interact with the station without docking
        /// </summary>
        public void InteractWithStation(bool keyDown)
        {
            if (m_att.InteractStation != null)
            {
                // for now first task is to retrieve 
                // player ship and notify it to disable
                GameObject PlayerObj = GameObject.FindGameObjectWithTag
                    ("PlayerShip");

                if (PlayerObj == null)
                    return;                 // need to be alive

                // retrieve ship atts from player object
                ShipAccessor ship = PlayerObj.GetComponent<ShipAccessor>();

                m_att.InteractStation.Interact(keyDown, ship);
            }
        }

        #endregion

        /// <summary>
        /// Updates the possible
        /// ship spawn items in our player data
        /// </summary>
        public void RefreshShipSpawnList()
        {
            int index = 0;
            // iterate through our new team list
            foreach (ShipSpawnData spawn in m_att.Team.Ships)
            {
                if (Spawns == null)
                    m_util.AddShipSpawn(spawn);
                else
                {
                    ShipSpawnData current = Spawns.
                        FirstOrDefault(x => x.ShipName == spawn.ShipName);

                    // If we dont have this current ship
                    // then add it
                    if (current.ShipName == null)
                    {
                        m_util.AddShipSpawn(spawn);
                    }

                    StartCoroutine("UpdatePlayerSpawn", index++);
                }              
            }
        }

        /// <summary>
        /// Triggers the orientate event
        /// for map and tracker to update 
        /// positioning (0, 1) = up, (0, -1) = down
        /// </summary>
        /// <param name="newDir"></param>
        public void ResetOrientation(Vector2 newDir)
        {
            if (OnOrientationChange != null)
                OnOrientationChange(newDir);
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Listens for player key input and 
        /// dispatch event for processing system keys
        /// </summary>
        private void ProcessSystemKeys()
        {
            // detect system input
            if (Input.anyKey)
                OnKeyPress(KeyLibrary.FindKeyPressed());

            // detect scroll
            if (Input.mouseScrollDelta.y != 0)
                OnMouseScroll(Input.mouseScrollDelta.y);

            // Detect key up
            OnKeyRelease(KeyLibrary.FindKeyReleased());
        }

        /// <summary>
        /// Searches for player object and 
        /// dispatches events for player death, spawn, and updates.
        /// </summary>
        private void ProcessPlayerState()
        {
            // Run checks for player entry
            GameObject PlayerObj = GameObject.FindGameObjectWithTag
                ("PlayerShip");

            if (PlayerObj == null)
            {
                if (m_att.PlayerOnStage)
                {
                    PlayerExitScene();
                    m_att.PlayerOnStage = false;
                }
            }
            else
            {
                if (!m_att.PlayerOnStage)
                {
                    PlayerEnterScene();
                    m_att.PlayerOnStage = true;
                }
                else
                {
                    if(OnPlayerUpdate != null)
                        OnPlayerUpdate(PlayerObj.transform);
                }
            }
        }

        #endregion

        #region COROUTINES

        private IEnumerator UpdatePlayerSpawn(int ship)
        {
            float seconds = 0.33f;

            float total = Spawns[ship].Ship.SpawnTime;

            Spawns[ship].SpawnTimer = 0;

            while (Spawns[ship].SpawnTimer < total)
            {
                yield return new WaitForSeconds(seconds);

                Spawns[ship].SpawnTimer += 0.33f;

                if(OnShipSpawnUpdate != null)
                    OnShipSpawnUpdate(ship);
            }

            yield break;
        }

        #endregion

    }
}