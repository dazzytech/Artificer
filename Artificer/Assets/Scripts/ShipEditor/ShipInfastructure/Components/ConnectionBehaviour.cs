using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Construction.ShipEditor
{
    public class ConnectionBehaviour {

        
        public List<SocketBehaviour> sockets;                       // Sockets belonging to this object 
        public List<SocketBehaviour> connectedSockets;
        public List<SocketBehaviour> socketsToConfirm;
        public List<BaseShipComponent> collidedObjects;
        public List<BaseShipComponent> otherPieces;

        public bool snappedToOther;
        public bool ChangedSockets;


    	// Use this for initialization
    	public void Init (List<SocketBehaviour> socks) {
    		sockets = socks;
    		connectedSockets = new List<SocketBehaviour> ();
            socketsToConfirm = new List<SocketBehaviour>();
            collidedObjects = new List<BaseShipComponent>();
    		otherPieces = new List<BaseShipComponent> ();
    	}
    	
    	// Update is called once per frame
    	public void Tick ()
    	{
    		if (collidedObjects.Count != 0) {
    			snappedToOther = 
    				FindConnectingObjects ();					// attempt to snap object to other
    		}

    		if (socketsToConfirm.Count != 0) {
    			ClearPendingSockets();							// clear any pending sockets
    															// that are too far away
    		}

            collidedObjects.Clear();
    	}

    	public void LateTick()
    	{
    		if (connectedSockets.Count != 0)
    			ClearFarSockets ();								// clear any connected
    													// socket too far away
        }

    	/// <summary>
    	/// Checks for sockets on the opposite 
    	/// piece on all sides then attempts to connect the closest 
    	/// ones
    	/// </summary>
    	/// <returns><c>true</c>, if connecting 
    	/// objects was found, <c>false</c> otherwise.</returns>
    	private bool FindConnectingObjects()
    	{
    		List<SocketBehaviour> upSockets = new List<SocketBehaviour> ();
    		List<SocketBehaviour> downSockets = new List<SocketBehaviour> ();
    		List<SocketBehaviour> leftSockets = new List<SocketBehaviour> ();
    		List<SocketBehaviour> rightSockets = new List<SocketBehaviour> ();


    		foreach (BaseShipComponent b 
                     in collidedObjects)
    		{
    			if(otherPieces.Contains					            // do not test is already connected
    			   (b)) 							
    				continue;

    			List<SocketBehaviour> collidedSockets =				// Start by retreiving a list of all
    				b.Sockets;								        // game object 

    			Vector3 direction = 
    				PiecesUtility.GetMouseDirection
    					(b.GetComponent<RectTransform>().position);

    			PiecesUtility.ApplyDirection(direction.normalized, 			// retreive list of sockets that are
    			     collidedSockets, ref upSockets,				// facing each other from the two
    			     ref downSockets, ref leftSockets, 				// objects
    			     ref rightSockets);
    		}

    		List<SocketBehaviour> directionalSockets = null;

    		if (upSockets.Count != 0) {
    			directionalSockets = PiecesUtility.GetSocketsOfDirection
    					(sockets, SocketAttributes.Alignment.DOWN);

    			if(directionalSockets.Count != 0)
    				ConnectObjects(directionalSockets,
    			               upSockets);
    		}

    		if (downSockets.Count != 0) {
    			directionalSockets = PiecesUtility.GetSocketsOfDirection
    				(sockets, SocketAttributes.Alignment.UP);

    			if(directionalSockets.Count != 0)
    				ConnectObjects(directionalSockets,
    			               downSockets);
    		}

    		if (leftSockets.Count != 0) {
    			directionalSockets = PiecesUtility.GetSocketsOfDirection
    				(sockets, SocketAttributes.Alignment.RIGHT);
    			
    			if(directionalSockets.Count != 0)
    				ConnectObjects(directionalSockets,
    			               leftSockets);
    		}

    		if (rightSockets.Count != 0) {
    			directionalSockets = PiecesUtility.GetSocketsOfDirection
    				(sockets, SocketAttributes.Alignment.LEFT);
    			
    			if(directionalSockets.Count != 0)
    				ConnectObjects(directionalSockets,
    			               rightSockets);
    		}

    		return true;
    	}

    	/// <summary>
    	/// Add the closest socket pair 
    	/// to a pending list to be later confirmed
    	/// </summary>
    	/// <param name="sockets">Sockets.</param>
    	/// <param name="collidingSockets">Colliding sockets.</param>
    	private void ConnectObjects(List<SocketBehaviour> sockets, 
    	    List<SocketBehaviour> collidingSockets)
    	{
    		SocketBehaviour socketToAdd = 
    			PiecesUtility.FindClosestSockets
    				(sockets, collidingSockets);

    		if (!socketsToConfirm.Contains (socketToAdd))
    			socketsToConfirm.Add (socketToAdd);
    	}

    	/// <summary>
    	/// any pending sockets with a distance of more
    	/// than .5 units is deleted
    	/// </summary>
    	private void ClearPendingSockets()
    	{
    		List<SocketBehaviour> removeSockets = new List<SocketBehaviour> ();
    		foreach(SocketBehaviour socket in socketsToConfirm)
    		{
    			if(socket.connectedSocket != null)
    			{
    				SocketBehaviour attachedSocket = socket.connectedSocket;
    				if(Vector3.Distance(socket.position,
    				                    attachedSocket.position) > 50)
    				{
                        socket.Wipe();
    					removeSockets.Add(socket);
    				}
    			}
                else
                {
                    socket.Wipe();
                    removeSockets.Add(socket);
                }
    		}
    		foreach (SocketBehaviour socket in removeSockets) {
    			socketsToConfirm.Remove(socket);
    		}
    	}

    	/// <summary>
    	/// Checks the distance of connected sockets 
    	/// and disconnects any with a distance longer
    	/// than .1 units.
    	/// </summary>
    	private void ClearFarSockets()
    	{
    		List<SocketBehaviour> removeSockets 						// Create a new list for sole purpose
    			= new List<SocketBehaviour> ();							// of deletion
    		foreach(SocketBehaviour socket in
    		        connectedSockets)
    		{
                if(socket.connectedSocket == null)
                {
                    socket.Wipe();
                    removeSockets.Add(socket);
                    continue;
                }
    			SocketBehaviour attachedSocket 
    				= socket.connectedSocket;
    			if(Vector3.Distance(socket.position,
    			           attachedSocket.position) > 10)
    			{
    				socket.Wipe();
    				otherPieces.Remove
    					(attachedSocket.container);
    				removeSockets.Add(socket);					
    			}
    		}

    		foreach (SocketBehaviour socket in removeSockets) {
    		    connectedSockets.Remove(socket);			// Delete the sockets from storage
    		}
    	}

    	/// <summary>
    	/// Takes sockets stored within the pending 
    	/// container and confirms them as actual connection 
    	/// adds them to currently connected list.
    	/// </summary>
        public void Confirm()
        {
            // sort in order from longest distance to shortist?
            socketsToConfirm = socketsToConfirm.OrderBy(o=>o.distance).Reverse().ToList();

            foreach (SocketBehaviour socket in 
                     socketsToConfirm) 
            {
                if(socket.connected == false)
                    ChangedSockets = true;

                socket.Connect();               // move sockets pending to the connected pointer
                
                connectedSockets.Add (socket);
                otherPieces.Add
                    (socket.connectedSocket.container);
                socket.Tick();

            }
            
            socketsToConfirm.Clear ();                      // clear out the current pending list
        }

    	/// <summary>
    	/// When the player drags a piece this function
    	/// turns all the sockets to male so the 
    	/// connected pieces do not follow
    	/// </summary>
    	public void SetDraggedPiece()
    	{
    		foreach (SocketBehaviour socket in sockets) {
    			socket.SetMale ();
    		}
    	}

        /// <summary>
        /// When ships are loading
        /// this utiilty will connect said pieces
        /// without the need for dragging and colliding
        /// </summary>
        /// <param name="socketID">Socket I.</param>
        /// <param name="other">Other.</param>
        public void ConnectToPiece(int socketID, SocketBehaviour other)
        {
            foreach (SocketBehaviour s in sockets)
            {
                int sID = int.Parse(s.SocketID);
                if(sID == socketID)
                {
                    // this the socket to add
                    s.CreatePending(other);
                    s.Connect();               // move sockets pending to the connected pointer
                    s.SnapToSocket(s.position, other.position);
                    
                    connectedSockets.Add (s);
                    otherPieces.Add
                        (s.connectedSocket.container);
                    break;
                }
            }
        }
    }
}
