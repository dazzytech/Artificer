using Space.UI.Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

namespace Space.UI
{
    /// <summary>
    /// Responsible for handling input responses from
    /// the player
    /// </summary>
    public class UIHotkeyListener : MonoBehaviour
    {
        #region ATTRIBUTES

        /// <summary>
        /// List of all windows within the _gui object
        /// </summary>
        private List<HUDPanel> m_windows;

        /// <summary>
        /// Stores the delegate for when player
        /// presses a key to hide window
        /// </summary>
        private Dictionary<KeyCode, HUDPanel> m_collapseWindows;

        /// <summary>
        /// Edit mode allows player to move windows
        /// </summary>
        private bool m_editMode;

        /// <summary>
        /// Determines if all windows are hidden
        /// </summary>
        private bool m_hidden;

        #endregion

        #region MONO BEHAVIOUR

        private void OnEnable()
        {
            if(SystemManager.Space != null)
                SystemManager.Space.OnKeyPress += OnKeyPressed;

            m_hidden = false;

            m_editMode = false;
        }

        private void OnDisable()
        {
            if(SystemManager.Space != null)
            SystemManager.Space.OnKeyPress -= OnKeyPressed;
        }

        /// <summary>
        /// Add all the HUDWindows to memory
        /// </summary>
        private void Start()
        {
            // Retrieve HUDS
            HUDPanel[] windows = GameObject.Find("_gui").
                GetComponentsInChildren<HUDPanel>();

            // store each HUD in list
            m_windows = new List<HUDPanel>(windows);

            // Init collapse list
            m_collapseWindows = new Dictionary<KeyCode, HUDPanel>();

            foreach(HUDPanel window in m_windows)
            {
                if(window.Key != KeyCode.None)
                {
                    // if there is a key then window is intended to be
                    // collapsable
                    m_collapseWindows.Add
                        (window.Key, window);
                }
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Detects player interaction 
        /// and collapses/expands windows
        /// or updates edit mode
        /// </summary>
        /// <param name="key"></param>
        public void OnKeyPressed(KeyCode key)
        {
            if (m_collapseWindows.ContainsKey(key))
                    m_collapseWindows[key].ToggleHUD(m_collapseWindows[key].Hidden);

            // set all toggles 
            if (key == Control_Config.GetKey("hud", "sys"))
            {
                m_hidden = !m_hidden;
                foreach (HUDPanel window in m_collapseWindows.Values)
                    window.ToggleHUD(!m_hidden);
            }
        }

        /// <summary>
        /// Called when player elects to rearrange HUD
        /// </summary>
        /// <param name="mode"></param>
        public void SetEditMode(bool mode)
        {
            foreach (HUDPanel window in m_windows)
                window.EditMode = mode;
        }

        #endregion
    }
}
