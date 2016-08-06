using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

using Data.Space;
using Data.Shared;
using Space.Ship;
using Space.UI;

namespace Space.Segment
{
    [RequireComponent(typeof(SegmentGenerator))]
    /// <summary>
    /// Segment behaviour.
    /// responsible for automated tasks and dispatching events
    /// e.g. What segment the player is currently, loading and unloading data
    /// </summary>
    public class SegmentBehaviour 
        : NetworkBehaviour
    {
        private SegmentGenerator _gen;
        private SegmentAttributes _att;
        
        void Awake()
        {
            _gen = GetComponent<SegmentGenerator>();
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

        /// <summary>
        /// Raises the start client event.
        /// Builds the space segment before the player 
        /// object is created
        /// </summary>
        public override void OnStartClient()
        {
            Debug.Log("Segment Start Local Player");

            EnterSegment();
        }

        /// <summary>
        /// Run here because segment is independant of game parameters
        /// </summary>
        public override void OnStartServer()
        {
            InitializeSegment();
        }

        /*(public override void OnStartAuthority()
        {
            Debug.Log("Authority Start");

            //base.OnStartServer();
          * 
        }*/

        void Start()
        {
            // Segment is seperate from game functions 
            // so is encapsulated

            //_att.SegObjs.Callback += SyncListGOAdded;

            //if (isServer)
              //  InitializeSegment();
        }

        // Stores space
        Texture2D playerCursorCombat;
        Texture2D playerCursorTurning;

        /// <summary>
        /// Builds the space enviroment is server
        /// and initializes playercam and space generation
        /// for clients
        /// </summary>
        private void EnterSegment()
        {
           // GameManager.GUI.DisplayMessege(new MsgParam("bold",
               // "Initializing server space segment - User Count: " + (++_att.playerCount).ToString()));

           // GameManager.GUI.DisplayMessege(new MsgParam("bold", "Retreiving space."));

            _gen.GenerateBase();

            //MessageHUD.DisplayMessege(new MsgParam("md-green", "\nObject Count: " + _att.SegObjs.Count));

            //_gen.GenerateSegment(_att.Objects);

            // initialize cursor
            playerCursorCombat = 
                Resources.Load ("Textures/playerCursorCombat") 
                    as Texture2D;
            playerCursorTurning = 
                Resources.Load ("Textures/playerCursorTurning") 
                    as Texture2D;
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
            //GameManager.GUI.DisplayMessege(new MsgParam("bold", "Generating space..."));
            //SegmentDataBuilder.BuildNewSegment(_att.Objects);
           //MessageHUD.DisplayMessege(new MsgParam("bold", "Finished!"));
        }

        public void SyncListGOAdded(SyncListSO.Operation op, int index)
        {
            Debug.Log("list changed " + op);

            /*MessageHUD.DisplayMessege(new MsgParam("sm-green", ("\nName: " + _att.SegObjs[index]._name +
                 ", Position: " + _att.SegObjs[index]._position.ToString() +
                 ", Type: " + _att.SegObjs[index]._type + "\n")));*/
        }

        /// <summary>
        /// Raises the player update event.
        /// detects to see if player is leaving the area.
        /// also updates cursor
        /// </summary>
        /// <param name="player">Player.</param>
        private void OnPlayerUpdate(Transform playerShip)
        {
            // Check that player is within bounds of current segment
            if (!_att.MapBounds.Contains(playerShip.transform.position))
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
            }

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
    }
}

