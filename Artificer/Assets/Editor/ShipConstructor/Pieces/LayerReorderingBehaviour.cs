using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
public class LayerReorderingBehaviour : MonoBehaviour
{
	SpriteRenderer _renderer;
	List<SocketBehaviour> _sockets;

	// Use this for initialization
	void Start ()
	{
		_renderer = GetComponent<SpriteRenderer> ();
		_sockets = GetBackSockets (transform.parent.transform);
	}

	private List<SocketBehaviour>
		GetBackSockets(Transform parentTransform)
	{
		List<SocketBehaviour> newList = new List<SocketBehaviour> ();
		
		foreach (Transform socketTransform in parentTransform) {
			if(socketTransform.tag == "Socket")
			{
			SocketBehaviour socket = 
				socketTransform.GetComponent<SocketBehaviour>();
			if (socket.alignment == SocketAttributes.Alignment.DOWN)
				newList.Add (socket);
			}
		}
		return newList;
	}
	
	// Update is called once per frame
	void Update ()
	{
		foreach (SocketBehaviour socket in _sockets) {
			if(socket.state == SocketAttributes.SocketState.CLOSED)
			{
				SpriteRenderer otherRenderer =
					socket.connectedSocket.
						container.GetComponentInChildren
						<SpriteRenderer>();

				if(otherRenderer.sortingLayerName ==
				   _renderer.sortingLayerName)
					_renderer.sortingOrder =
						otherRenderer.sortingOrder -1;
				else
					_renderer.sortingOrder = 0;
			} else {
				_renderer.sortingOrder = 0;
			}
		}
	}
}*/

