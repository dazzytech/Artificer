﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;
using Data.Space.Library;
using Data.Space;
using Space.UI;
using Space.Ship;
using Space.Teams;
using Stations;
using Space.Map;
using Space.AI;

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
        private SpaceAttributes _att;

        #region EVENTS 

        public delegate void KeyPress(KeyCode key);
        public event KeyPress OnKeyPress;
        public event KeyPress OnKeyRelease;

        public delegate void MouseScroll(float yDelta);
        public event MouseScroll OnMouseScroll;

        public delegate void SceneEvent();
        public event SceneEvent PlayerEnterScene;
        public static event SceneEvent PlayerExitScene;
        public event SceneEvent TeamSelected;

        public delegate void PlayerUpdate(Transform data);
        public static event PlayerUpdate OnPlayerUpdate;

        #endregion

        #region ACCESSORS

        public List<MapObject> Map
        {
            get { return _att.Map.Map; }
        }

        private AIManager AI
        {
            get
            {
                return _att.AI;
            }
        }

        /// <summary>
        /// Access to the client connection
        /// 
        /// </summary>
        public NetworkConnection PlayerConn
        {
            get
            {
                if (_att.PlayerShip != null)
                    return _att.PlayerShip.GetComponent<NetworkIdentity>()
                        .connectionToClient;
                else
                    return null;
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
                if (_att == null)
                    return 0u;

                return _att.netID;
            }
        }

        public TeamController Team
        {
            get
            {
                if (_att == null)
                    return null;

                return _att.Team;
            }
        }

        public int TeamID
        {
            get { return _att.Team.ID; }
        }

        /// <summary>
        /// determines if able to build or not
        /// </summary>
        public bool CanBuild
        {
            get
            {
                return _att.buildRange;
            }
        }

        #endregion

        #region MONO BEHAVIOUR

        void Awake()
        {
            _att = GetComponent<SpaceAttributes>();

            // init onstage
            _att.PlayerOnStage = false;

            _att.docked = false;

            _att.overStation = false;
        }

        void Update()
        {
            ProcessSystemKeys();

            ProcessPlayerState();

            /*  // Update Spawns
              if (_att.EnemySpawn != null)
              {
                  _att.EnemySpawn.CycleEnemySpawn();
              }
              if (_att.FriendlySpawn != null)
              {
                  _att.FriendlySpawn.CycleFriendlySpawn();
              }*/

            // Update Contract info
            /*if (_att.Contract != null)
            {
                // Test for end of level
                if(_att.Contract.ContractStatus != ContractState.Pending)
                {
                    Stop();
                }
                else
                {
                    _att.Contract.RunUpdate();
                }
            }*/
        }

        #endregion

        #region INTERNAL PROCESSING

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
                if (_att.PlayerOnStage)
                {
                    PlayerExitScene();
                    _att.PlayerOnStage = false;
                }
            }
            else
            {
                if (!_att.PlayerOnStage)
                {
                    PlayerEnterScene();
                    _att.PlayerOnStage = true;
                }
                else
                {
                    OnPlayerUpdate(PlayerObj.transform);
                }
            }
        }

        // TODO: INTERNAL OR EXTERNAL
        /// <summary>
        /// Builds game related instances and adds us to the server game environment
        /// </summary>
        public void InitializeSpaceParameters()//GameParameters param)
        {
            /// Dont run these yet
            // Initialize space attributes
            //_att.Contract.Initialize(param);
            //_att.EnemySpawn = new EnemySpawnManager(param);
            //_att.FriendlySpawn = new FriendlySpawnManager(param);
        }

        #endregion

        #region EXTERNAL FUNCTIONS

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
            return  _att.Map.GetMapObject(item);
        }

        /// <summary>
        /// Called by the team selector once a team is
        /// selected to start the process of spawning a player
        /// </summary>
        public void InitializePlayer()
        {
            _att.PlayerOnStage = true;

            // Trigger that team has been selected
            if(TeamSelected != null)
                TeamSelected();

            // Instead of doing this create a team selected event
            /*foreach(Transform ship in GameObject.Find("_ships").transform)
            {
                if(ship.tag == "Untagged")
                {
                    NetworkInstanceId netId = ship.GetComponent<NetworkIdentity>().netId;

                    // Determine tag based on our reference to team
                    if (SystemManager.Space.Team.PlayerOnTeam(netId))
                    {
                        ship.name = "AllyShip";
                        ship.tag = "Friendly";
                    }
                    else
                    {
                        ship.name = "EnemyShip";
                        ship.tag = "Enemy";
                    }
                }
            }*/
        }

        public void DockAtStation()
        {
            // SHARED FUNCTION

            // Only perform if we have a station
            if (!_att.overStation)
                return;

            // for now first task is to retrieve 
            // player ship and notify it to disable
            GameObject PlayerObj = GameObject.FindGameObjectWithTag
                ("PlayerShip");

            if (PlayerObj == null)
                return;                 // need to be alive

            _att.overStation = false;

            _att.docked = true;

            // Next is to update the HUD to display the
            // micro stationHUD
            SystemManager.UIState.SetState(UIState.Station);

            // WARP FUNCTION
            if (_att.station.Type == Stations.STATIONTYPE.WARP)
            {
                // call warp map HUD
                SystemManager.UI.InitializeWarpMap(
                    ((WarpController)_att.station).Nearby, _att.station.transform);
            }
            else
            {
                // For now every other type does this
                PlayerObj.SendMessage("DisableShip",
                    SendMessageOptions.RequireReceiver);

                // retrieve ship atts from player object
                ShipAttributes shipAtt = PlayerObj.GetComponent<ShipAttributes>();

                // Add message for sending ship attributes
                SystemManager.UI.InitializeStationHUD(shipAtt);
            }
        }

        /// <summary>
        /// Returns ship (newparameters)
        /// And changes space and hud settings to play
        /// </summary>
        public void LeaveStation()
        {
            if (!_att.docked)
                return;

            // for now first task is to retrieve 
            // player ship and notify it to disable
            GameObject PlayerObj = GameObject.FindGameObjectWithTag
                ("PlayerShip");

            if (PlayerObj == null)
                return;                 // need to be alive

            _att.overStation = true;

            _att.docked = false;

            PlayerObj.SendMessage("EnableShip",
                SendMessageOptions.RequireReceiver);

            // Next is to update the HUD to display the
            // micro stationHUD
            SystemManager.UIState.SetState(UIState.Play);
        }

        #endregion
    }
}