using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Space.UI.Station.Editor.Socket;
using UnityEngine.UI;

namespace Space.UI.Station.Editor.Component
{
    public class LayerReorderingBehaviour : MonoBehaviour
    {

        RawImage _renderer;
        List<SocketBehaviour> _sockets;

        Canvas self;

        // Use this for initialization
        void Start()
        {
            //_renderer = GetComponent<SpriteRenderer>();
            //_sockets = GetBackSockets(transform.parent.transform);
        }

        private List<SocketBehaviour>
            GetBackSockets(Transform parentTransform)
        {
            List<SocketBehaviour> newList = new List<SocketBehaviour>();

            foreach (Transform socketTransform in parentTransform)
            {
                if (socketTransform.tag == "Socket")
                {
                    SocketBehaviour socket =
                        socketTransform.GetComponent<SocketBehaviour>();
                    if (socket.alignment == SocketAttributes.Alignment.DOWN)
                        newList.Add(socket);
                }
            }
            return newList;
        }

        // Update is called once per frame
        void Update()
        {
            if (ShipEditor.DraggedObj != null)
            {
                if (ShipEditor.DraggedObj.transform == transform)
                {
                    if (self == null)
                        self = gameObject.AddComponent<Canvas>();

                    self.overrideSorting = true;
                    self.sortingOrder = 1;
                }
                else
                {
                    Destroy(self);
                    self = null;
                }
            }
            else
            {
                Destroy(self);
                self = null;
            }
            /*else
                foreach (SocketBehaviour socket in _sockets)
                {
                    if (socket.state == SocketAttributes.SocketState.CLOSED)
                    {
                        SpriteRenderer otherRenderer =
                            socket.connectedSocket.
                                container.GetComponentInChildren
                                <SpriteRenderer>();

                        if (otherRenderer.sortingLayerName ==
                           _renderer.sortingLayerName)
                            _renderer.sortingOrder =
                                otherRenderer.sortingOrder - 1;
                        else
                            _renderer.sortingOrder = 0;
                    }
                    else
                    {
                        _renderer.sortingOrder = 0;
                    }
                }*/
        }
    }

}