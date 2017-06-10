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
        // Defined in editor
        // how long the angle
        [SerializeField]
        private float m_angleAccuracy;

        public AttackState()
        {
            m_stateID = FSMStateID.Attacking;
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
           
            float dist = Vector3.Distance
                (Self.transform.position, Self.Target.position);

            // Incorperate go around method
            //if(CombUtil.EnemyIsVisible(_dest, dist, npc, target) != null)
            //npc.SendMessage("SetTransition", Transition.GoAround);

            // Check if we are out of attack range
            if (dist > Self.AttackRange)
                Self.SetTransition(Transition.ChaseEnemy);
            
            // TODO INCORPERATE SHIP TYPEs

            base.Reason();
        }

        /// <summary>
        /// Attempt to fire on the target
        /// </summary>
        /// <param name="enemies"></param>
        /// <param name="npc"></param>
        public override void Act()
        {
            Keys.Clear();

            Con.ReleaseKey(Control_Config.GetKey("fire", "ship"));
            Con.ReleaseKey(Control_Config.GetKey("moveDown", "ship"));
            Con.ReleaseKey(Control_Config.GetKey("moveUp", "ship"));

            float angleDiff = DestUtil.FindAngleDifference(Self.transform, Self.Target.position);

            // Changed so that the doesnt move towards target 
            // change when applying types
            // Keys.Add(Control_Config.GetKey("moveUp", "ship"));

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
