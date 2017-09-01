using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Space.AI.State
{
    public class TravelState : FSMState
    {
        #region ATTRIBUTES

        /// <summary>
        /// How close agent gets in order to reach waypoint
        /// </summary>
        private float m_reachDistance = 10f;
        /// <summary>
        /// Distance the agent way point reaches maximum
        /// </summary>
        private float m_travelDistance = 50f;
        /// <summary>
        /// the waypoint position the agent travels to
        /// </summary>
        private Vector2 m_waypoint = Vector2.zero;

        #endregion

        public TravelState()
        {
            m_stateID = FSMStateID.Travelling;
            Keys = new List<KeyCode>();
        }

        #region FSM STATE

        public override void Reason()
        {
            // Check that we have enemies to fight
            if (Self.Target == null)
            {
                Self.SetTransition(Transition.Resume);
                return;
            }
            if(m_waypoint == Vector2.zero)
            {
                m_waypoint = Math.RandomWithinRange(Self.Target.position, 5f, m_travelDistance);
            }

            // Check that the ship is close enough to the target
            float dist = Vector3.Distance
                (Self.transform.position, m_waypoint);

            if (dist < m_reachDistance)
            {
                // Return temp travel target
                Self.Target = null;
                m_waypoint = Vector2.zero;

                // resume previous action
                Self.SetTransition(Transition.Resume);
                return;
            }

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

            if (Self.Target == null)
                return;

            Keys.Add(Control_Config.GetKey("moveUp", "ship"));

            AimAtPoint(m_angleAccuracy, m_waypoint);
        }

        #endregion
    }
}
