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

        private bool _keyDelay = false;

        protected Transform HUD;

        [SerializeField]
        private List<GameObject> ToggleActive;

        #endregion

        #region PUBLIC INTERACTION

        public void ToggleHUD()
        {
            if (!_keyDelay)
            {
                HUD.gameObject.SetActive
                    (!HUD.gameObject.activeSelf);
                _keyDelay = true;
                Invoke("PauseRelease", 0.3f);
            }
        }

        public void ActivateHUD()
        {
            foreach(GameObject GO in ToggleActive)
            {
                GO.SetActive(true);
            }
        }

        public void DeactivateHUD()
        {
            foreach (GameObject GO in ToggleActive)
            {
                GO.SetActive(false);
            }
        }

        #endregion

        #region PRIVATE UTILITIES

        private void PauseRelease()
        {
            _keyDelay = false;
        }

        #endregion

    }
}
