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
        #region ATTRIBUTES

        /// <summary>
        /// duration the agent will attack before
        /// cooldown
        /// </summary>
        private float m_attackTimer = 3f;

        /// <summary>
        /// used to track alloted time
        /// </summary>
        private float m_curTime = 0f;

        #endregion

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

            AddTransition(Transition.Strafe, FSMStateID.Strafing);
            AddTransition(Transition.Wait, FSMStateID.Static);
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
                Self.SetTransition(Transition.Resume);
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
                Self.SetTransition(Transition.Resume);

            // increment time and cooldown after attacking
            m_curTime += Time.deltaTime;
            if (m_curTime >= m_attackTimer)
            {
                Self.SetTransition(Transition.Wait, 1.0f, Transition.Resume);
                m_curTime = 0.0f;
            }

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

            switch(Self.Control)
            {
                case ControlStyle.DOGFIGHTER:
                case ControlStyle.NONE:
                    if (AimAtPoint(m_angleAccuracy))
                        Keys.Add(Control_Config.GetKey("fire", "ship"));
                    break;
                case ControlStyle.AUTOTARGET:
                    AimAtPoint(90f);
                    break;
            }
        }

        #endregion
    }
}
