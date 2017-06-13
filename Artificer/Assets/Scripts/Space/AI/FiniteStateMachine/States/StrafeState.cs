using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Space.AI.State
{
    /// <summary>
    /// If there is no line of sight between 
    /// the target then continue to strafe out of the way
    /// </summary>
    public class StrafeState
        : FSMState
    {
        /// <summary>
        /// The direction that the ship will strage e.g. left or right
        /// in future consider forward and backwards
        /// </summary>
        private Vector3 m_stafeDirection;

        public StrafeState()
        {
            m_stateID = FSMStateID.Strafing;
            Keys = new List<KeyCode>();
        }

        public override void Reason()
        {
            // Find if we have a block or if we can resume
            Transform block = CombUtil.ObjectIsVisible(Self.transform, Self.Target);
            if (block == null)
            {
                Self.SetTransition(Transition.Resume);
                return;
            }

            // Calculate direction of the block
            // and convert to our local rotation
            m_stafeDirection = Self.transform.InverseTransformDirection
                ((Self.transform.position - block.position).normalized);
        }

        public override void Act()
        {
            // Reset key press for next step
            Keys.Clear();

            Con.ReleaseKey(Control_Config.GetKey("moveUp", "ship"));
            Con.ReleaseKey(Control_Config.GetKey("moveDown", "ship"));


            // Strafe right is the x axis is < 0
            // TODO TEST AND FIX LOGIC
            if (m_stafeDirection.x < 0)
            { 
                Con.ReleaseKey(Control_Config.GetKey("strafeLeft", "ship"));
                Keys.Add(Control_Config.GetKey("strafeRight", "ship"));
            }
            else
            {
                Con.ReleaseKey(Control_Config.GetKey("strafeRight", "ship"));
                Keys.Add(Control_Config.GetKey("strafeLeft", "ship"));
            }
        }
    }

}