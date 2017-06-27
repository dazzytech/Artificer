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
        public AttackState()
        {
            m_stateID = FSMStateID.Attacking;
            Keys = new List<KeyCode>();
        }

        #region FSM STATE

        /// <summary>
        /// Assign transitions unique to attack
        /// and any variables
        /// </summary>
        /// <param name="selfRef"></param>
        public override void Initialize(FSM selfRef)
        {
            base.Initialize(selfRef);

            // Assgin all the transitions this state can perform
            AddTransition(Transition.Wait, FSMStateID.Static);
            AddTransition(Transition.LostEnemy, FSMStateID.Static);
            AddTransition(Transition.Strafe, FSMStateID.Strafing);
            AddTransition(Transition.ChaseEnemy, FSMStateID.Pursuing);
        }

        /// <summary>
        /// Checks that we haven't 
        /// lost the enemy or gone out of range
        /// </summary>
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

            // Check we currently have a line of site with the object 
            // If not then strafe around the blocking obj
            if(CombUtil.ObjectIsVisible(Self.transform, Self.Target) != null)
            {
                Self.SetTransition(Transition.Strafe);
                return;
            }

            // Check if we are out of attack range
            if (dist > Self.AttackRange)
                Self.SetTransition(Transition.ChaseEnemy);
            
            // TODO INCORPERATE SHIP TYPES

            base.Reason();
        }

        /// <summary>
        /// Attempt to fire on the target
        /// or face the target
        /// </summary>
        /// <param name="enemies"></param>
        /// <param name="npc"></param>
        public override void Act()
        {
            Keys.Clear();

            Con.ReleaseKey(Control_Config.GetKey("fire", "ship"));
            Con.ReleaseKey(Control_Config.GetKey("moveDown", "ship"));
            Con.ReleaseKey(Control_Config.GetKey("moveUp", "ship"));

            if (Self.Target == null)
                return;

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
            else
            {
                Con.ReleaseKey(Control_Config.GetKey("turnRight", "ship"));
                Con.ReleaseKey(Control_Config.GetKey("turnLeft", "ship"));
                Keys.Add(Control_Config.GetKey("fire", "ship"));
            }
        }

        #endregion
    }
}
