using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace Space.Ship
{
    [RequireComponent(typeof(ShipInputReceiver))]
    public class ShipPlayerInputController : NetworkBehaviour
    {
        ShipInputReceiver _controller;
        private bool dragging = false;

        // Use this for initialization
        void Awake()
        {
            _controller = GetComponent<ShipInputReceiver>();
        }

        // Update is called once per frame
        void Update()
        {
            // Attempt to make this a command
            if (Input.anyKey)
                _controller.ReceiveKey(KeyLibrary.FindKeysPressed());

            _controller.ReleaseKey(KeyLibrary.FindKeyReleased());

            // Update mouse cursor
            Vector2 mousePos = Input.mousePosition;

            // for now link to left mouse button
            if (Input.GetMouseButton(0))
            {
                _controller.ReceiveKey(KeyCode.Mouse0);
                if (!dragging)
                {
                    _controller.StartRect
                        (Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    _controller.SingleClick
                        (Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    dragging = true;
                } else {
                    _controller.DragRect
                        (Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }
            }
            // clear all selections on right
            if (Input.GetMouseButton(1))
            {
                _controller.ReceiveKey(KeyCode.Mouse1);
                if (!dragging)
                    _controller.ClearTargets();
                else
                {
                    dragging = false;
                    _controller.CancelRect();
                }
            }
            // confirm on mouse up
            if (Input.GetMouseButtonUp(0))
            {
                if (dragging)
                {
                    dragging = false;
                    _controller.ConfirmRect();
                }
            }
        }
    }
}

