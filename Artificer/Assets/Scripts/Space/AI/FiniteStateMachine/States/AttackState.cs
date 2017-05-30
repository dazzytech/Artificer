using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Space.AI.State
{
    /// <summary>
    /// 
    /// </summary>
    public class AttackState : FSMState
    {
        private Vector3 _dest;

        [SerializeField]
        private float _angleAccuracy;

        public AttackState()
        {
            m_stateID = FSMStateID.Attacking;
            Keys = new List<KeyCode>();
        }

        public override void Reason(List<Transform> enemies, Transform npc)
        {
            // Test for emergency eject
            if (ShipStatus.EvacNeeded(npc))
            {
                npc.SendMessage("SetTransition", Transition.Eject);
                return;
            }

            // Check that we have enemies to fight
            if (enemies.Count <= 0 || enemies[0] == null)
            {
                npc.SendMessage("SetTransition", Transition.LostEnemy);
                return;
            }

            Transform target = DestUtil.FindClosestEnemy(enemies, npc.position);
            _dest = target.position;
            float dist = Vector3.Distance(npc.position, _dest);

            //if(CombUtil.EnemyIsVisible(_dest, dist, npc, target) != null)
            //npc.SendMessage("SetTransition", Transition.GoAround);

            if (dist >= 100.0f && dist < 200.0f)
                npc.SendMessage("SetTransition", Transition.SawEnemy);
            else if (dist > 200.0f)
                npc.SendMessage("SetTransition", Transition.LostEnemy);
            else if (dist < 30f)
                npc.SendMessage("SetTransition", Transition.PullOff);
        }

        public override void Act(List<Transform> enemies, Transform npc)
        {
            Keys.Clear();

            Con.ReleaseKey(Control_Config.GetKey("fire", "ship"));
            Con.ReleaseKey(Control_Config.GetKey("moveDown", "ship"));

            float angleDiff = DestUtil.FindAngleDifference(npc, _dest);

            Keys.Add(Control_Config.GetKey("moveUp", "ship"));

            if (angleDiff >= _angleAccuracy)
            {
                Con.ReleaseKey(Control_Config.GetKey("turnRight", "ship"));
                Keys.Add(Control_Config.GetKey("turnLeft", "ship"));
            }
            else if (angleDiff <= -_angleAccuracy)
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
