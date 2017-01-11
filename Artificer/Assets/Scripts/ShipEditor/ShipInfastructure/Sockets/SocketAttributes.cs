using UnityEngine;
using System.Collections;

namespace Construction.ShipEditor
{
    public class SocketAttributes {

    	public enum Alignment {LEFT = 0, RIGHT, UP, DOWN};	

    	public Alignment alignment;								// alignment has to be defined manually

    	public enum SocketState {OPEN = 0,PENDING,CLOSED};		

    	public SocketState state;								// State that the socket is currently in

    	public SocketState previous;							// State that the socket is previously in
    	
    	public SocketBehaviour connectedSocket;					// Socket that's currently connected

    	public bool isMale = false;								// Male socket follows female 

    	public bool compulsory = true;							// whether or not the piece must be connected
    															// for the ship to be valid 
    															// && must be defined manually

        public string socketID;
    }
}
