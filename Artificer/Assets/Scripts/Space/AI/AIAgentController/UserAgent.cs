using Space.Ship.Components.Listener;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.AI.Agent
{
    /// <summary>
    /// Agent behaviour that
    /// hosts the user implemented programming.
    /// </summary>
    public class UserAgent : FSM
    {
        #region ATTRIBUTES

        /// <summary>
        /// An interface for the user made script
        /// </summary>
        private ICustomScript m_userScript;

        /// <summary>
        /// reference to the controller module
        /// </summary>
        private ControlListener m_control;

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Assigns the player script and the control
        /// listener and initialize the agent
        /// </summary>
        /// <param name="userScript"></param>
        /// <param name="listener"></param>
        public void SetNPC(ICustomScript userScript, ControlListener listener)
        {
            m_userScript = userScript;

            m_control = listener;

            m_userScript.InitializeScript();
        }

        #endregion

        #region FSM

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void FSMUpdate()
        {
            // Perform update function
            if (m_control == null)
                return;

            m_userScript.PerformLoop();

            if (m_userScript.KeysPressed.Count > 0)
                Con.ReceiveKey(m_userScript.KeysPressed);
        }

        protected override void FSMLateUpdate()
        {
            foreach(KeyCode key in m_userScript.KeysReleased)
                Con.ReleaseKey(key);
        }

        #endregion
    }
}
