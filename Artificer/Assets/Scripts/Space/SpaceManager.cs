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
using ShipComponents;

/// <summary>
/// Is responsible for processing
/// actions not existant within a game object
/// General UI
/// Missions
/// </summary>
namespace Space
{
    [RequireComponent(typeof(SpaceAttributes))]
    public class SpaceManager :  NetworkBehaviour
    {
        private SpaceAttributes _att;
        
        void Awake()
        {
            _att = GetComponent<SpaceAttributes>();
        }


        // Event managment
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

        public override void OnStartLocalPlayer()
        {
            // Enter space segment
            MessageHUD.DisplayMessege(new MsgParam("bold",
                                                   "Entering space segment"));
            
            _att.PlayerOnStage = true;
            
            PlayerEnterScene();

            base.OnStartLocalPlayer();
        }

        /// <summary>
        /// Builds game related instances and adds us to the server game environment
        /// </summary>
        public void InitializeSpaceParameters(GameParameters param)
        {
            /// Dont run these yet
            // Initialize space attributes
            //_att.Contract.Initialize(param);
            //_att.EnemySpawn = new EnemySpawnManager(param);
            //_att.FriendlySpawn = new FriendlySpawnManager(param);
        }

        void Update()
        {
            // detect system input
            if (Input.anyKey)
                OnKeyPress(KeyLibrary.FindKeyPressed());

            // detect scroll
            if (Input.mouseScrollDelta.y != 0)
                OnMouseScroll(Input.mouseScrollDelta.y);

            // Detect key up
            OnKeyRelease(KeyLibrary.FindKeyReleased ());

            // Run checks for player entry
            GameObject PlayerObj = GameObject.FindGameObjectWithTag 
                ("PlayerShip");

           if(PlayerObj == null)
           {
               if(_att.PlayerOnStage)
               {
                    PlayerExitScene();
                    _att.PlayerOnStage = false;
                }
           } else
           {
                if(!_att.PlayerOnStage)
                {
                    PlayerEnterScene();
                    _att.PlayerOnStage = true;
                }
                else
                {
                    OnPlayerUpdate(PlayerObj.transform);
                }
           }


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

        // call disconnect to gamemanager
        public void ExitLevel()
        {
            GameManager.Disconnect();
        }
    }
}