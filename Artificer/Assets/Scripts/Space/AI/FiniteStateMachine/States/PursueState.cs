using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Space.AI.State
{
    public class PursueState : FSMState
    {
        public PursueState()
        {
            m_stateID = FSMStateID.Pursue;
            Keys = new List<KeyCode>();
        }

        public override void Reason()
        {
            // Check that we have enemies to fight
            if (Self.Target == null)
            {
                Self.SetTransition(Transition.LostEnemy);
                return;
            }

            // Check that the ship is close enough to the target

            float dist = Vector3.Distance
                (Self.transform.position, Self.Target.position);

            if (dist < Self.AttackRange)
                Self.SetTransition(Transition.ReachEnemy);

            // TODO add travel equivilent to strafe

            base.Reason();
            
            /* TODO go around similar to 
            if (DestUtil.FrontIsClear(npc, 3f, 90f) != null)
            {
                npc.SendMessage("SetTransition", Transition.GoAround);
                return;
            }*/
        }

        public override void Act()
        {
            Keys.Clear();

            Keys.Add(Control_Config.GetKey("moveUp", "ship"));

            float angleDiff = DestUtil.FindAngleDifference(Self.transform, Self.Target.position);

            if (angleDiff >= 10f)
            {
                Con.ReleaseKey(Control_Config.GetKey("turnRight", "ship"));
                Keys.Add(Control_Config.GetKey("turnLeft", "ship"));
            }
            else if (angleDiff <= -10f)
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
