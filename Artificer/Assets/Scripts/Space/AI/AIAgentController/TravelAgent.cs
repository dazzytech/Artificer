using Space.AI.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.AI.Agent
{
    public class TravelAgent : FSM
    {
        #region ATTRIBUTES 

        [SerializeField]
        protected List<FSMState> m_fsmStates = new List<FSMState>()
        {
            new AttackState()
        };

        #endregion

        #region FSM 

        protected override List<FSMState> StateMap()
        {
            return m_fsmStates;
        }

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
