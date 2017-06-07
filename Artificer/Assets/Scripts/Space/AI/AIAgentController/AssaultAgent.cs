using Space.AI.State;
using Space.Ship;
using Stations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.AI.Agent
{
    /// <summary>
    /// Attaches to a ship that follows and assault role
    /// e.g. raider, team attack ship
    /// </summary>
    public class AssaultAgent : FSM
    {
        #region FSM

        protected override void Initialize()
        {
            base.Initialize();

            StartCoroutine("SearchTargets");
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

        #region COROUTINE

        /// <summary>
        /// Searchs all possible targets
        /// infinite loop
        /// </summary>
        private IEnumerator SearchTargets()
        {
            // initialize variables
            m_targets = new List<Transform>();

            while(true)
            {
                foreach(ShipAttributes ship in m_ships)
                {
                    if(m_teamID != ship.TeamID && !m_team.Contains(ship.transform))
                    {
                        m_targets.Add(ship.transform);
                    }
                }

                foreach (StationAttributes station in m_stations)
                {
                    if (m_teamID != station.Team.ID)
                    {
                        m_targets.Add(station.transform);
                    }
                }

                yield return null;
            }
        }

        #endregion
    }
}
