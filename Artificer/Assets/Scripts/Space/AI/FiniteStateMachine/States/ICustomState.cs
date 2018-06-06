using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.AI.State
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
        /// Called by the CustomAgent
        /// </summary>
        public abstract void PerformLoop();

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
