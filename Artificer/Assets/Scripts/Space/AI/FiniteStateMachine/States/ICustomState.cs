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
    public abstract class ICustomState : FSMState
    {
        #region ATTRIBUTES



        #endregion

        public ICustomState()
        {
            m_stateID = FSMStateID.Custom;
            Keys = new List<KeyCode>();
        }

        /// <summary>
        /// Called by custom agent to 
        /// assign default parameters etc
        /// </summary>
        public void InitializeScript()
        {

        }

        /// <summary>
        /// Called by the CustomAgent
        /// </summary>
        public abstract void PerformLoop();

        /// <summary>
        /// Called by custom agent when the ship
        /// takes damage
        /// </summary>
        public abstract void ComponentDamaged();

        #region FSM STATE

        public override void Reason()
        {
            // Do nothing here
        }

        public override void Act()
        {
            // Do nothing here
        }

        #endregion
    }
}
