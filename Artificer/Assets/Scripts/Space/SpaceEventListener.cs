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
using Data.Space.Collectable;
using System.Linq;

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
            NetworkManager.singleton.client.RegisterHandler((short)MSGCHANNEL.TRANSACTIONCLIENT, OnTransaction);
        }

        void Start()
        {
            m_util.Initialize();
        }

        void OnEnable()
        {
            m_con.OnKeyPress += PlayerSystemInput;
            m_con.OnKeyRelease += PlayerSystemInputRelease;
            m_con.OnMouseScroll += PlayerMouseScroll;
            m_con.PlayerEnterScene += new SpaceManager.SceneEvent(LoadPlayerDataIntoScene);
            m_con.PlayerExitScene += PlayerDeath;

            // Station events
            StationController.OnEnterRange += OnEnterStation;
            StationController.OnExitRange += OnExitStation;
            StationController.OnEnterBuildRange += OnEnterBuildRange;
            StationController.OnExitBuildRange += OnExitBuildRange;
            StationController.OnStationCreated += OnStationCreated;

            if (SystemManager.Events != null)
            {
                SystemManager.Events.EventShipDestroyed
                  += OnShipDestroyed;

                SystemManager.Events.EventStationDestroyed
                    += OnStationDestroyed;
            }
        }

        void OnDisable()
        {
            m_con.OnKeyPress -= PlayerSystemInput;
            m_con.OnKeyRelease -= PlayerSystemInputRelease;
            m_con.OnMouseScroll -= PlayerMouseScroll;
            m_con.PlayerEnterScene -= new SpaceManager.SceneEvent(LoadPlayerDataIntoScene);
            m_con.PlayerExitScene -= PlayerDeath;

            // Station events
            StationController.OnEnterRange -= OnEnterStation;
            StationController.OnExitRange -= OnExitStation;
            StationController.OnEnterBuildRange -= OnEnterBuildRange;
            StationController.OnExitBuildRange -= OnExitBuildRange;
            StationController.OnStationCreated -= OnStationCreated;

            if (SystemManager.Events != null)
            {
                SystemManager.Events.EventShipDestroyed
                -= OnShipDestroyed;

                SystemManager.Events.EventStationDestroyed
                    -= OnStationDestroyed;
            }
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
            if (m_att.Docked || key == KeyCode.None)
                return;

            if (key == Control_Config.GetKey("pause", "sys"))
            {
                m_util.Pause(true);
            }

            if (SystemManager.UIState.Current == UIState.Play)
            {
                if (key == Control_Config.GetKey("zoomOut", "sys"))
                {
                    m_util.ZoomOut();
                }

                if (key == Control_Config.GetKey("zoomOut", "sys"))
                {
                    m_util.ZoomIn();
                }
                if (key == Control_Config.GetKey("dock", "sys"))
                {
                    m_con.DockAtStation();
                }
                if(key == Control_Config.GetKey("interact", "sys"))
                {
                    m_con.InteractWithStation(true);
                }
                if (key == Control_Config.GetKey("camUp", "sys"))
                {
                    m_util.RotateCam(0, true);
                    m_con.ResetOrientation(new Vector2(0,1));
                }
                if (key == Control_Config.GetKey("camDown", "sys"))
                {
                    m_util.RotateCam(180f, true);
                    m_con.ResetOrientation(new Vector2(0, -1));
                }
                if (key == Control_Config.GetKey("camLeft", "sys"))
                {
                    m_util.RotateCam(270f, true);
                    m_con.ResetOrientation(new Vector2(-1, 0));
                }
                if (key == Control_Config.GetKey("camRight", "sys"))
                {
                    m_util.RotateCam(90f, true);
                    m_con.ResetOrientation(new Vector2(1, 0));
                }
            }
        }

        private void PlayerSystemInputRelease(KeyCode key)
        {
            if (key == Control_Config.GetKey("pause", "sys"))
            {
                m_util.PauseRelease();
            }
            if (key == Control_Config.GetKey("interact", "sys"))
            {
                m_con.InteractWithStation(false);
            }
            if (key == Control_Config.GetKey("camUp", "sys")
                || key == Control_Config.GetKey("camDown", "sys")
                || key == Control_Config.GetKey("camLeft", "sys")
                || key == Control_Config.GetKey("camRight", "sys"))
            {
                m_util.RotateRelease();
            }
        }

        /// <summary>
        /// Zoom the map with middle mouse wheel
        /// </summary>
        /// <param name="yDelta"></param>
        private void PlayerMouseScroll(float yDelta)
        {
            if (m_att.Docked)
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

            m_att.InRangeList.Clear();

            if(m_att.DockingStation != null)
            {
                m_att.DockingStation.Range(false);

                m_att.DockingStation.Interact(false);

                m_att.DockingStation.Dock(false);

                m_att.DockingStation = null;
            }


            if (m_att.InteractStation != null)
            {
                m_att.InteractStation.Range(false);

                m_att.InteractStation.Interact(false);

                m_att.InteractStation.Dock(false);

                m_att.InteractStation = null;
            }

            // Prompt player to pick a spawn
            SystemManager.UIState.SetState(UIState.SpawnPicker);

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

            m_att.PlayerCamera = GameObject.FindGameObjectWithTag
                    ("MainCamera").transform;

            if (m_att.PlayerShip != null)
            {
                m_att.netID = m_att.PlayerShip.GetComponent<NetworkIdentity>().
                    netId.Value;
            }

            // Set to popup gui
            SystemManager.UIState.SetState(UIState.Play);

            SystemManager.Background.StartBackground();
        }

        /// <summary>
        /// When player is in proximity of a station it can enter
        /// then keep reference tho the station
        /// </summary>
        /// <param name="controller"></param>
        private void OnEnterStation(StationAccessor station)
        {
            // if we arent already in range
            // add to our list
            if (!m_att.InRangeList.Contains(station))
            {
                m_att.InRangeList.Add(station);

                // retrieve ship atts from player object
                ShipAccessor ship = m_att.PlayerShip.
                    GetComponent<ShipAccessor>();

                station.Range(true, ship);

                // define the station reference 
                if (station.DockPrompt != null)
                {
                    if (m_att.DockingStation != null)
                        SystemManager.UIPrompt.HidePrompt
                            (m_att.DockingStation.DockPrompt.ID);

                    m_att.DockingStation = station;
                }

                if (station.InteractPrompt != null)
                {
                    if (m_att.InteractStation != null)
                        SystemManager.UIPrompt.HidePrompt
                            (m_att.InteractStation.InteractPrompt.ID);

                    m_att.InteractStation = station;
                }
            }
        }

        /// <summary>
        /// When player leaves station vicinity then clear reference
        /// </summary>
        /// <param name="controller"></param>
        private void OnExitStation(StationAccessor station)
        {
            if (m_att.InRangeList.Contains(station))
                m_att.InRangeList.Remove(station);

            station.Range(false);

            if (m_att.Docked)
                SystemManager.Space.LeaveStation();

            // if the prompts are null, consider 
            if (station.DockPrompt == null
                && m_att.DockingStation != null)
            {
                if (m_att.DockingStation.Equals(station))
                {
                    m_att.DockingStation = null;

                    foreach (StationAccessor other in m_att.InRangeList)
                    {
                        if (other.DockPrompt != null)
                        {
                            m_att.DockingStation = other;

                            Debug.Log("Replaced Dock");

                            SystemManager.UIPrompt.DisplayPrompt
                                (m_att.DockingStation.DockPrompt.ID);

                            break;
                        }
                    }
                }
            }

            if (station.InteractPrompt == null &&
                m_att.InteractStation != null)
            {
                if (m_att.InteractStation.Equals(station))
                {
                    m_att.InteractStation = null;

                    Debug.Log("Interact Removed");

                    foreach (StationAccessor other in m_att.InRangeList)
                    {
                        if (other.InteractPrompt != null)
                        {
                            m_att.InteractStation = other;

                            Debug.Log("Replaced Interact");

                            SystemManager.UIPrompt.DisplayPrompt
                                (m_att.InteractStation.InteractPrompt.ID);

                            break;
                        }
                    }
                }
            }
        }

        private void OnEnterBuildRange(StationAccessor station)
        {
            m_att.buildRange = true;
        }

        /// <summary>
        /// When player leaves station vicinity then clear reference
        /// </summary>
        /// <param name="controller"></param>
        private void OnExitBuildRange(StationAccessor station)
        {
            m_att.buildRange = false;
        }

        #endregion

        #region SPACE EVENTS

        /// <summary>
        /// Receives a message that a ship
        /// has been destroyed and processes the event
        /// </summary>
        /// <param name="input">Input.</param>
        public void OnShipDestroyed(DestroyDespatch destroyed)
        {
            /*if (destroyed.AggressorTag == "PlayerShip")
            {
                if (destroyed.AlignmentLabel == "Enemy")
                    SystemManager.GUI.DisplayMessege(new MsgParam("sm-green", "You have destroyed an enemy."));
                if (destroyed.AlignmentLabel == "Friendly")
                    SystemManager.GUI.DisplayMessege(new MsgParam("sm-red", "You have destroyed an friendly."));
            }*/
        }

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

        /// <summary>
        /// If a station is destroyed we remove it from 
        /// our global list
        /// </summary>
        /// <param name="DD"></param>
        public void OnStationDestroyed(DestroyDespatch DD)
        {
            if (!isServer)
                return;

            // Find the station in our list and remove it
            for(int i = 0; i < m_att.GlobalStations.Count; i++)
            {
                if(m_att.GlobalStations[i].netId == DD.SelfID)
                {
                    m_att.GlobalStations.RemoveAt(i);
                    break;
                }
            }

            // whatever done here
        }

        /// <summary>
        /// Adds the station to our segment station 
        /// reference
        /// </summary>
        /// <param name="station"></param>
        [Server]
        public void OnStationCreated(StationAccessor station)
        {
            if (station == null)
            {
                return;
            }
            if (!m_att.GlobalStations.Contains(station))
            {
                m_att.GlobalStations.Add(station);
            }
        }

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

        /// <summary>
        /// Called when player picks a team
        /// to initialize the game
        /// </summary>
        /// <param name="netMsg"></param>
        public void OnDefineTeam(NetworkMessage netMsg)
        {
            // Retrieve net id from sent message
            NetworkInstanceId teamID = new NetworkInstanceId(
                (uint)netMsg.ReadMessage<IntegerMessage>().value);

            // Get our local team object with the same netID
            GameObject teamObj = ClientScene.
                FindLocalObject(teamID);

            // extract the team manager and store int space object
            m_att.Team = teamObj.
                GetComponent<TeamController>();

            if (m_att.Team.ID == 1)
            {
                m_util.RotateCam(180, false);
                m_con.ResetOrientation(new Vector2(0, -1f));
            }

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

        /// <summary>
        /// Listens to a transaction
        /// from the server and applies it
        /// </summary>
        /// <param name="netMsg"></param>
        public void OnTransaction(NetworkMessage netMsg)
        {
            TransactionMessage transaction
                = netMsg.ReadMessage<TransactionMessage>();

            // Retreive values
            int currency = transaction.CurrencyAmount;

            ItemCollectionData[] assets = 
                transaction.Assets;

            // Discover if player is recipiant
            if (transaction.Recipiant == -1)
            {
                if (currency > 0)
                    m_util.AddFundingToPlayerData
                        (currency);
            }
        }

        #endregion
    }
}
