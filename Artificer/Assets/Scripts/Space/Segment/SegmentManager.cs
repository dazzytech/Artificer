using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

using Data.Space;
using Data.Shared;
using Space.CameraUtils;
using Space.Ship;
using Space.UI;

namespace Space.Segment
{
    [RequireComponent(typeof(SegmentObjectManager))]
    /// <summary>
    /// Segment behaviour.
    /// responsible for automated tasks and dispatching events
    /// e.g. What segment the player is currently, loading and unloading data
    /// </summary>
    public class SegmentManager 
        : NetworkBehaviour
    {
        #region NESTED SYNCLIST CLASS

        // sync list callback
        public void SyncListGOAdded(SyncListSO.Operation op, int index)
        {
            /*MessageHUD.DisplayMessege(new MsgParam("sm-green", ("\nName: " + _att.SegObjs[index]._name +
                 ", Position: " + _att.SegObjs[index]._position.ToString() +
                 ", Type: " + _att.SegObjs[index]._type + "\n")));*/
        }

        #endregion

        #region ATTRIBUTES

        private SegmentObjectManager _gen;
        private SegmentAttributes _att;

        private bool _segInit;

        // Stores textures for player colours
        private Texture2D playerCursorCombat;
        private Texture2D playerCursorTurning;

        #endregion

        #region MONO BEHAVIOUR

        void Awake()
        {
            _gen = GetComponent<SegmentObjectManager>();
            _att = GetComponent<SegmentAttributes>();           
        }

        void OnEnable()
        {
            SpaceManager.OnPlayerUpdate += OnPlayerUpdate;
        }
        
        void OnDisable()
        {
            SpaceManager.OnPlayerUpdate -= OnPlayerUpdate;

            // clear combat cursor
            Cursor.SetCursor(null, new Vector2(16, 16), CursorMode.Auto);
        }

        void Start()
        {
            // Segment is seperate from game functions 
            // so is encapsulated

            _att.SegObjs.Callback += SyncListGOAdded;

            // initialize cursor
            playerCursorCombat =
                Resources.Load("Textures/playerCursorCombat")
                    as Texture2D;
            playerCursorTurning =
                Resources.Load("Textures/playerCursorTurning")
                    as Texture2D;

            _segInit = false;
        }

        #endregion

        #region NETWORK BEHAVIOUR

        /// <summary>
        /// Builds the space segment before the player 
        /// object is created
        /// </summary>
        public override void OnStartClient()
        {
            _gen.GenerateBase();
        }

        /// <summary>
        /// Run here because segment is independant of game parameters
        /// </summary>
        public override void OnStartServer()
        {
            InitializeSegment();
        }

        #endregion

        #region SEGMENT INITIALIZATION

        /// <summary>
        /// Starts the process of making 
        /// objects within range visable
        /// </summary>
        private void EnterSegment()
        {
            _gen.StartSegmentCycle();
            _segInit = true;
        }

        /// <summary>
        /// Builds the space enviroment is server
        /// and initializes playercam and space generation
        /// for clients
        /// </summary>
        [Server]
        private void InitializeSegment()
        {
            // Initialize space segment if not created - server's job
            //SystemManager.GUI.DisplayMessege(new MsgParam("bold", "Generating space..."));
            SegmentObject[] sObjs = SegmentDataBuilder.BuildNewSegment();

            foreach (SegmentObject sObj in sObjs)
                _att.SegObjs.Add(sObj);
            //MessageHUD.DisplayMessege(new MsgParam("bold", "Finished!"));

            _gen.GenerateServerObjects();

            // Initialize parellax objects
            SyncPI[] pItems = SegmentDataBuilder.BuildNewBackground();


            foreach (SyncPI pItem in pItems)
                _att.BGItem.Add(pItem);
        }

        #endregion

        #region EVENT LISTENER

        /// <summary>
        /// Raises the player update event.
        /// also updates cursor
        /// </summary>
        /// <param name="player">Player.</param>
        private void OnPlayerUpdate(Transform playerShip)
        {
            // initialize segment 
            if (!_segInit)
                EnterSegment();

            // Check that player is within bounds of current segment
            /*if (!_att.MapBounds.Contains(playerShip.transform.position))
            {
                Vector2 temp = playerShip.transform.position;
                // Boundary detection
                if (playerShip.transform.position.x < 0) 
                    temp.x -= 1;
                else if (playerShip.transform.position.x >= _att.MapSize.x)
                    temp.x += 1;
                
                
                if (playerShip.transform.position.y < 0) 
                    temp.y -= 1;
                else if (playerShip.transform.position.y >= _att.MapSize.y)
                    temp.y += 1;
            }*/

            // retrive ship attributes
            ShipData ship = playerShip.GetComponent<ShipAttributes>().Ship;

            if (ship.CombatActive && Time.timeScale != 0f)
            {
                if(ship.Aligned)
                    // Set Cursor
                    Cursor.SetCursor(playerCursorCombat,
                        new Vector2(8, 8), CursorMode.Auto);
                else
                    // Set Cursor
                    Cursor.SetCursor(playerCursorTurning,
                                     new Vector2(8, 8), CursorMode.Auto);
            } else
            {
                // clear combat cursor
                Cursor.SetCursor(null, new Vector2(16, 16), CursorMode.Auto);
            }
        }

        #endregion
    }
}

