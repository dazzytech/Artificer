using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
// Artificer
using Data.Shared;
using Data.Space.Library;
using Space.GameFunctions;
using Space.Generator;
using Space.Ship;
using Space.UI;


namespace Space
{
    /// <summary>
    /// Listens for events from the space manager obj
    /// e.g. Player death
    /// </summary>
    [RequireComponent(typeof(SpaceAttributes))]
    [RequireComponent(typeof(SpaceManager))]
    [RequireComponent(typeof(SpaceUtilities))]
    public class SpaceEventListener : MonoBehaviour
    {
        #region ATTRIBUTES

        private SpaceAttributes _att;
        private SpaceManager _con;
        private SpaceUtilities _util;

        #endregion

        #region MONOBEHAVIOUR

        void Awake()
        {
            _att = GetComponent<SpaceAttributes>();
            _con = GetComponent<SpaceManager>();
            _util = GetComponent<SpaceUtilities>();
        }

        void Start()
        {
            _util.Init();
        }

        void OnEnable()
        {
            _con.OnKeyPress += PlayerSystemInput;
            _con.OnKeyRelease += PlayerSystemInputRelease;
            _con.OnMouseScroll += PlayerMouseScroll;
            _att.GameController.OnChangeState += EndLevel;
            _con.PlayerEnterScene += LoadPlayerDataIntoScene;
            SpaceManager.PlayerExitScene += PlayerDeath;
            ShipMessageController.OnShipDestroyed += ShipDestroyed;
        }
    	
        void OnDisable()
        {
            _con.OnKeyPress -= PlayerSystemInput;
            _con.OnKeyRelease -= PlayerSystemInputRelease;
            _con.OnMouseScroll -= PlayerMouseScroll;
            _att.GameController.OnChangeState -= EndLevel;
            _con.PlayerEnterScene -= LoadPlayerDataIntoScene;
            SpaceManager.PlayerExitScene -= PlayerDeath;
            ShipMessageController.OnShipDestroyed -= ShipDestroyed;
        }

        #endregion

        #region PLAYER INTERACTION

        /// <summary>
        /// Handles Players input
        /// for system actions.
        /// </summary>
        /// <param name="key">Key.</param>
        private void PlayerSystemInput(KeyCode key)
        {
            if (key == Control_Config.GetKey("pause", "sys"))
            {
                _util.Pause(true);
            }

            if (Time.timeScale == 1f)
            {
                if (key == Control_Config.GetKey("zoomOut", "sys"))
                {
                    _util.ZoomOut();
                }

                if (key == Control_Config.GetKey("zoomOut", "sys"))
                {
                    _util.ZoomIn();
                }
                if(key == Control_Config.GetKey("toggle hud", "sys"))
                {
                    GameObject.Find("_gui").SendMessage("ToggleHUD");
                }
                if(key == Control_Config.GetKey("toggle objectives", "sys"))
                {
                    GameObject.Find("_gui").SendMessage("ToggleMissionHUD");
                }
            }
        }

        private void PlayerSystemInputRelease(KeyCode key)
        {
            if (key == Control_Config.GetKey("pause", "sys"))
            {
                _util.PauseRelease();
            }
        }

        /// <summary>
        /// Zoom the map with middle mouse wheel
        /// </summary>
        /// <param name="yDelta"></param>
        private void PlayerMouseScroll(float yDelta)
        {
            if (Input.mouseScrollDelta.y < 0f)
                _util.ZoomOut();
            if (Input.mouseScrollDelta.y > 0f)
                _util.ZoomIn();
        }

        #endregion

        #region PLAYER EVENTS

        /// <summary>
        /// When player ship is destroyed everything in here
        /// is executed
        /// </summary>
        private void PlayerDeath()
        {
            // Send this to gamemanager instead
            //_att.TeamSpawn.CmdSpawnNewPlayerShip();
            // Group up all player respawns
            /*GameObject[] spawnObjs = GameObject.FindGameObjectsWithTag("TeamSpawner");
            PlayerSpawner PS = spawnObjs [Random.Range(0,
                      spawnObjs.Length)].GetComponent<PlayerSpawner>();

            PS.Engage();

            // Set to popup gui
            GameObject.Find("_gui").
                SendMessage("SetState", UIState.Popup);

            // Set to popup gui
            GameObject.Find("_gui").
                SendMessage("SetCounter", 5f);
            // Set Popup UI to respawn view 
            */
        }

        /// <summary>
        /// Load player and ship data into scene 
        /// when player ship spawns
        /// </summary>
        private void LoadPlayerDataIntoScene()
        {
            GameObject.Find("_gui").
              SendMessage("BuildShipData");

            // Set to popup gui
            GameObject.Find("_gui").
                SendMessage("SetState", UIState.Play);
        }

        #endregion

        #region SPACE EVENTS

        /// <summary>
        /// Receives a message that a ship
        /// has been destroyed and processes the event
        /// </summary>
        /// <param name="input">Input.</param>
        public void ShipDestroyed(DestroyDespatch destroyed)
        {
            //_att.Contract.ProcessShipDestroyed
                //(destroyed);

            if(destroyed.AggressorTag == "PlayerShip")
            {
                if(destroyed.AlignmentLabel == "Enemy")
                    MessageHUD.DisplayMessege(new MsgParam("sm-green", "You have destroyed an enemy."));
                if(destroyed.AlignmentLabel == "Friendly")
                    MessageHUD.DisplayMessege(new MsgParam("sm-red", "You have destroyed an friendly."));
            }
        }
        
        public void MaterialCollected(Dictionary<MaterialData, float> newMat)
        {
            //_att.Contract.ProcessMaterials
              //  (newMat);
        }
        
        public void StationReached(Transform ship)
        {
            //if (_att.Contract == null)
                //return;

            //_att.Contract.ProcessStationReached(ship);
        }

        private void EndLevel(GameState newState)
        {
            _util.Stop();

            // Set Popup to display either win or lose screen
            /*GameObject.Find("_gui").
                SendMessage("EndGame", _att.Contract.ContractStatus
                == Space.Contract.ContractState.Completed);

            if(_att.Contract.Rewards != null)
                GameObject.Find("_gui").
                    SendMessage("UpdateReward", _att.Contract.Rewards);*/
        }

        #endregion
    }
}
