using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Space.AI.State
{
    /// <summary>
    /// When within attack range of target
    /// open fire.
    /// Used by attack and guard agents
    /// </summary>
    public class AttackState : FSMState
    {
        private Vector3 m_attackPoint;

        // Defined in editor
        // how long the angle
        [SerializeField]
        private float m_angleAccuracy;

        public AttackState()
        {
            m_stateID = FSMStateID.Attacking;
            Keys = new List<KeyCode>();
        }

        public override void Reason(List<Transform> targets, Transform npc)
        {
            // Test for emergency eject
            if (ShipStatus.EvacNeeded(npc))
            {
                npc.SendMessage("SetTransition", Transition.Eject);
                return;
            }

            // Check that we have enemies to fight
            if (targets.Count <= 0 || targets[0] == null)
            {
                npc.SendMessage("SetTransition", Transition.LostEnemy);
                return;
            }

            Transform target = DestUtil.FindClosestEnemy(targets, npc.position);
            m_attackPoint = target.position;
            float dist = Vector3.Distance(npc.position, m_attackPoint);

            //if(CombUtil.EnemyIsVisible(_dest, dist, npc, target) != null)
            //npc.SendMessage("SetTransition", Transition.GoAround);

            if (dist >= 100.0f && dist < 200.0f)
                npc.SendMessage("SetTransition", Transition.SawEnemy);
            else if (dist > 200.0f)
                npc.SendMessage("SetTransition", Transition.LostEnemy);
            else if (dist < 30f)
                npc.SendMessage("SetTransition", Transition.PullOff);
        }

        /// <summary>
        /// Attempt to fire on the target
        /// </summary>
        /// <param name="enemies"></param>
        /// <param name="npc"></param>
        public override void Act(List<Transform> targets, Transform npc)
        {
            Keys.Clear();

            Con.ReleaseKey(Control_Config.GetKey("fire", "ship"));
            Con.ReleaseKey(Control_Config.GetKey("moveDown", "ship"));

            float angleDiff = DestUtil.FindAngleDifference(npc, m_attackPoint);

            Keys.Add(Control_Config.GetKey("moveUp", "ship"));

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
            else
            {
                Con.ReleaseKey(Control_Config.GetKey("turnRight", "ship"));
                Con.ReleaseKey(Control_Config.GetKey("turnLeft", "ship"));
                Keys.Add(Control_Config.GetKey("fire", "ship"));
            }
        }
    }
}
