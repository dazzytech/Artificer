using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
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
    public class SpaceEventListener : NetworkBehaviour
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

            NetworkManager.singleton.client.RegisterHandler(MsgType.Highest + 5, OnTeamPickerMessage);
            NetworkManager.singleton.client.RegisterHandler(MsgType.Highest + 6, OnNewIDMessage);
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
            _con.PlayerEnterScene += LoadPlayerDataIntoScene;
            SpaceManager.PlayerExitScene += PlayerDeath;
            ShipMessageController.OnShipDestroyed += ShipDestroyed;
        }
    	
        void OnDisable()
        {
            _con.OnKeyPress -= PlayerSystemInput;
            _con.OnKeyRelease -= PlayerSystemInputRelease;
            _con.OnMouseScroll -= PlayerMouseScroll;
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
            // Prompt player to pick a spawn
            GameManager.GUI.SetState(UIState.SpawnPicker);

            // For now each spawn is 10 seconds
            GameManager.GUI.SetSpawnDelay(10f);

            GameManager.Background.StopBackground();

            // Send this to gamemanager instead
            //_att.TeamSpawn.CmdSpawnNewPlayerShip();
            // Group up all player respawns
            /*GameObject[] spawnObjs = GameObject.FindGameObjectsWithTag("TeamSpawner");
            PlayerSpawner PS = spawnObjs [Random.Range(0,
                      spawnObjs.Length)].GetComponent<PlayerSpawner>();

            PS.Engage();*/
        }

        /// <summary>
        /// Load player and ship data into scene 
        /// when player ship spawns
        /// </summary>
        private void LoadPlayerDataIntoScene()
        {
            // Set to popup gui
            GameManager.GUI.SetState(UIState.Play);

            GameManager.GUI.BuildShipData();

            GameManager.Background.StartBackground();
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
                    GameManager.GUI.DisplayMessege(new MsgParam("sm-green", "You have destroyed an enemy."));
                if(destroyed.AlignmentLabel == "Friendly")
                    GameManager.GUI.DisplayMessege(new MsgParam("sm-red", "You have destroyed an friendly."));
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

        #region SERVER EVENTS
        // RPCS CALLED BY THE SERVER TO PERFORM ACTIONS

        /// <summary>
        /// Called from server object when the game is
        /// over
        /// </summary>
        /// <param name="newState"></param>
        [ClientRpc]
        public void RpcEndGame(GameState newState)
        {

        }

        /// <summary>
        /// Called from the game server to 
        /// display the team selection screen
        /// listen for the button being pressed and send that 
        /// result to the server
        /// </summary>
        public void OnTeamPickerMessage(NetworkMessage netMsg)
        {
            // Set to popup gui
            GameManager.GUI.SetState(UIState.TeamPicker);

            // Retreive variables and display options
            TeamPickerMessage tpm = netMsg.ReadMessage<TeamPickerMessage>();
            GameManager.GUI.SetTeamOptions(tpm.teamOne, tpm.teamTwo);
        }

        /// <summary>
        /// Stores the ID assigned from the game controller
        /// </summary>
        /// <param name="netMsg"></param>
        public void OnNewIDMessage(NetworkMessage netMsg)
        {
            // Retreive variables and display options
            IntegerMessage im = netMsg.ReadMessage<IntegerMessage>();

            // Store our id on the server
            _att.playerID = im.value;
        }

        #endregion
    }
}
