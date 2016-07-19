using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;
using Data.Space.Library;
using Data.Space;
using Space.Contract;
using Space.UI;

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
        public event SceneEvent Stop;
        public event SceneEvent PlayerEnterScene;
        public static event SceneEvent PlayerExitScene;

        public delegate void PlayerUpdate(Transform data);
        public static event PlayerUpdate OnPlayerUpdate;

        #endregion

        #region NETWORK BEHAVIOUR

        /// <summary>
        /// Initialize game rules and spawns
        /// </summary>
        public override void OnStartServer()
        {
            InitializeSpaceParameters(); 
        }

        /// <summary>
        /// Runs an event to add the player ship to the 
        /// scene when local player starts.
        /// </summary>
        public override void OnStartLocalPlayer()
        {
            // Enter space segment
            MessageHUD.DisplayMessege(new MsgParam("bold",
                                                   "Entering space segment"));
            
            _att.PlayerOnStage = true;
            
            PlayerEnterScene();

            base.OnStartLocalPlayer();
        }

        #endregion

        #region MONO BEHAVIOUR

        void Awake()
        {
            _att = GetComponent<SpaceAttributes>();
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

        #endregion

        // TODO: INTERNAL OR EXTERNAL
        /// <summary>
        /// Builds game related instances and adds us to the server game environment
        /// </summary>
        public void InitializeSpaceParameters()//GameParameters param)
        {
            _att.Builder = new GameFunctions.GameBuilder();
            _att.Builder.GenerateSpawners();
            /// Dont run these yet
            // Initialize space attributes
            //_att.Contract.Initialize(param);
            //_att.EnemySpawn = new EnemySpawnManager(param);
            //_att.FriendlySpawn = new FriendlySpawnManager(param);
        }

        #region EXTERNAL FUNCTIONS

        /// <summary>
        /// Handles exiting the level
        /// </summary>
        public void ExitLevel()
        {
            GameManager.Disconnect();
        }

        #endregion
    }
}