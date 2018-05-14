using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ArtificerEditor
{
    public class PiecesUtility
    {

        /// <summary>
        /// Retreive mouse coordinates
        ///  in the world, -10 in the z-axis
        ///  counteracts the z value of the camera
        /// </summary>
        /// <returns>The mouse position.</returns>
        /// <param name="colliderObject">Collider object.</param>
        public static Vector2 GetMouseDirection(Vector2 position)
        {
           // return  (Camera.main.ScreenToWorldPoint (Input.mousePosition)
             //        - new Vector3(0,0, -10f)) -  position;

            return  (ShipEditor._mousePos
                             - new Vector2(0,0)) -  position;
        }

    	/// <summary>
    	/// Retrieves the sockets from a given list
    	/// that match the required direction and 
    	/// additionally will only return free
    	/// sockets.
    	/// </summary>
    	/// <returns>The sockets of direction.</returns>
    	/// <param name="sockets">Sockets list.</param>
    	/// <param name="alignment">Alignment of sockets returned.</param>
    	public static List<SocketBehaviour> GetSocketsOfDirection
    		(List<SocketBehaviour> sockets, 
    		 SocketAttributes.Alignment alignment)
    	{
    		List<SocketBehaviour> newList = new List<SocketBehaviour> ();
    		
    		foreach (SocketBehaviour socket in sockets) {
    			if (socket.alignment == alignment &&
    			    socket.state != SocketAttributes.SocketState.CLOSED)
    				newList.Add (socket);
    		}
    		
    		return newList;
    	}

    	/// <summary>
    	/// Populates the directional socket list
    	/// based on the direction of collision
    	/// perspective of collided object
    	/// </summary>
    	/// <param name="direction">Direction.</param>
    	public static void ApplyDirection
    		(Vector3 direction,  
    		 List<SocketBehaviour> sockets,
    		 ref List<SocketBehaviour> upSockets,
    		 ref List<SocketBehaviour> downSockets,
    		 ref List<SocketBehaviour> leftSockets,
    		 ref List<SocketBehaviour> rightSockets)
    	{
    		if (Mathf.Abs (direction.y) > Mathf.Abs (direction.x)) {
    			// Collision on Y Axis
    			if(Mathf.Sign (direction.y) == -1)					// Object has a collision on top 
    			{
    				upSockets.AddRange(GetSocketsOfDirection(sockets,  
    					 SocketAttributes.Alignment.UP));
    			}
    			else  												// object has a collision on bottom 
    			{
    				downSockets.AddRange(GetSocketsOfDirection(sockets,  
    					 SocketAttributes.Alignment.DOWN));
    			}
    		} else {
    			// Collision on X Axis
    			if(Mathf.Sign (direction.x) == 1)					// object has a collision on right 
    			{	
    				rightSockets.AddRange(GetSocketsOfDirection(sockets,  
    				     SocketAttributes.Alignment.RIGHT));
    				
    			}
    			else 												// object has a collision on left 
    			{
    				leftSockets.AddRange(GetSocketsOfDirection(sockets,  
    					 SocketAttributes.Alignment.LEFT));
    			}
    		}
    	}


    	public static SocketBehaviour
    		FindClosestSockets
    			(List<SocketBehaviour> socketsA,
    			 List<SocketBehaviour> socketsB)
    	{
    		SocketBehaviour cSocketA = null;
    		SocketBehaviour cSocketB = null;

    		float closestDistance = float.MaxValue;
    		
    		foreach (SocketBehaviour socketA in socketsA) {
    			foreach (SocketBehaviour socketB in socketsB) {
    				float distance = Vector3.Distance (socketA.position, socketB.position);
    				if (distance < closestDistance) {
    					closestDistance = distance;
    					cSocketA = socketA;
    					cSocketB = socketB;
    				}
    			}
    		}
    		cSocketA.CreatePending(cSocketB);
    		return cSocketA;
    	}

    }
}

