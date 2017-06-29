using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System;
using System.Collections;
using Serializer;
using Data.Shared;
using Data.Space.Library;
using Space.Segment.Generator;
using Space.UI;
using Networking;
using System.Collections.Generic;
using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;

namespace Space.Ship
{
    /// <summary>
    /// Builds the ship object
    /// upon creation following 
    /// criteria
    /// </summary>
    public class ShipInitializer : NetworkBehaviour
    {
        #region EVENTS

        public delegate void ShipEvent(ShipAttributes Ship);

        public static event ShipEvent OnShipCreated;

        #endregion

        #region ATTRIBUTES

        private ShipAttributes m_att;

        #region GENERATION VARIABLES

        private List<int> m_addedIDs;

        [SyncVar]
        public bool hasSpawned;

        #endregion

        #region CONTAINER

        [Header("Container")]
        private Transform m_shipContainer;

        #endregion

        #endregion

        #region ONSTART OVERRIDES

        /// <summary>
        /// Builds the ship if it was build on server already
        /// </summary>
        public override void OnStartClient()
        {
            // This player may have spawned before this client joined the game, 
            // so if that's the case, spawn it now. Otherwise, just wait for the RpcSpawnMe call.
            if (hasSpawned)
            {
                SetUpPlayer(SystemManager.Space.TeamID);
                // Add this item to local UI list
                // p.s should be performed on each client without cmd
                SystemManager.UI.AddUIPiece(transform);

                if (OnShipCreated != null)
                    OnShipCreated(m_att);
            }

            transform.SetParent(m_shipContainer);
        }

        #endregion

        #region MONO BEHAVIOUR

        private void Awake()
        {
            // Retrive container object
            m_shipContainer = 
                GameObject.Find("_ships").transform;

            // assign atriubutes for the ship
            m_att = GetComponent<ShipAttributes>();
        }

