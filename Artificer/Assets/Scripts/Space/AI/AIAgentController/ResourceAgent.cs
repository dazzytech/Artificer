using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.AI.Agent
{
    /// <summary>
    /// Attaches to mining ships
    /// </summary>
    public class ResourceAgent : FSM
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