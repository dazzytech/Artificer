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
using Space.Segment;
using Space.UI;
using Networking;
using Space.Teams;
using Space.Projectiles;

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

        private SpaceAttributes m_att;
        private SpaceManager m_con;
        private SpaceUtilities m_util;

        #endregion

        #region MONOBEHAVIOUR

        void Awake()
        {
            m_att = GetComponent<SpaceAttributes>();
            m_con = GetComponent<SpaceManager>();
            m_util = GetComponent<SpaceUtilities>();

            NetworkManager.singleton.client.RegisterHandler((short)MSGCHANNEL.TEAMPICKER, OnTeamPickerMessage);
            NetworkManager.singleton.client.RegisterHandler((short)MSGCHANNEL.NEWID, OnNewIDMessage);
            NetworkManager.singleton.client.RegisterHandler((short)MSGCHANNEL.ASSIGNTEAM, OnDefineTeam);
            NetworkManager.singleton.client.RegisterHandler((short)MSGCHANNEL.PROCESSOBJECTHIT, OnProcessHitMsg);
            NetworkManager.singleton.client.RegisterHandler((short)MSGCHANNEL.PROCESSSHIPHIT, OnProcessHitMsgShip);
            NetworkManager.singleton.client.RegisterHandler((short)MSGCHANNEL.CREATEPROJECTILE, OnProjectileCreated);
            NetworkManager.singleton.client.RegisterHandler((short)MSGCHANNEL.DISPLAYINTEGRITYCHANGE, OnIntegrityChanged);
        }

        void Start()
        {
            m_util.Init();
        }

        void OnEnable()
        {
            m_con.OnKeyPress += PlayerSystemInput;
            m_con.OnKeyRelease += PlayerSystemInputRelease;
            m_con.OnMouseScroll += PlayerMouseScroll;
            m_con.PlayerEnterScene += LoadPlayerDataIntoScene;
            SpaceManager.PlayerExitScene += PlayerDeath;

            // Station events
            StationController.EnterStation += OnEnterStation;
            StationController.ExitStation += OnExitStation;
        }

        void OnDisable()
        {
            m_con.OnKeyPress -= PlayerSystemInput;
            m_con.OnKeyRelease -= PlayerSystemInputRelease;
            m_con.OnMouseScroll -= PlayerMouseScroll;
            m_con.PlayerEnterScene -= LoadPlayerDataIntoScene;
            SpaceManager.PlayerExitScene -= PlayerDeath;

            // Station events
            StationController.EnterStation -= OnEnterStation;
            StationController.ExitStation -= OnExitStation;
        }

        #endregion

        #region NETWORK BEHAVIOUR

        public override void OnStartClient()
        {
            GameManager.GUI.DisplayMessege(new MsgParam("bold", "Connected to Server Match."));
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
            // only interact if not docked
            if (m_att.docked)
                return;

            if (key == Control_Config.GetKey("pause", "sys"))
            {
                m_util.Pause(true);
            }

            if (Time.timeScale == 1f)
            {
                if (key == Control_Config.GetKey("zoomOut", "sys"))
                {
                    m_util.ZoomOut();
                }

                if (key == Control_Config.GetKey("zoomOut", "sys"))
                {
                    m_util.ZoomIn();
                }
                if (key == Control_Config.GetKey("toggle hud", "sys"))
                {
                    GameObject.Find("_gui").SendMessage("ToggleHUD");
                }
                if (key == Control_Config.GetKey("toggle objectives", "sys"))
                {
                    GameObject.Find("_gui").SendMessage("ToggleMissionHUD");
                }
                if (key == Control_Config.GetKey("dock", "sys"))
                {
                    m_con.DockAtStation();
                }
            }
        }

        private void PlayerSystemInputRelease(KeyCode key)
        {
            if (key == Control_Config.GetKey("pause", "sys"))
            {
                m_util.PauseRelease();
            }
        }

        /// <summary>
        /// Zoom the map with middle mouse wheel
        /// </summary>
        /// <param name="yDelta"></param>
        private void PlayerMouseScroll(float yDelta)
        {
            if (m_att.docked)
                return;

            if (Input.mouseScrollDelta.y < 0f)
                m_util.ZoomOut();
            if (Input.mouseScrollDelta.y > 0f)
                m_util.ZoomIn();
        }

        #endregion

        #region PLAYER EVENTS

        /// <summary>
        /// When player ship is destroyed everything in here
        /// is executed
        /// </summary>
        private void PlayerDeath()
        {
            m_att.netID = 0;

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
            // Run checks for player entry
            GameObject PlayerObj = GameObject.FindGameObjectWithTag
                ("PlayerShip");

            if (PlayerObj != null)
            {
                m_att.netID = PlayerObj.GetComponent<NetworkIdentity>().
                    netId.Value;
            }

            // Set to popup gui
            GameManager.GUI.SetState(UIState.Play);

            GameManager.GUI.BuildShipData();

            GameManager.Background.StartBackground();
        }

        /// <summary>
        /// When player is in proximity of a station it can enter
        /// then keep reference tho the station
        /// </summary>
        /// <param name="controller"></param>
        private void OnEnterStation(StationController controller)
        {
            m_att.overStation = true;

            m_att.station = controller;

            GameManager.GUI.DisplayPrompt("Press Enter to dock at station");
        }

        /// <summary>
        /// When player leaves station vicinity then clear reference
        /// </summary>
        /// <param name="controller"></param>
        private void OnExitStation(StationController controller)
        {
            m_att.overStation = false;

            m_att.station = null;

            GameManager.GUI.ClearPrompt();
        }

        #endregion

        #region SPACE EVENTS

        // Consider using sync events

        /*
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

        public void StationDestroyed(DestroyDespatch destroyed)
        {

        }*/

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
            m_util.Stop();

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
            m_att.playerID = im.value;
        }

        public void OnDefineTeam(NetworkMessage netMsg)
        {
            // Retrieve net id from sent message
            NetworkInstanceId teamID = netMsg.ReadMessage
                <NetMsgMessage>().SelfID;

            // Get our local team object with the same netID
            GameObject teamObj = ClientScene.
                FindLocalObject(teamID);

            // extract the team manager and store int space object
            m_att.Team = teamObj.
                GetComponent<TeamController>();

            m_con.InitializePlayer();
        }

        public void OnProcessHitMsg(NetworkMessage msg)
        {
            SOColliderHitMessage colMsg = msg.ReadMessage<SOColliderHitMessage>();

            GameObject HitObj = ClientScene.FindLocalObject(colMsg.SObjectID);

            if (HitObj != null)
            {
                HitObj.transform.SendMessage("ApplyDamage", colMsg.HitD, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                Debug.Log("ERROR: Space Event Listener - Process Hit Msg: " +
                    "Passed hit object is null.");
            }
        }

        public void OnProcessHitMsgShip(NetworkMessage msg)
        {
            ShipColliderHitMessage colMsg = msg.ReadMessage<ShipColliderHitMessage>();

            GameObject HitObj = ClientScene.FindLocalObject(colMsg.ShipID);
            if (HitObj != null)
            {
                HitObj.transform.GetComponent<ShipImpactCollider>()
                .ProcessDamage(colMsg.HitComponents, colMsg.HitD, colMsg.HitValues);
            }
            else
            {
                Debug.Log("ERROR: Space Event Listener - Process Ship Hit Msg: " +
                    "Passed ship object is null.");
            }
        }

        public void OnProjectileCreated(NetworkMessage msg)
        {
            // retreive message
            ProjectileSpawnedMessage projMsg = msg.ReadMessage<ProjectileSpawnedMessage>();

            // find our projectile
            GameObject GO = ClientScene.FindLocalObject
                (projMsg.Projectile);

            // client side projectile building
            GO.GetComponent<WeaponController>().CreateProjectile(projMsg.WData);
        }

        public void OnIntegrityChanged(NetworkMessage msg)
        {
            // Convert Message
            IntegrityChangedMsg chgMsg =
                msg.ReadMessage<IntegrityChangedMsg>();

            // Display message
            GameManager.GUI.DisplayIntegrityChange
                    (chgMsg.Location, chgMsg.Amount);
        }

        #endregion
    }
}
