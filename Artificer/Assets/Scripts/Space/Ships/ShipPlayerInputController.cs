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
        #region EVENTS

        public delegate void PlayerUpdate(bool state);

        public static event PlayerUpdate OnStateChanged;

        #endregion

        #region ATTRIBUTES

        private ShipInputReceiver m_con;

        private ShipAttributes m_att;

        private Texture2D m_cursorCombat;

        private Texture2D m_cursorTurning;

        #endregion

        #region MONOBEHAVIOUR

        private void OnDisable()
        {
            // clear combat cursor
            Cursor.SetCursor(null, new Vector2(16, 16), CursorMode.Auto);
        }

        /// <summary>
        /// Init variables here as 
        /// component is always added programatically
        /// </summary>
        void Awake()
        {
            // Initialize components
            m_con = GetComponent<ShipInputReceiver>();

            m_att = GetComponent<ShipAttributes>();

            // initialize cursor
            m_cursorCombat =
                Resources.Load("Textures/playerCursorCombat")
                    as Texture2D;
            m_cursorTurning =
                Resources.Load("Textures/playerCursorTurning")
                    as Texture2D;
        }
        
        void Update()
        {
            if (Input.anyKey)
            {
                m_con.ReceiveKey(KeyLibrary.FindKeysPressed());

                // Trigger event for external listeners
                if (KeyLibrary.FindKeysPressed().Contains(Control_Config.GetKey("switchstate", "ship")))
                    if(OnStateChanged != null)
                        OnStateChanged(m_att.Ship.CombatActive);
            }

            m_con.ReleaseKey(KeyLibrary.FindKeyReleased());

            // Update mouse cursor
            Vector2 mousePos = Input.mousePosition;

            // for now link to left mouse button
            if (Input.GetMouseButton(0))
            {
                m_con.ReceiveKey(KeyCode.Mouse0);
                m_con.SingleClick
                    (Camera.main.ScreenToWorldPoint(mousePos));
            }
            // clear all selections on right
            if (Input.GetMouseButton(1))
            {
                m_con.ReceiveKey(KeyCode.Mouse1);
                m_con.ClearTargets();
            }
        }

        private void LateUpdate()
        {
            if (m_att.Ship.CombatActive && Time.timeScale != 0f)
            {
                if (m_att.Aligned)
                    // Set Cursor
                    Cursor.SetCursor(m_cursorCombat,
                        new Vector2(8, 8), CursorMode.Auto);
                else
                    // Set Cursor
                    Cursor.SetCursor(m_cursorTurning,
                                        new Vector2(8, 8), CursorMode.Auto);
            }
            else
            {
                // clear combat cursor
                Cursor.SetCursor(null, new Vector2(16, 16), CursorMode.Auto);
            }
        }

        #endregion
    }
}

