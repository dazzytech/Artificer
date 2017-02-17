using UnityEngine;
using System.Collections;
using UI;
using Space.Ship;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace Space.UI.Ship
{
    /// <summary>
    /// Container for tracking targeted components
    /// </summary>
    public class TargetComponentItem
    {
        public Transform Target;
        public GameObject Tracker;
    }

    /// <summary>
    /// Container object for ship highlight 
    /// object for Target HUD
    /// </summary>
    public class TargetShipItem : MonoBehaviour
    {
        #region ATTRIBUTES

        [Header("Reference")]

        [SerializeField]
        private Transform m_self;

        public ComponentBuilderUtility Builder;

        [HideInInspector]
        public ShipSelect Selected;

        [Header("Prefabs")]

        [SerializeField]
        private GameObject m_UIComponentPrefab;

        [SerializeField]
        private GameObject m_targetPrefab;

        [Header("Colours")]

        [SerializeField]
        private Color m_prefabColour;

        // Tracking components
        private List<TargetComponentItem> m_targetedComponents;

        private bool m_active = false;

        #endregion

        #region MONOBEHAVIOUR

        void Update()
        {
            if(m_active)
            {
                // As this is worldUI
                // place directly over ship object
                m_self.position = 
                    Selected.Ship.transform.position;

                m_self.localRotation = 
                    Selected.Ship.transform.localRotation;

                // update components
                UpdateTargets();

                SeekTargets();

            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Initialised the ship target item
        /// - builds ship components HUD
        /// - Triggers tracking function
        /// </summary>
        /// <param name="ship"></param>
        public void BuildShip(ShipSelect ship, Color prefabColour)
        {
            Selected = ship;

            m_prefabColour = prefabColour;

            Builder.BuildShip(Selected.Ship, m_UIComponentPrefab);

            // recolour here
            foreach (ViewerItem item
                in Builder.ViewerItems)
                item.SetColour(m_prefabColour);

            // init variables
            m_targetedComponents = 
                new List<TargetComponentItem>();

            m_active = true;
        }

        /// <summary>
        /// Clears UI components
        /// </summary>
        public void ClearShip()
        {
            Builder.ClearShip();

            m_active = false;

            m_targetedComponents.Clear();
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Loops through each target and repositions them
        /// as well as destroy any targets that are
        /// no longers targeted
        /// </summary>
        private void UpdateTargets()
        {
            // first iterate through each target and ensure
            // it is still targeted
            for(int i = 0; i < m_targetedComponents.Count; i++)
            {
                TargetComponentItem target = m_targetedComponents[i];
                if (!Selected.TargetedComponents.Contains(target.Target)
                    || target.Target == null)
                {
                    // this target is no longer selected
                    DeleteTarget(target);
                    i--;
                    continue;
                }
            }

            // Now we removed void targets
            // update existing target positions
            foreach(TargetComponentItem target
                in m_targetedComponents)
            {
                target.Tracker.transform.position
                    = target.Target.position;

                // for now only positions but more could go here
            }
        }

        private void SeekTargets()
        {
            foreach(Transform pending in Selected.TargetedComponents)
            {
                if (m_targetedComponents.Count > 0)
                {
                    if(m_targetedComponents.
                        FirstOrDefault(o => o.Target == pending) != null)
                        continue;
                    else
                        AddTarget(pending);
                }
                else
                    AddTarget(pending);
            }
        }

        /// <summary>
        /// Deletes target tracker and removes it from memory
        /// </summary>
        /// <param name="target"></param>
        private void DeleteTarget(TargetComponentItem target)
        {
            m_targetedComponents.Remove(target);
            Destroy(target.Tracker);
            target = null;
        }

        /// <summary>
        /// Creates a target tracker 
        /// and places it in memory
        /// </summary>
        /// <param name="target"></param>
        private void AddTarget(Transform target)
        {
            TargetComponentItem item = new
                TargetComponentItem();

            item.Target = target;
            item.Tracker = Instantiate(m_targetPrefab);
            item.Tracker.transform.parent = this.transform;

            m_targetedComponents.Add(item);
        }

        #endregion
    }
}
