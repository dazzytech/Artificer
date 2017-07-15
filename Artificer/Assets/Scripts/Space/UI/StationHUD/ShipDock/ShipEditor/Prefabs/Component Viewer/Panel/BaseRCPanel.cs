using Space.Ship.Components.Attributes;
using Space.UI.Station.Editor.Component;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Space.UI.Station.Utility
{
    /// <summary>
    /// Attaches to right click window 
    /// selectors extend this panel
    /// </summary>
    public class BaseRCPanel : MonoBehaviour
    {
        #region ATTRIBUTES

        [Header("Right Click Panel")]

        /// <summary>
        /// UI Panel that the user interacts with
        /// </summary>
        [SerializeField]
        protected Transform m_panel;

        /// <summary>
        /// Group of toggles for selection
        /// </summary>
        [SerializeField]
        protected ToggleGroup m_group;

        /// <summary>
        /// List of toggles for singular selection
        /// order in list deteremines index
        /// </summary>
        [SerializeField]
        protected Toggle[] m_toggles;

        /// <summary>
        /// Index of selected toggle
        /// </summary>
        [SerializeField]
        protected int m_index;

        /// <summary>
        /// component interacted with by this panel
        /// </summary>
        protected BaseComponent m_BC;

        #endregion

        #region PUBLIC INTERACTION

        public virtual void Display(ComponentAttributes att, BaseComponent bC)
        {
            m_BC = bC;

            AssignToggles();
        }

        #endregion

        #region PRIVATE FUNCTIONALITY

        /// <summary>
        /// Assigns toggles to group
        /// and applies delegates to them
        /// </summary>
        private void AssignToggles()
        {
            for(int i = 0; i < m_toggles.Length; i++)
            {
                Toggle toggle = m_toggles[i];

                m_group.RegisterToggle(toggle);

                int index = i;

                toggle.onValueChanged.AddListener
                    (delegate { SetToggle(index, toggle); });

                if (m_index == i)
                    toggle.isOn = true;
                else
                    toggle.isOn = false;
            }
        }

        /// <summary>
        /// Delegate function called when 
        /// toggle is pressed
        /// sets the index then allows changes 
        /// to be made
        /// </summary>
        /// <param name="index"></param>
        /// <param name="toggle"></param>
        private void SetToggle(int index, Toggle toggle)
        {
            if (m_index == index)
            {
                if (!toggle.isOn)
                {
                    toggle.isOn = true;
                    return;
                }
            }

            if (!toggle.isOn)
                return;

            m_index = index;

            foreach (Toggle other in m_toggles)
            {
                if (toggle.Equals(other))
                    continue;

                other.isOn = false;
            }

            ApplyToggle(m_index);
        }

        #region VIRTUAL FUNCTIONS

        /// <summary>
        /// Applies any changes made by the 
        /// toggle selection
        /// </summary>
        /// <param name="index"></param>
        protected virtual void ApplyToggle(int index)
        { }

        #endregion
        
        #endregion
    }
}