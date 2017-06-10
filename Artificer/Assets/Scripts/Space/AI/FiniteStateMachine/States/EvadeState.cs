using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Space.AI.Agent
{
    public class EvadeState : FSMState
    {
        public EvadeState()
        {
            m_stateID = FSMStateID.Shifting;
            Keys = new List<KeyCode>();
        }

        public override void Reason()
        {
            // Run untill we are far away objects
            Self.Target = DestUtil.ObjectWithinProximity(Self.transform, Self.PullOffDistance);
            if (Self.Target == null)
            {
                Self.SetTransition(Transition.Resume);
            }
        }

        public override void Act()
        {
            Keys.Clear();

            if (Self.Target != null)
            {
                float angleDiff = DestUtil.FindAngleDifference(Self.transform, Self.Target.position);

                if (Mathf.Abs(angleDiff) <= 90)
                {
                    Con.ReleaseKey(Control_Config.GetKey("moveUp", "ship"));
                    Keys.Add(Control_Config.GetKey("moveDown", "ship"));
                }
                else
                {
                    Con.ReleaseKey(Control_Config.GetKey("moveDown", "ship"));
                    Keys.Add(Control_Config.GetKey("moveUp", "ship"));
                }

                if (angleDiff <= 0f)
                {
                    Con.ReleaseKey(Control_Config.GetKey("turnRight", "ship"));
                    Keys.Add(Control_Config.GetKey("turnLeft", "ship"));
                }
                else if (angleDiff > 0f)
                {
                    Con.ReleaseKey(Control_Config.GetKey("turnLeft", "ship"));
                    Keys.Add(Control_Config.GetKey("turnRight", "ship"));
                }
                else
                {
                    Con.ReleaseKey(Control_Config.GetKey("turnRight", "ship"));
                    Con.ReleaseKey(Control_Config.GetKey("turnLeft", "ship"));
                }
            }
        }
    }
}

