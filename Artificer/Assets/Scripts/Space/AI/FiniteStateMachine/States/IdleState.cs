using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Space.AI.State
{
    public class IdleState : FSMState
    {
        public float waitTime;
        private float m_curTime;

        public IdleState()
        {
            //waitTime = waitFor; NEEDS ASSIGNING
            m_stateID = FSMStateID.Static;
            m_curTime = 0.0f;
            Keys = new List<KeyCode>();
        }

        /// <summary>
        /// Wait for the alotted time
        /// the revert to previous state
        /// </summary>
        public override void Reason()
        {
            /* If this is an attack ship then check for enemies in range
            foreach (Transform e in enemies)
            {
                // check the distance with player tank
                if (Vector3.Distance(npc.position, e.position)
                    <= eng)
                {
                    npc.SendMessage("SetTransition", Transition.SawEnemy);
                    return;
                }
            }*/

            /*m_curTime += Time.deltaTime;
            if (m_curTime >= waitTime)
            {
                Self.SetTransition(Transition.Resume);
                m_curTime = 0.0f;
            }*/

            if(Self.Target != null)
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
            Con.ReleaseKey(Control_Config.GetKey("turnRight", "ship"));
            Con.ReleaseKey(Control_Config.GetKey("turnLeft", "ship"));
        }
    }
}
