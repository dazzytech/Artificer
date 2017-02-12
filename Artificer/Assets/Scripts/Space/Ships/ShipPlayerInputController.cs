using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace Space.Ship
{
    /// <summary>
    /// interface for receiving input from the player to control the ship
    /// </summary>
    [RequireComponent(typeof(ShipInputReceiver))]
    public class ShipPlayerInputController : NetworkBehaviour
    {
        #region ATTRIBUTES

        ShipInputReceiver _controller;
        private bool dragging = false;

        #endregion

        #region MONOBEHAVIOUR
        
        void Awake()
        {
            _controller = GetComponent<ShipInputReceiver>();
        }
        
        void Update()
        {
            if (Input.anyKey)
                _controller.ReceiveKey(KeyLibrary.FindKeysPressed());

            _controller.ReleaseKey(KeyLibrary.FindKeyReleased());

            // Update mouse cursor
            Vector2 mousePos = Input.mousePosition;

            // for now link to left mouse button
            if (Input.GetMouseButton(0))
            {
                _controller.ReceiveKey(KeyCode.Mouse0);
                _controller.SingleClick
                    (Camera.main.ScreenToWorldPoint(mousePos));
            }
            // clear all selections on right
            if (Input.GetMouseButton(1))
            {
                _controller.ReceiveKey(KeyCode.Mouse1);
                _controller.ClearTargets();
            }
        }

        #endregion
    }
}

