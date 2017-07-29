using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;
using System.Collections.Generic;
using System.IO;
// Artificer
using Data.Space;
using Data.Space.Library;
using Space.Ship;
using Space.Segment;
using Stations;
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

        [SerializeField]
        private SpaceAttributes m_att;
        [SerializeField]
        private SpaceManager m_con;
        [SerializeField]
        private SpaceUtilities m_util;

        #endregion

        #region MONOBEHAVIOUR

        void Awake()
        {
            m_att = GetComponent<SpaceAttributes>();
            m_con = GetComponent<SpaceManager>();
            m_util = GetComponent<SpaceUtilities>();

            NetworkManager.singleton.client.RegisterHandler((short)MSGCHANNEL.TEAMPICKER, OnTeamPickerMessage);         
            NetworkManager.singleton.client.RegisterHandler((short)MSGCHANNEL.ASSIGNTEAM, OnDefineTeam);
            NetworkManager.singleton.client.RegisterHandler((short)MSGCHANNEL.PROCESSOBJECTHIT, OnProcessHitMsg);
            NetworkManager.singleton.client.RegisterHandler((short)MSGCHANNEL.PROCESSSHIPHIT, OnProcessHitMsgShip);
            NetworkManager.singleton.client.RegisterHandler((short)MSGCHANNEL.DISPLAYINTEGRITYCHANGE, OnIntegrityChanged);
        }

        void Start()
        {
            m_util.Initialize();
        }

        void OnEnable()
        {
            SpaceManager.OnKeyPress += PlayerSystemInput;
            m_con.OnKeyRelease += PlayerSystemInputRelease;
            m_con.OnMouseScroll += PlayerMouseScroll;
            m_con.PlayerEnterScene += LoadPlayerDataIntoScene;
            SpaceManager.PlayerExitScene += PlayerDeath;

            // Station events
            StationController.EnterRange += OnEnterStation;
            StationController.ExitRange += OnExitStation;
            StationController.EnterBuildRange += OnEnterBuildRange;
            StationController.ExitBuildRange += OnExitBuildRange;
        }

        void OnDisable()
        {
            SpaceManager.OnKeyPress -= PlayerSystemInput;
            m_con.OnKeyRelease -= PlayerSystemInputRelease;
            m_con.OnMouseScroll -= PlayerMouseScroll;
            m_con.PlayerEnterScene -= LoadPlayerDataIntoScene;
            SpaceManager.PlayerExitScene -= PlayerDeath;

            // Station events
            StationController.EnterRange -= OnEnterStation;
            StationController.ExitRange -= OnExitStation;
            StationController.EnterBuildRange -= OnEnterBuildRange;
            StationController.ExitBuildRange -= OnExitBuildRange;
        }

        #endregion

        #region NETWORK BEHAVIOUR

        public override void OnStartClient()
        {
            SystemManager.UIMsg.DisplayMessege
                ("bold", "Connected to Server Match.");
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
            if (m_att.docked || key == KeyCode.None)
                return;

            if (key == Control_Config.GetKey("pause", "sys"))
            {
                m_util.Pause(true);
            }

            if (SystemManager.UIState.Current == UIState.Play ||
                SystemManager.UIState.Current == UIState.Map)
            {
                if (key == Control_Config.GetKey("zoomOut", "sys"))
                {
                    m_util.ZoomOut();
                }

                if (key == Control_Config.GetKey("zoomOut", "sys"))
                {
                    m_util.ZoomIn();
                }
                if (key == Control_Config.GetKey("map", "sys"))
                {
                    // show and hide map
                    m_util.Map(true);
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
            if (key == Control_Config.GetKey("map", "sys"))
            {
                m_util.MapRelease();
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
            m_att.PlayerShip = null;

            m_att.netID = 0;

            // Prompt player to pick a spawn
            SystemManager.UIState.SetState(UIState.SpawnPicker);

            // For now each spawn is 10 seconds
            SystemManager.UI.SetSpawnDelay(10);

            SystemManager.Background.StopBackground();
        }

        /// <summary>
        /// Load player and ship data into scene 
        /// when player ship spawns
        /// </summary>
        private void LoadPlayerDataIntoScene()
        {
            // Run checks for player entry
            m_att.PlayerShip = GameObject.FindGameObjectWithTag
                ("PlayerShip");

            if (m_att.PlayerShip != null)
            {
                m_att.netID = m_att.PlayerShip.GetComponent<NetworkIdentity>().
                    netId.Value;
            }

            // Set to popup gui
            SystemManager.UIState.SetState(UIState.Play);

            SystemManager.UI.BuildShipData();

            SystemManager.Background.StartBackground();
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

            if(m_att.station.Type == STATIONTYPE.WARP)
                SystemManager.UIMsg.DisplayPrompt("Press Enter to enter Warp Map");
            else
                SystemManager.UIMsg.DisplayPrompt("Press Enter to dock at station");
        }

        /// <summary>
        /// When player leaves station vicinity then clear reference
        /// </summary>
        /// <param name="controller"></param>
        private void OnExitStation(StationController controller)
        {
            m_att.overStation = false;

            m_att.station = null;

            SystemManager.UIMsg.ClearPrompt();

            if (m_att.docked)
                SystemManager.Space.LeaveStation();
        }

        private void OnEnterBuildRange(StationController controller)
        {
            m_att.buildRange = true;
        }

        /// <summary>
        /// When player leaves station vicinity then clear reference
        /// </summary>
        /// <param name="controller"></param>
        private void OnExitBuildRange(StationController controller)
        {
            m_att.buildRange = false;
        }

        #endregion

        #region SPACE EVENTS

        /// <summary>
        /// Alerts the space segment to 
        /// component collection
        /// </summary>
        /// <param name="item"></param>
        public void ItemCollected(int item)
        {
            //_att.Contract.ProcessMaterials
            //  (newMat);
        }

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
                    SystemManager.GUI.DisplayMessege(new MsgParam("sm-green", "You have destroyed an enemy."));
                if(destroyed.AlignmentLabel == "Friendly")
                    SystemManager.GUI.DisplayMessege(new MsgParam("sm-red", "You have destroyed an friendly."));
            }
        }

        public void StationDestroyed(DestroyDespatch destroyed)
        {

        }*/

        public void StationReached(Transform ship)
        {
            //if (_att.Contract == null)
            //return;

            //_att.Contract.ProcessStationReached(ship);
        }

        private void EndLevel(/*GameState newState*/)
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
        public void RpcEndGame(/*GameState newState*/)
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
            SystemManager.UIState.SetState(UIState.TeamPicker);

            // Retreive variables and display options
            TeamPickerMessage tpm = netMsg.ReadMessage<TeamPickerMessage>();
            SystemManager.UI.SetTeamOptions(tpm.teamOne, tpm.teamTwo);
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

            m_att.Team.EventShipListChanged += m_con.RefreshShipSpawnList;

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

        public void OnIntegrityChanged(NetworkMessage msg)
        {
            // Convert Message
            IntegrityChangedMsg chgMsg =
                msg.ReadMessage<IntegrityChangedMsg>();

            // Display message
            SystemManager.UI.DisplayIntegrityChange
                    (chgMsg.Location, chgMsg.Amount);
        }

        #endregion
    }
}
