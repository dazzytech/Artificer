using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
// Artificer Defined
using ShipComponents;
using Data.Shared;

public class ShipGenerator : MonoBehaviour
{
    GameObject home;

    List<int> addedIDs;

    public void GenerateBase()
    {
        home = new GameObject();
        home.name = "_ships";
        home.transform.parent = this.transform;
    }
	/// <summary>
	/// Creates player prefab then adds
	/// all other objects as children
	/// </summary>
	/// <param name="">.</param>
	public void GeneratePlayerShip(PlayerData player, Vector3 pos, Vector2 up)
	{
        ShipData ship = player.Ship;

        Transform baseShip = GenerateShip(ship, pos, up);

		baseShip.tag = "PlayerShip";
        baseShip.name = "PlayerShip";

		CreatePlayerCamera (baseShip.GetChild(0).gameObject);
		baseShip.gameObject.AddComponent<ShipPlayerInputController>();
        baseShip.gameObject.AddComponent<ShipExternalController>();
        baseShip.GetComponent<ShipAttributes>().FactionLabel = "Player";
        baseShip.GetComponent<ShipAttributes>().Player = player;
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
    public Transform GenerateShip
        (ShipData ship, Vector3 pos, Vector2 up)
    {
        // create list for storing info
        addedIDs = new List<int>();
        // Create a base ship from empty prefab template
        GameObject baseShip = 
            Instantiate(Resources.Load
                        ("Space/Ships/Ship"), pos,
                        Quaternion.identity) as GameObject;
        
        // Set parent of empty ship
        baseShip.transform.parent = home.transform;
        
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

        // build the body around this
        BuildConnectedPieces 
            (ship.Head, headGO.transform, ship);

        baseShip.SendMessage ("AddComponentsToList");
        baseShip.transform.up = up;

        return baseShip.transform;
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
	public void BuildConnectedPieces
        (Data.Shared.Component component, Transform componentTransform,
		 ShipData ship)
	{
		Vector3 otherPos;
		Vector3 thisPos;

		List<Socket> socketList = component.GetSockets ();

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
			if(piece == null)
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
			Vector3 dir = new Vector3();
			switch(piece.Direction)
			{
			case "up":
				dir = Vector3.up; break;
			case "down":
				dir = Vector3.down; break;
			case "left":
				dir = Vector3.left; break;
			case "right":
				dir = Vector3.right; break;
			}
			pieceGO.transform.up = dir;

            // Set trigger - none interactive components will ignore this
			pieceGO.SendMessage("SetTriggerKey", piece.Trigger,
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

            addedIDs.Add(piece.InstanceID);

			BuildConnectedPieces 
				(piece, pieceGO.transform, ship);
		}
	}

	private void CreatePlayerCamera(GameObject GOToFollow)
	{
		// ASSIGN MAIN CAMERA
		if (Camera.main == null)
        {
			
			// create the game object for the camera
			GameObject camObject = new GameObject();
            //camObject.transform.position = GOToFollow.transform.position;
            camObject.transform.Translate(new Vector3(0,0, -10f));
			camObject.transform.parent = this.transform;
			camObject.name = "PlayerCamera";
			camObject.tag = "MainCamera";
			
			// create and format the camera
			Camera playerCam = camObject.AddComponent<Camera> ();
			playerCam.clearFlags = CameraClearFlags.Color;
			playerCam.backgroundColor = Color.black;
			playerCam.orthographic = true;
			playerCam.orthographicSize = 10;

			// add and initialize scripts for the camera object
			CameraFollow camFollow = 
				camObject.AddComponent<CameraFollow>();
			camFollow.SetFollowObj(GOToFollow.transform);
            camObject.AddComponent<CameraShake>();

            camObject.AddComponent<AudioListener>();
		}
	}
}

