using Space.UI.Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.UI
{
    /// <summary>
    /// Responsible for handling input responses from
    /// the player
    /// </summary>
    public class UIInputController : MonoBehaviour
    {
        #region ATTRIBUTES

        private bool m_keyDelay = false;

        [SerializeField]
        private PlayHUD m_play;

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Sets the HUD visible or not if on playHUD
        /// </summary>
        public void ToggleHUD()
        {
            if (!m_keyDelay && m_play.gameObject.activeSelf)
            {
                m_play.gameObject.SetActive(!m_play.gameObject.activeSelf);
                m_keyDelay = true;
                Invoke("PauseRelease", 0.3f);
            }
        }

        /// <summary>
        /// The player can set the mission HUD visible and invisible alone
        /// due to its size
        /// </summary>
        public void ToggleHUD(string element)
        {
            if (!m_keyDelay && m_play.gameObject.activeSelf)
            {
                m_play.HidePanel(element);
                m_keyDelay = true;
                Invoke("PauseRelease", 0.3f);
            }
        }

        public void PauseRelease()
        {
            m_keyDelay = false;
        }

        #endregion
    }
}
