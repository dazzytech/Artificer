using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Space.AI;
using Space.Ship;

namespace Space.AI
{
    /// <summary>
    /// The base class for user created scripts
    /// contains default attributes and event listeners
    /// interacted with by the CustomAgent
    /// </summary>
    public abstract class ICustomScript
    {
        #region ATTRIBUTES

        public List<KeyCode> KeysPressed;

        public List<KeyCode> KeysReleased;

        #endregion

        #region ACCESSOR

        /// <summary>
        /// Access the ship input component
        /// </summary>
        /*protected ShipInputReceiver Con
        {
            get { return m_self.Con; }
        }

        /// <summary>
        /// Access to the agent controller
        /// </summary>
        protected FSM Self
        {
            get { return m_self; }
        }*/

        #endregion

        /// <summary>
        /// Called by custom agent to 
        /// assign default parameters etc
        /// </summary>
        public void InitializeScript()
        {
            KeysPressed = new List<KeyCode>();
            KeysReleased = new List<KeyCode>();
        }

        /// <summary>
        /// Called by the CustomAgent
        /// </summary>
        public abstract void PerformLoop();

        #region SHIP EVENTS

        /// <summary>
        /// Called by custom agent when the ship
        /// takes damage
        /// </summary>
        //public abstract void ComponentDamaged();

        #endregion
    }
}
