using Space.AI.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.AI.Agent
{
    /// <summary>
    /// Attaches to a ship that follows and assault role
    /// e.g. raider, team attack ship
    /// </summary>
    public class AssaultAgent : FSM
    {
        #region FSM

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
            if (CurrentState == null)
                return;
        }

        #endregion
    }
}
