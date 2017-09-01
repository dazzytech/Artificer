using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Space.AI.State
{
    /// <summary>
    /// default state for each agent
    /// can be used in two ways:
    /// - a base state, very first state that is
    /// applied, has defined default action that is perform
    /// when target visible
    ///  - Waiting: waits for an alloted period and then resumes
    /// </summary>
    public class IdleState : FSMState
    {
        #region ATTRIBUTES

        /// <summary>
        /// duration the agent will attack before
        /// cooldown
        /// </summary>
        private float m_waitTime = 30f;

        /// <summary>
        /// used to track alloted time
        /// </summary>
        private float m_curTime = 0f;

        #endregion

        public IdleState(FSMStateID defaultAction)
        {
            m_stateID = FSMStateID.Static;

            AddTransition(Transition.Default, defaultAction);
            AddTransition(Transition.Travel, FSMStateID.Travelling);

            Keys = new List<KeyCode>();
        }

        #region PRIVATE UTILITIES

        #region FSM STATE

        /// <summary>
        /// Wait for the alotted time
        /// the revert to previous state
        /// </summary>
        public override void Reason()
        {
            base.Reason();

            // If we are not waiting for a timer
            // then we are waiting for a target to appear
            if (!Timed)
            {
                if (Self.Target != null)
                {
                    Self.SetTransition(Transition.Default);
                    return;
                }
                else
                {
                    // detect if this agent will remain within
                    // range of home
                    if (Self.Home != null && Self.HomeDistance > 0)
                    {
                        if (Vector3.Distance(Self.transform.position,
                            Self.Home.position) > Self.HomeDistance)
                        {
                            GoHome();
                        }
                        else
                        {
                            // increment time and change when ready
                            m_curTime += Time.deltaTime;
                            if (m_curTime >= m_waitTime)
                            {
                                GoHome();
                                m_curTime = 0.0f;
                            }
                        }
                    }
                }
            }
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
            else
                AimAtPoint(m_angleAccuracy);
        }

        #endregion

        private void GoHome()
        {
            Self.Target = Self.Home;
            Self.SetTransition(Transition.Travel);
        }

        #endregion
    }
}
