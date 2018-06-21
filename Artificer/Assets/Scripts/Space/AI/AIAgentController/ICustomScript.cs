using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Space.AI;

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

        public List<KeyCode> Keys;

        #endregion

        /// <summary>
        /// Called by custom agent to 
        /// assign default parameters etc
        /// </summary>
        public void InitializeScript()
        {
            Keys = new List<KeyCode>();
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
