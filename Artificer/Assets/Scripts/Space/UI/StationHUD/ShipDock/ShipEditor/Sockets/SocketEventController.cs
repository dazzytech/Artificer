using UnityEngine;
using System.Collections;

namespace Space.UI.Station.Editor.Socket
{
    public class SocketEventController
    {
    	private SocketAttributes _attributes;

    	public delegate void SocketStateHandler(SocketAttributes.SocketState newState);

    	public event SocketStateHandler OnStateChange;

    	public void Init(SocketAttributes att)
    	{
            _attributes = att;
    	}

    	public void Connect ()
    	{
    		OnStateChange(SocketAttributes.SocketState.CLOSED);
    	}

    	public void Wipe()
    	{
    		OnStateChange(SocketAttributes.SocketState.OPEN);
    	}

    	public void CreatePending(SocketBehaviour other)
    	{
    		_attributes.connectedSocket = other;
    		OnStateChange (SocketAttributes.SocketState.PENDING);
    	}
    }
}

