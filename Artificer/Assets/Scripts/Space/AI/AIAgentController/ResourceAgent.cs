using Space.AI.State;
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

        

        #endregion

        #region FSM 

        protected override void Initialize()
        {
            base.Initialize();

            FindContainers();
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

            // 
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

        #endregion
    }
}