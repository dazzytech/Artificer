using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.AI.State
{
    /// <summary>
    /// Simply destroys the agent
    /// </summary>
    public class EjectState : FSMState
    {
        public EjectState()
        {
            m_stateID = FSMStateID.Ejecting;
            Keys = new List<KeyCode>();
        }

        #region FSM STATE

        public override void Reason()
        { }

        public override void Act()
        {
            Keys.Clear();

            Keys.Add(Control_Config.GetKey("eject", "ship"));
        }

        #endregion
    }
}
