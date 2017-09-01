using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.AI.State
{
    /// <summary>
    /// positions the agent between
    /// target and home, attacks when within
    /// range.
    /// </summary>
    public class GuardState : FSMState
    {
        public GuardState()
        {
            m_stateID = FSMStateID.Defensive;
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
            AddTransition(Transition.Pursuit, FSMStateID.Pursuing);
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

            // Check if we are out of attack range
            if (dist < Self.PersuitRange)
                Self.SetTransition(Transition.Pursuit);

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

            AimAtPoint(m_angleAccuracy);
        }

        #endregion

        #region PRIVATE UTILITIES

        #endregion
    }
}