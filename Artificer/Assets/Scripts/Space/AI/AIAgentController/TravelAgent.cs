using Space.AI.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.AI.Agent
{
    /// <summary>
    /// An AI agent that travels from
    /// key location to key location picking
    /// targets at random
    /// note:
    /// m_targets must be initialized before the
    /// agent runs
    /// Uses m_station as a points to avoid?
    /// /// </summary>
    public class TravelAgent : FSM
    {
        #region FSM 

        /// <summary>
        /// Select the first target for our agent to travel 
        /// to
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void FSMUpdate()
        {
            if (CurrentState == null)
                return;

            base.FSMUpdate();
        }

        protected override void FSMLateUpdate()
        {
            base.FSMLateUpdate();
        }

        #endregion
    }
}