        void Start()
        {
            //NetworkProximityChecker npc = gameObject.AddComponent<NetworkProximityChecker>();
            //npc.checkMethod = NetworkProximityChecker.CheckMethod.Physics2D;

            // we will add player interaction if this is our ship
            if (isLocalPlayer)
            {
                // Set after ship created
                // we will need to check this is ours in future
                //GameObject.Find("PlayerCamera").SendMessage("SetFollowObj",
                                   //                       transform.GetChild(0));

                name = "PlayerShip";
                tag = "PlayerShip";

                gameObject.AddComponent<ShipPlayerInputController>();
                m_att.AlignmentLabel = "Player";

                //SendMessage("BuildColliders");
            }
            // Any ships spawned before we pick team will need
            // assigning when team is assigned
            else if (SystemManager.Space.Team)
            {
                // Determine tag based on our reference to team
                if (SystemManager.Space.Team.PlayerOnTeam(netId))
                {
                    name = "AllyShip";
                    tag = "Friendly";
                }
                else
                {
                    name = "EnemyShip";
                    tag = "Enemy";
                }
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Initializes and Assigns ship attributes
        /// based on parameters
        /// </summary>
        [Server]
        public void AssignShipData(string shipItem, int alignment)
        {
            // Assign the shipdata to the attributes
            m_att.Ship = ShipLibrary.GetShip(shipItem);

            // Assign the team information to the attributes
            m_att.TeamID = alignment;

            // Spawn the ship on server
            GenerateShip();

            // Add to other uis
            SystemManager.UI.RpcAddRemotePlayer(netId);
        }

        #endregion

        #region SHIP INITIALIZATION

        /// <summary>
        /// Assigns the passed ship data
        /// to the player's own ship data variable
        /// </summary>
        /// <param name="a_data">ship data to pass</param>
        private void OnSpawnMe(NetworkMessage netMsg)
        {
            string shipName = netMsg.
                ReadMessage<StringMessage>().value;
            
            CmdSpawnMe(shipName, SystemManager.Space.TeamID);
        }

        [Command]
        private void CmdSpawnMe(string shipName, int teamID)
        {
            if (hasSpawned)
                return; 

            hasSpawned = true;
            RpcSpawnMe(shipName, teamID);

            SystemManager.UI.RpcAddRemotePlayer(netId);
        }

        /// <summary>
        /// Sent to each client to instruct them to build the ship
        /// </summary>
        /// <param name="a_data">ship data to pass</param>
        [ClientRpc]
        private void RpcSpawnMe(string shipName, int teamID)
        {
            //spawnData will be synced by the server automatically,
            //but I don't trust it to arrive before this call, so I pass it into
            //this function anyway to be sure.
            SetUpPlayer(teamID);
            

            if(OnShipCreated != null)
                OnShipCreated(m_att);
        }

        /// <summary>
        /// Construct the ship object for the player
        /// </summary>
        private void SetUpPlayer(int teamID)
        {
            
            m_att.instID = netId;
            m_att.TeamID = teamID;

            //Build the object with spawnData
            //ShipGenerator.GenerateShip(ShipLibrary.GetShip(Ship), this.gameObject);

            // add network proximity checker
            NetworkProximityChecker npc = gameObject.AddComponent<NetworkProximityChecker>();
            npc.checkMethod = NetworkProximityChecker.CheckMethod.Physics2D;

            // we will add player interaction if this is our ship
            if (isLocalPlayer)
            {
                // we will need to check this is ours in future
                GameObject.Find("PlayerCamera").SendMessage("SetFollowObj",
                                                            transform.GetChild(0));

                name = "PlayerShip";
                tag = "PlayerShip";

                gameObject.AddComponent<ShipPlayerInputController>();
                m_att.AlignmentLabel = "Player";

                SendMessage("BuildColliders");
            }
            // Any ships spawned before we pick team will need
            // assigning when team is assigned
            else if (SystemManager.Space.Team)
            {
                // Determine tag based on our reference to team
                if (SystemManager.Space.Team.PlayerOnTeam(netId))
                {
                    name = "AllyShip";
                    tag = "Friendly";
                }
                else
                {
                    name = "EnemyShip";
                    tag = "Enemy";
                }
            }
        }

        [Command]
        private void CmdUpdateHUD()
        {
            SystemManager.UI.RpcAddRemotePlayer(netId);
            RpcFinishCreate();
        }

        [ClientRpc]
        private void RpcFinishCreate()
        {
            if (OnShipCreated != null)
                OnShipCreated(m_att);
        }

        #endregion

        #region PRIVATE UTILITIES

        #region SHIP GENERATION

        /// <summary>
        /// When Attributes have been assigned then
        /// this command with generate the ship
        /// </summary>
        /// <returns>The AI ship.</returns>
        /// <param name="ship">ShipData template 
        /// to ship the ship with.</param>
        [Server]
        public void GenerateShip
            ()
        {
            // create list for storing info
            m_addedIDs = new List<int>();

            // Add head to this game object
            GameObject headGO =
                Instantiate(Resources.Load
                            ("Space/Ships/" + m_att.Ship.Head.Path))
                    as GameObject;

            // set transform
            headGO.transform.parent = transform;
            headGO.transform.localPosition = Vector3.zero;
            headGO.tag = "Head";
            headGO.SendMessage("SetStyle", m_att.Ship.Head.Style,
                               SendMessageOptions.DontRequireReceiver);
            headGO.SendMessage("SetID", m_att.Ship.Head.InstanceID,
                SendMessageOptions.DontRequireReceiver);

            m_addedIDs.Add(m_att.Ship.Head.InstanceID);

            // build the body around this
            ShipGenerator.BuildConnectedPieces
                (m_att.Ship.Head, headGO.transform, m_att.Ship);

            //baseShip.GetComponent<ShipAttributes>().Ship = ship;
            SendMessage("AddComponentsToList",
                SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// Builds the connected pieces.
        /// Takes a piece and constructs all 
        /// connected pieces and 
        /// works recursively
        /// </summary>
        /// <param name="component">Component.</param>
        /// <param name="componentTransform">Component transform.</param>
        /// <param name="ship">Ship.</param>
        [Server]
        public void BuildConnectedPieces
            (Data.Shared.Component component, Transform componentTransform)
        {
            Vector3 otherPos;
            Vector3 thisPos;

            Socket[] socketList = component.sockets;

            // no sockets = no body
            if (socketList == null)
                return;

            foreach (Socket socket in socketList)
            {
                // Get position of this socket
                // through the components transform
                Transform thisTrans = componentTransform.Find
                    (String.Format
                     ("socket_{0}", socket.SocketID));

                // test we successfully found the socket 
                if (thisTrans == null)
                {
                    Debug.Log("Ship Generator - " +
                            "BuildConnectedPieces:" +
                              "socket Transform not found - "
                              + socket.SocketID);
                    return;
                }

                thisPos =
                    componentTransform.Find
                        (String.Format
                         ("socket_{0}", socket.SocketID)).position;

                // find the second piece through the socket
                Data.Shared.Component piece = m_att.Ship.GetComponent(socket.OtherID);
                if (piece.Path == "")
                {
                    Debug.Log("Ship Generator - " +
                        "BuildConnectedPieces: other " +
                        "socket not found!");
                    return;
                }

                // test we haven't already added this piece.
                // stops unending loops
                if (m_addedIDs.Contains(piece.InstanceID))
                    continue;

                // create the piece
                GameObject pieceGO = null;

                // Test if we actually successfully created the new piece
                if (Resources.Load
                    ("Space/Ships/" + piece.Path) == null)
                {
                    Debug.Log("Ship Generator - " +
                              "BuildConnectedPieces: other " +
                              "Could not find: " + piece.Path);
                    return;
                }

                //Add the component piece to the game world
                pieceGO =
                    Instantiate(Resources.Load
                                ("Space/Ships/" + piece.Path))
                            as GameObject;

                // Set the direction of the new piece
                Vector3 dirEuler = new Vector3(0, 0, 0);
                switch (piece.Direction)
                {
                    case "up":
                        dirEuler.z = 0f; break;
                    case "down":
                        dirEuler.z = 180f; break;
                    case "left":
                        dirEuler.z = 90; break;
                    case "right":
                        dirEuler.z = 270f; break;
                }

                // Apply direction to obj and sockets
                pieceGO.transform.eulerAngles = dirEuler;

                // Set trigger - none interactive components will ignore this
                pieceGO.SendMessage("SetTriggerKey", piece.Trigger,
                                    SendMessageOptions.DontRequireReceiver);

                // Combat trigger - activation key when the ship enters combat mode
                if (piece.CTrigger != null)
                    pieceGO.SendMessage("SetCombatKey", piece.CTrigger,
                                    SendMessageOptions.DontRequireReceiver);

                // Sets the ship's visual style
                pieceGO.SendMessage("SetStyle", piece.Style,
                                    SendMessageOptions.DontRequireReceiver);

                pieceGO.SendMessage("SetID", piece.InstanceID,
                                    SendMessageOptions.DontRequireReceiver);

                // make child of ship
                pieceGO.transform.parent =
                    componentTransform.parent;

                // find position of other piece and then
                // snap the pieces together
                otherPos = pieceGO.transform.Find
                    (String.Format
                     ("socket_{0}", socket.OtherLinkID)).position;

                Vector3 snapDistance = otherPos - thisPos;

                pieceGO.transform.position -= snapDistance;

                // Initiailize the connector list for this piece
                pieceGO.SendMessage("InitCL");

                pieceGO.SendMessage("SetSock", socket);

                pieceGO.SendMessage("LockTo", componentTransform);

                // Add this piece to the parent piece connector list
                componentTransform.gameObject.SendMessage
                    ("AddConnection", pieceGO.GetComponent<ComponentListener>());

                // Finally assign autolock and autofire if component is the right type
                if (piece.Folder == "Launchers")
                {
                    pieceGO.GetComponent<LauncherAttributes>().AutoTarget =
                        piece.AutoLock;
                }

                if (piece.Folder == "Targeter")
                {
                    pieceGO.GetComponent<TargeterAttributes>().Behaviour =
                        (TargeterBehaviour)piece.behaviour;

                    pieceGO.GetComponent<TargeterAttributes>().EngageFire =
                        piece.AutoFire;

                    pieceGO.GetComponent<TargeterAttributes>().homeForward = pieceGO.transform.up;
                }

                m_addedIDs.Add(piece.InstanceID);

                BuildConnectedPieces
                    (piece, pieceGO.transform);
            }
        }

        #endregion

        #endregion
    }
}

