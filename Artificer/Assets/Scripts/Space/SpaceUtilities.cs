using UnityEngine;
using System.Collections;

using Space.UI;

namespace Space
{
    /// <summary>
    /// Misc functions used by space objects
    /// game state control and zooming
    /// </summary>
    public class SpaceUtilities : MonoBehaviour
    {
        #region ATTRIBUTES

        // pause vars
        private bool _isPaused = false;
        private bool _pauseDelay = false;

        #endregion

        #region INITIALIZATION

        /// <summary>
        /// Sets default values
        /// </summary>
        public void Init()
        {
            Time.timeScale = 1f;
            _isPaused = false;
            _pauseDelay = false;
        }

        #endregion

        #region MONOBEHAVIOUR

        // restore time when exiting
        void OnDestroy()
        {
            Time.timeScale = 1f;
        }

        #endregion

        #region ZOOM

        public void ZoomIn()
        {
            GameObject PCGO = GameObject.Find("PlayerCamera");
            if (PCGO == null)
                return;
            Camera cam = PCGO.GetComponent<Camera>();
            if (cam.orthographicSize < 10)
                return; 
            cam.orthographicSize--;
        }
        
        public void ZoomOut()
        {
            GameObject PCGO = GameObject.Find("PlayerCamera");
            if (PCGO == null)
                return;
            Camera cam = PCGO.GetComponent<Camera>();

            if (cam.orthographicSize > 30)
                return; 
            cam.orthographicSize++;
        }

        #endregion

        #region GAME CONTROLS

        public void Pause(bool keyed)
        {
            if(!_pauseDelay)
            {
                _isPaused = !_isPaused;

                if (_isPaused)
                    SystemManager.UIState.SetState(UIState.Pause);
                else
                    SystemManager.UIState.RevertState();

                if(keyed)
                {
                    // stop the ability to pause until key is released
                    _pauseDelay = true;
                }
            }
        }

        public void Map(bool keyed)
        {

        }

        public void Stop()
        {
            SystemManager.UIState.SetState(UIState.Popup);
        }

        public void PauseRelease()
        {
            _pauseDelay = false;
        }

        public void MapRelease()
        {

        }

        #endregion
    }
}

