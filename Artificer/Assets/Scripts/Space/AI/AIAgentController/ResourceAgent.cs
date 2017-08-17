using Space.AI.State;
using Stations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.AI.Agent
{
    /// <summary>
    /// Attaches to mining ships
    /// </summary>
    public class ResourceAgent : FSM
    {
        #region ATTRIBUTES

        // Parent objects for resources
        private List<Transform> m_containers;

        // Bool the detects if we are close enough to a container
        private bool m_withinRange;

        // if true then we will need to deposit the goods
        private bool m_reachedCapacity;

        #endregion

        #region FSM 

        protected override void Initialize()
        {
            base.Initialize();

            FindContainers();

            m_withinRange = false;
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

            // Detect if we have a target
            if (m_targets.Count > 0) CheckTarget();

            if (m_targets.Count == 0) NewTarget();
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Retreive resource containers from the scene
        /// </summary>
        private void FindContainers()
        {
            // Initialize the list
            m_containers = new List<Transform>();

            // Retreive the transform of each container
            foreach (Transform container in GameObject.Find("_asteroids").transform)
                m_containers.Add(container);
        }

        /// <summary>
        /// Searches station list for stations the agent
        /// can deposit at
        /// </summary>
        /// <returns></returns>
        private List<Transform> RetriveDepositStations()
        {
            // Create a new list of transforms
            List<Transform> returnValue = new List<Transform>();

            // find each station of the correct type and team
            foreach(StationAccessor station in m_stations)
            {
                if (station.Team.ID == SystemManager.Space.TeamID &&
                    station.Type == STATIONTYPE.HOME)
                    returnValue.Add(station.transform);
            }

            return returnValue;
        }

        /// <summary>
        /// If we have a target check that if we reached
        /// target or reached capacity
        /// </summary>
        private void CheckTarget()
        {
            //check if we are close enough if not with range 
            if (!m_withinRange)
            {
                if (Vector3.Distance(transform.position, m_target.position) < m_engageDistance)
                {
                    m_withinRange = true;

                    // Clear current targets for refresh
                    m_targets.Clear();
                }
            }

            // check we are at capacity
        }

        /// <summary>
        /// Finds a new target depending if we want
        /// rock, contianer or home base
        /// </summary>
        private void NewTarget()
        {
            if (m_reachedCapacity)
            {
                // return the material we have to the nearest station
                m_targets = RetriveDepositStations();

                base.GetClosestTarget(m_pursuitDistance);

                // May need to change the transition here perhaps?
            }
            // if we are within range then
            else if (m_withinRange)
            {
                // Add the individual rocks to the targets
                foreach (Transform child in m_target)
                    m_targets.Add(child);

                // Use FSM to get our closest target
                base.GetClosestTarget(m_pursuitDistance);

                // We may need to set transition here (might not though)
            }
            else
            {
                // We need to find a resource container
                m_targets.AddRange(m_containers);

                base.GetClosestTarget(m_pursuitDistance);
            }
        }

        #endregion
    }
}