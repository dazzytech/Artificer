using Space.AI.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Space.AI.Agent
{
    /// <summary>
    /// Attaches to a ship that performs a guard role e.g.
    /// following and protecting a ship or station.
    /// </summary>
    public class GuardAgent : FSM
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

            base.FSMLateUpdate();
        }

        #endregion
    }
}
