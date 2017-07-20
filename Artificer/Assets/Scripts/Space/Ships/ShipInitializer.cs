using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System;
using System.Collections;
using Serializer;
using Data.Space;
using Data.Space.Library;
using Space.Segment.Generator;
using Space.UI;
using Networking;
using System.Collections.Generic;
using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;
using Game;

namespace Space.Ship
{
    /// <summary>
    /// Builds the ship object
    /// upon creation following 
    /// criteria
    /// </summary>
    public class ShipInitializer : NetworkBehaviour
    {
        #region ATTRIBUTES

        private ShipAttributes m_att;

        #region GENERATION VARIABLES

        private List<int> m_addedIDs;

        #endregion

        #region CONTAINER

        [Header("Container")]
        private Transform m_shipContainer;

        #endregion

        #endregion

        #region MONO BEHAVIOUR

        public override void OnStartClient()
        {
            // Retrive container object
            m_shipContainer = 
                GameObject.Find("_ships").transform;

            transform.SetParent(m_shipContainer);

            // assign atriubutes for the ship
            m_att = GetComponent<ShipAttributes>();

            // Define transform if ship has been already created
            if (m_att.hasSpawned)
            {
                if (isLocalPlayer)
                    SetPlayer();
                else
                    SystemManager.Space.TeamSelected += SetNonPlayer;

                SystemManager.UI.AddUIPiece(transform);

                
            }
            else
                // Begin listening for the ship created 
                // event
                SystemManager.Events.EventShipCreated += ShipCreated;
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Initializes and Assigns ship attributes
        /// based on parameters
        /// </summary>
        [Server]
        public void AssignShipData(ShipData shipItem, int alignment)
        {
            // Assign the shipdata to the attributes
            m_att.Ship = shipItem;

            // Assign the team information to the attributes
            m_att.TeamID = alignment;

            // Assign the network instance for this ship
            m_att.NetworkID = netId;

            // Spawn the ship on server
            GenerateShip();

            // Add to other uis
            // will be replaces with listening to on ship create event?
            SystemManager.UI.RpcAddRemotePlayer(netId);

            SystemManager.GameMSG.OnShipCreated(netId, SystemManager.Space.ID);

            m_att.hasSpawned = true;
        }

        #endregion

        #region PRIVATE UTILITIES

        #region SET TRANSFORM

        /// <summary>
        /// Defines our player ship
        /// within its transform
        /// </summary>
        private void SetPlayer()
        {
            // We have the authority on this ship
            // we will need to check this is ours in future
            name = "PlayerShip";
            tag = "PlayerShip";

            gameObject.AddComponent<ShipPlayerInputController>();
            m_att.AlignmentLabel = "Player";

            SendMessage("BuildColliders");
        }

        /// <summary>
        /// Set the transform data
        /// as an NonPlayer object
        /// </summary>
        private void SetNonPlayer()
        {
            if (SystemManager.Space.Team)
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

                SystemManager.Space.TeamSelected -= SetNonPlayer;
            }
            else
                SystemManager.Space.TeamSelected += SetNonPlayer;
        }

        #endregion

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

            NetworkServer.SpawnWithClientAuthority(headGO, connectionToClient);

            ComponentListener head = headGO.GetComponent<ComponentListener>();

            head.InitializeData(m_att.Ship.Head, netId, NetworkInstanceId.Invalid, new SocketData());

            m_addedIDs.Add(m_att.Ship.Head.InstanceID);

            // build the body around this
            BuildConnectedPieces
                (m_att.Ship.Head, headGO.transform);
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
            (ComponentData component, Transform componentTransform)
        {
            SocketData[] socketList = component.sockets;

            // no sockets = no body
            if (socketList == null)
                return;

            foreach (SocketData socket in socketList)
            {
                // test we successfully found the socket 
                if (componentTransform.Find
                    (String.Format
                     ("socket_{0}", socket.SocketID)) == null)
                {
                    Debug.Log("Ship Initializer - " +
                            "BuildConnectedPieces:" +
                              "socket Transform not found - "
                              + socket.SocketID);
                    return;
                }

                // find the second piece through the socket
                ComponentData piece = m_att.Ship.GetComponent(socket.OtherID);
                if (piece.Path == "")
                {
                    Debug.Log("Ship Initializer - " +
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
                    Debug.Log("Ship Initializer - " +
                              "BuildConnectedPieces: other " +
                              "Could not find: " + piece.Path);
                    return;
                }

                // Add the component piece to the game world
                // and give it local player authority
                // which isn't always the pilot
                pieceGO =
                    Instantiate(Resources.Load
                                ("Space/Ships/" + piece.Path))
                            as GameObject;

                NetworkServer.SpawnWithClientAuthority
                    (pieceGO, connectionToClient);

                // Retrieve the component listener
                ComponentListener pieceCon =
                    pieceGO.GetComponent<ComponentListener>();

                NetworkInstanceId connectID = componentTransform.
                    GetComponent<NetworkIdentity>().netId;

                // Send the data the component needs to init on the
                // server
                pieceCon.InitializeData(piece, netId, connectID, socket);

                // Add this piece to the parent piece connector list
                componentTransform.gameObject.SendMessage
                    ("AddConnection", pieceGO.
                    GetComponent<NetworkIdentity>().netId);

                m_addedIDs.Add(piece.InstanceID);

                BuildConnectedPieces
                    (piece, pieceGO.transform);
            }
        }

        #endregion

        #endregion

        #region EVENTS

        /// <summary>
        /// Initializes ship object
        /// once creation event is triggered
        /// </summary>
        /// <param name="CD"></param>
        private void ShipCreated(CreateDispatch CD)
        {
            if (!(CD.Self == netId.Value))
                // this isn't our ship
                return;

            NetworkProximityChecker npc = gameObject.AddComponent<NetworkProximityChecker>();
            npc.checkMethod = NetworkProximityChecker.CheckMethod.Physics2D;

            // Check if this is the player ship
            if(isLocalPlayer)
                // this is a player ship
                SetPlayer();
            else
                // this is a non player ship
                SetNonPlayer();

            SystemManager.Events.EventShipCreated -= ShipCreated;
        }

        #endregion
    }
}

