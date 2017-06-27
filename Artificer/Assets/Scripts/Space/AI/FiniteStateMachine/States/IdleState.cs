using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Space.AI.State
{
    public class IdleState : FSMState
    {
        public IdleState()
        {
            m_stateID = FSMStateID.Static;
            Keys = new List<KeyCode>();
        }

        /// <summary>
        /// Wait for the alotted time
        /// the revert to previous state
        /// </summary>
        public override void Reason()
        {
            // If we are not waiting for a timer
            // then we are waiting for a target to appear
            if(!Timed && Self.Target != null)
            {
                Self.SetTransition(Transition.Resume);
                return;
            }

            base.Reason();
        }

        public override void Act()
        {
            Keys.Clear();

            Con.ReleaseKey(Control_Config.GetKey("moveUp", "ship"));
            Con.ReleaseKey(Control_Config.GetKey("moveDown", "ship"));


            if (Self.Target == null)
            {
                Con.ReleaseKey(Control_Config.GetKey("turnRight", "ship"));
                Con.ReleaseKey(Control_Config.GetKey("turnLeft", "ship"));
                return;
            }

            float angleDiff = DestUtil.FindAngleDifference(Self.transform, Self.Target.position);

            // Changed so that the doesnt move towards target 
            // change when applying types

            if (angleDiff >= m_angleAccuracy)
            {
                Con.ReleaseKey(Control_Config.GetKey("turnRight", "ship"));
                Keys.Add(Control_Config.GetKey("turnLeft", "ship"));
            }
            else if (angleDiff <= -m_angleAccuracy)
            {
                Con.ReleaseKey(Control_Config.GetKey("turnLeft", "ship"));
                Keys.Add(Control_Config.GetKey("turnRight", "ship"));
            }
        }
    }
}
