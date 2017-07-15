using Space.Ship.Components.Attributes;
using Space.Ship.Components.Listener;
using Space.UI.Station.Editor;
using Space.UI.Station.Editor.Component;
using Space.UI.Station.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Space.UI.Station.Prefabs
{
    public class ComponentInteractivePrefab : ComponentViewerPrefab
    {
        #region ATTRIBUTES

        [Header("Interactive Panel")]

        #region HUD ELEMENTS

        /// <summary>
        /// Interactable toggle that determines
        /// if component is head
        /// </summary>
        [SerializeField]
        protected Toggle m_headToggle;

        /// <summary>
        /// Reference to panel that allows us to 
        /// select style
        /// </summary>
        [SerializeField]
        protected Transform m_stylePanel;

        #endregion

        /// <summary>
        /// Used to determine if we are head
        /// </summary>
        [SerializeField]
        protected BaseComponent m_head;

        /// <summary>
        /// Reference to our component
        /// </summary>
        [SerializeField]
        protected BaseComponent m_BC;

        /// <summary>
        /// Store the delegated function parent gives us.
        /// </summary> 
        [SerializeField]
        private ShipEditor.DelegateHead m_callHead;

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Initializes RC menu with component
        /// and displays info, also
        /// assigns head assigning delegate
        /// </summary>
        /// <param name="BC"></param>
        /// <param name="head"></param>
        /// <param name="newcall"></param>
        public void DisplayBC(BaseComponent BC, BaseComponent head,
                                ShipEditor.DelegateHead newcall)
        {
            this.m_BC = BC;
            m_head = head;
            m_callHead = newcall;

            Display(BC.GO);

            ComponentListener Con = BC.GO.GetComponent<ComponentListener>();

            ComponentAttributes att = Con.GetAttributes();

            if (Con.ComponentType == "Components")
                m_headToggle.isOn = BC == head;
            else
                m_headToggle.interactable = false;

            AssignType(Con.ComponentType, att, BC);

            m_stylePanel.GetComponent<StyleController>().Display(att, BC);
        }

        /// <summary>
        /// Updates the head toggle visually
        /// </summary>
        public void AssignHead()
        {
            if (m_headToggle.isOn)
            {
                m_head = m_BC;
                m_callHead(m_BC);
            }
            else if (m_head == m_BC)
            {
                m_headToggle.isOn = true;
            }
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Adds additional panels dependant on
        /// component type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="att"></param>
        private void AssignType(string type, ComponentAttributes att, BaseComponent BC)
        {
            base.AssignType(type, att);

            // Show panel to corresponding type
            // Any panel with interactivity will have their own script
            switch (type)
            {
                case "Weapons":
                    m_weaponPanel.gameObject.SetActive(true);
                    m_weaponPanel.GetComponent<WeaponTriggerSelection>().
                        Display((WeaponAttributes)att, BC);
                    break;
                case "Launchers":
                    m_launcherPanel.gameObject.SetActive(true);
                    m_launcherPanel.GetComponent<LauncherCustomizerSelection>().
                        Display((LauncherAttributes)att, BC);
                    break;
                case "Targeter":
                    m_targeterPanel.gameObject.SetActive(true);
                    m_targeterPanel.GetComponent<TargeterCustomizerSelection>().
                        Display((TargeterAttributes)att, BC);
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}
