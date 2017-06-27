using Space.AI.State;
using Space.Ship;
using Stations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Game;

namespace Space.AI.Agent
{
    /// <summary>
    /// Attaches to a ship that follows and assault role
    /// e.g. raider, team attack ship
    /// </summary>
    public class AssaultAgent : FSM
    {
        #region MONO BEHAVIOUR

        #endregion

        #region OVERRIDE FUNCTIONS

        #region FSM

        protected override void Initialize()
        {
            base.Initialize();

            // Add all possible states the agent will
            // perform
            AddFSMState(new AttackState());
            AddFSMState(new EvadeState());
            AddFSMState(new PursueState());
            AddFSMState(new StrafeState());
            AddFSMState(new IdleState());
            AddFSMState(new EjectState());

            SetTransition(Transition.ChaseEnemy);
        }

        protected override void FSMUpdate()
        {
            if (CurrentState == null)
                return;

            base.FSMUpdate();
        }

        protected override void FSMLateUpdate()
        {
            if (CurrentState == null)
                return;

            base.FSMLateUpdate();

            // Ensure we have a target
            if (m_target == null)
                // * .75 cause we dont want to pick a target we are about to break off
                base.GetClosestTarget(m_pursuitDistance * .75f);
            else if (Vector3.Distance(transform.position, m_target.position) > m_pursuitDistance)
            {
                // Break off if we're too far
                m_target = null;
                base.GetClosestTarget(m_pursuitDistance * .75f);
            }
        }

        #endregion

        /// <summary>
        /// Searchs all possible targets
        /// infinite loop
        /// </summary>
        protected override void SeekTargets()
        {
            for (int i = 0; i < m_ships.Count; i++)
            {
                ShipAttributes ship = m_ships[i];
                if (ship == null)
                    continue;

                if (Att.TeamID != ship.TeamID)// TODO TEAM && !m_team.Contains(ship.transform))
                {
                    if (!m_targets.Contains(ship.transform))
                        m_targets.Add(ship.transform);
                }
            }

            /*for (int i = 0; i < m_stations.Count; i++)
            {
                StationAttributes station = m_stations[i];
                if (station == null)
                    continue;

                if (Att.TeamID != station.Team.ID)
                {
                    m_targets.Add(station.transform);
                }

                yield return null;
            }*/

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

        #region PUBLIC INTERACTION

        public void DefineTarget(Transform target)
        {
            Target = target;
        }

        #endregion

        #region COROUTINE

        #endregion
    }
}
