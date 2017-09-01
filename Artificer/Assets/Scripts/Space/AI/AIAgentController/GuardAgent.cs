using Space.AI.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Space.Ship;

namespace Space.AI.Agent
{
    /// <summary>
    /// Attaches to a ship that performs a guard role e.g.
    /// following and protecting a ship or station.
    /// </summary>
    public class GuardAgent : FSM
    {
        #region PRIVATE UTILITIES

        #region FSM 

        protected override void Initialize()
        {
            base.Initialize();

            // Add all possible states the agent will
            // perform
            AddFSMState(new IdleState(FSMStateID.Defensive));
            AddFSMState(new TravelState());
            AddFSMState(new AttackState());
            AddFSMState(new EvadeState());
            AddFSMState(new PursueState());
            AddFSMState(new StrafeState());
            AddFSMState(new GuardState());
            AddFSMState(new EjectState());
        }

        protected override void FSMUpdate()
        {
            if (CurrentState == null)
                return;

            base.FSMUpdate();
        }

        protected override void FSMLateUpdate()
        {
            base.FSMLateUpdate();

            // Ensure we have a target
            if (m_target == null)
            {
                // * .75 cause we dont want to pick a target we are about to break off
                base.GetClosestTarget(m_engageDistance * .95f, Home.position);
            }
            else if (Vector3.Distance(Home.position, m_target.position) > m_engageDistance)
            {
                // Break off if we're too far
                m_target = null;
                base.GetClosestTarget(m_engageDistance * .75f, Home.position);
            }
        }

        #endregion

        protected override void SeekTargets()
        {
            // target any ships if the ship
            // if reference on team KOS list
            for (int i = 0; i < m_ships.Count; i++)
            {
                ShipAccessor other = m_ships[i];
                if (other == null)
                    continue;

                if (Team.KOS.Contains(other.NetID.Value)
                    || Team.EnemyTeam.Contains(other.TeamID))
                {
                    if (!m_targets.Contains(other.transform))
                        m_targets.Add(other.transform);
                }
            }

            for (int i = 0; i < m_targets.Count; i++)
            {
                if (m_targets[i] == null)
                {
                    m_targets.RemoveAt(i);
                    i--;
                }
            }
        }

        #endregion
    }
}
