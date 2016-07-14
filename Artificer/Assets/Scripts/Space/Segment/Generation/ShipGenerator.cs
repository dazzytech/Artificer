using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
// Artificer Defined
using Data.Shared;
using Space.Ship;
using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;

namespace Space.Segment.Generator
{
    public class ShipGenerator : NetworkBehaviour
    {
        static List<int> addedIDs;

        static Transform Base;

    	/// <summary>
    	/// Creates player prefab then adds
    	/// all other objects as children
    	/// </summary>
    	/// <param name="">.</param>
    	/*public GameObject GeneratePlayerShip(ShipData ship, Vector3 pos, Vector2 up)
    	{
            GameObject baseShip = GenerateShip(ship, pos, up);

            // For now forget this part

            return baseShip;
    	}*/

        void Awake()
        {
            Base = this.transform;
        }

        /// <summary>
        /// Generates an empty ship.
        /// and returns the transform for the ai to
        /// add it's FSM
        /// </summary>
        /// <returns>The AI ship.</returns>
        /// <param name="ship">Ship.</param>
        /// <param name="pos">Position.</param>
        /// <param name="up">Up.</param>
        public static void GenerateShip
            (ShipData ship, GameObject baseShip)
        {
            // create list for storing info
            addedIDs = new List<int>();
            // Create a base ship from empty prefab template
            /*GameObject baseShip = 
                Instantiate(Resources.Load
                            ("Space/Ships/Ship"), pos,
                            Quaternion.identity) as GameObject;*/
            
            // Set parent of empty ship
            baseShip.transform.parent = Base;
            
            // POPULATE EMPTY SHIP
            // Add head 
            // Create game object
            GameObject headGO =
                Instantiate(Resources.Load
                            ("Space/Ships/" + ship.Head.Path))
                    as GameObject;

            // set transform
            headGO.transform.parent = baseShip.transform;
            headGO.transform.localPosition = Vector3.zero;
            headGO.tag = "Head";
            headGO.SendMessage("SetStyle", ship.Head.Style,
                               SendMessageOptions.DontRequireReceiver);
            headGO.SendMessage("SetID", ship.Head.InstanceID,
                SendMessageOptions.DontRequireReceiver);

            addedIDs.Add(ship.Head.InstanceID);

            // build the body around this
            ShipGenerator.BuildConnectedPieces 
                (ship.Head, headGO.transform, ship);

            baseShip.GetComponent<ShipAttributes>().Ship = ship;
            baseShip.SendMessage("AddComponentsToList");
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
    	public static void BuildConnectedPieces
            (Data.Shared.Component component, Transform componentTransform,
    		 ShipData ship)
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
    			if(thisTrans == null)
    			{
    				Debug.Log("Ship Generator - " +
    						"BuildConnectedPieces:"+
    				          "socket Transform not found - " 
    				          + socket.SocketID);
    				return;
    			}

    			thisPos = 
    				componentTransform.Find 
    					(String.Format 
    					 ("socket_{0}", socket.SocketID)).position;

    			// find the second piece through the socket
                Data.Shared.Component piece = ship.GetComponent (socket.OtherID);
    			if(piece.Path == "")
    			{
    				Debug.Log("Ship Generator - " +
    					"BuildConnectedPieces: other " +
    					"socket not found!");
    				return;
    			}

                // test we haven't already added this piece.
                // stops unending loops
                if(addedIDs.Contains(piece.InstanceID))
                    continue;

    			// create the piece
    			GameObject pieceGO = null;

                // Test if we actually successfully created the new piece
                if(Resources.Load
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
    			Vector3 dirEuler = new Vector3(0,0,0);
    			switch(piece.Direction)
    			{
    			case "up":
                    dirEuler.z = 0f; break;
    			case "down":
                        dirEuler.z = 180f;break;
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
                if(piece.CTrigger != null)
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
                if(piece.Folder == "Launchers")
                {
                    pieceGO.GetComponent<LauncherAttributes>().AutoTarget =
                        piece.AutoLock;
                }

                if(piece.Folder == "Targeter")
                {
                    pieceGO.GetComponent<TargeterAttributes>().Behaviour =
                        (TargeterBehaviour)piece.behaviour;

                    pieceGO.GetComponent<TargeterAttributes>().EngageFire =
                        piece.AutoFire;

                    pieceGO.GetComponent<TargeterAttributes>().homeForward = pieceGO.transform.up;
                }

                addedIDs.Add(piece.InstanceID);

    			ShipGenerator.BuildConnectedPieces 
    				(piece, pieceGO.transform, ship);
    		}
    	}
    }
}

