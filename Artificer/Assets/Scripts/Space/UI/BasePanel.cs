using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Space.UI
{
    /// <summary>
    /// Base class for each UI
    /// Panel. 
    /// - Store refernce to self HUD
    /// - Contain two functions
    ///     Activate: loops through each element and makes them interactive
    ///     Disable: loops through each element and makes them non-interactive
    /// </summary>
    public class BasePanel : MonoBehaviour
    {
        #region ATTRIBUTES

        private bool m_keyDelay = false;

        [Header("Base Panel")]

        [SerializeField]
        protected Transform m_HUD;

        [SerializeField]
        private List<GameObject> m_toggleActive;

        #endregion

        #region PUBLIC INTERACTION

        public void ToggleHUD()
        {
            if (!m_keyDelay)
            {
                m_HUD.gameObject.SetActive
                    (!m_HUD.gameObject.activeSelf);
                m_keyDelay = true;
                Invoke("PauseRelease", 0.3f);
            }
        }

        public void ActivateHUD()
        {
            foreach(GameObject GO in m_toggleActive)
            {
                GO.SetActive(true);
            }
        }

        public void DeactivateHUD()
        {
            foreach (GameObject GO in m_toggleActive)
            {
                GO.SetActive(false);
            }
        }

        #endregion

        #region PRIVATE UTILITIES

        private void PauseRelease()
        {
            m_keyDelay = false;
        }

        #endregion

    }
}
