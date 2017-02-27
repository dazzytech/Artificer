using UnityEngine;
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

namespace Space
{
    /// <summary>
    /// Manager class to handle background
    /// progresses within space
    /// (E.G Spawning, Objectives)
    /// </summary>
    [RequireComponent(typeof(SpaceAttributes))]
    public class SpaceManager :  NetworkBehaviour
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

        public delegate void PlayerUpdate(Transform data);
        public static event PlayerUpdate OnPlayerUpdate;

        #endregion

        #region NETWORK BEHAVIOUR

        /// <summary>
        /// Runs an event to add the player ship to the 
        /// scene when local player starts.
        /// </summary>
        public override void OnStartLocalPlayer()
        {
            // Enter space segment
            SystemManager.GUI.DisplayMessege(new MsgParam("bold",
                                                   "Entering space segment"));

            base.OnStartLocalPlayer();
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
        [Server]
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

        public int ID
        {
            get
            {
                if (_att == null)
                    return -1;

                return _att.playerID;
            }
        }

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

        /// <summary>
        /// Called by the team selector once a team is
        /// selected to start the process of spawning a player
        /// </summary>
        public void InitializePlayer()
        {
            _att.PlayerOnStage = true;

            foreach(Transform ship in GameObject.Find("_ships").transform)
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
            }
            //PlayerEnterScene();
        }

        public void DockAtStation()
        {
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

            PlayerObj.SendMessage("DisableShip",
                SendMessageOptions.RequireReceiver);

            // Next is to update the HUD to display the
            // micro stationHUD
            SystemManager.GUI.SetState(UIState.Station);

            // retrieve ship atts from player object
            ShipAttributes shipAtt = PlayerObj.GetComponent<ShipAttributes>();

            // Add message for sending ship attributes
            SystemManager.GUI.InitializeStationHUD(shipAtt);
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
            SystemManager.GUI.SetState(UIState.Play);
        }

        #endregion
    }
}