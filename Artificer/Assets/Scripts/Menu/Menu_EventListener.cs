using UnityEngine;
using System.Collections;

namespace Menu
{
    [RequireComponent(typeof(Menu_Attributes))]
    [RequireComponent(typeof(Menu_Behaviour))]

    public class Menu_EventListener : MonoBehaviour {

        // attributes
        private Menu_Attributes _attributes;
        // controller
        private Menu_Behaviour _controller;
        
        void Awake()
        {
            // Set attributes
            _attributes = GetComponent<Menu_Attributes> ();
            _controller = GetComponent<Menu_Behaviour> ();
        }
    	
        /// <summary>
        /// Raises the enable event.
        /// </summary>
        void OnEnable()
        {
            _controller.OnStateChanged += OnStateChanged;
        }

        /// <summary>
        /// Raises the disable event.
        /// </summary>
        void OnDisable()
        {
            _controller.OnStateChanged -= OnStateChanged;
        }

        /// <summary>
        /// Applys the newly changed menu state
        /// </summary>
        void OnStateChanged(MenuState newState)
        {
            // Enable if disabled e.g. popup closed
            if (newState != MenuState.Popup)
            {
                if(!_attributes.TabPanel.activeSelf)
                    _attributes.TabPanel.SetActive(true);
                if(!_attributes.BasePanel.activeSelf)
                    _attributes.BasePanel.SetActive(true);
            }

            CloseAllTabs();

            switch (newState)
            {
                case MenuState.None:
                    // set any active windows off
                    break;
                case MenuState.Play:
                    _attributes.PlayTab.SetActive(true);
                    break;
                case MenuState.Video:
                    // set video tab as active
                        _attributes.VideoTab.SetActive(true);
                    break;
                case MenuState.Audio:
                        _attributes.AudioTab.SetActive(true);
                    break;
                case MenuState.Controls:
                        _attributes.ControlTab.SetActive(true);
                    break;
                case MenuState.Credits:
                    _attributes.CreditsTab.SetActive(true);
                    break;
                case MenuState.Servers:
                    _attributes.ServerTab.SetActive(true);
                    break;
                case MenuState.Popup:
                        
                        // Disable tab window if popup else
                        _attributes.TabPanel.SetActive(false);
                    _attributes.BasePanel.SetActive(false);
                        _attributes.PopupWindow.SetActive(true);
                    break;
            }

            _attributes.MenuState = newState;
        }
        
        /// <summary>
        /// Closes all tabs.
        /// in the menu
        /// </summary>
        private void CloseAllTabs()
        {
            if (_attributes.PlayTab.activeSelf)
            {
                _attributes.PlayTab.SetActive(false);
            }
            if (_attributes.VideoTab.activeSelf)
            {
                _attributes.VideoTab.SetActive(false);
            }
            if (_attributes.AudioTab.activeSelf)
            {
                _attributes.AudioTab.SetActive(false);
            }
            if (_attributes.ControlTab.activeSelf)
            {
                _attributes.ControlTab.SetActive(false);
            }
            if (_attributes.CreditsTab.activeSelf)
            {
                _attributes.CreditsTab.SetActive(false);
            }
            if (_attributes.ServerTab.activeSelf)
            {
                _attributes.ServerTab.SetActive(false);
            }
            if (_attributes.PopupWindow.activeSelf)
            {
                _attributes.PopupWindow.SetActive(false);
            }
        }
    }
}