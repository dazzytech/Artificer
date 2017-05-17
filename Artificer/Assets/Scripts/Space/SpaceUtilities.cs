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
        private bool m_isPaused = false;
        private bool m_pauseDelay = false;

        // map vars
        private bool m_viewingMap = false;
        private bool m_mapDelay;

        #endregion

        #region INITIALIZATION

        /// <summary>
        /// Sets default values
        /// </summary>
        public void Initialize()
        {
            m_isPaused = false;
            m_pauseDelay = false;

            m_mapDelay = false;
            m_pauseDelay = false;
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
            if(!m_pauseDelay)
            {
                m_isPaused = !m_isPaused;

                if (m_isPaused)
                    SystemManager.UIState.SetState(UIState.Pause);
                else
                    SystemManager.UIState.RevertState();

                if(keyed)
                {
                    // stop the ability to pause until key is released
                    m_pauseDelay = true;
                }
            }
        }

        public void Map(bool keyed)
        {
            if (!m_mapDelay)
            {
                m_viewingMap = !m_viewingMap;

                if (m_viewingMap)
                    SystemManager.UIState.SetState(UIState.Map);
                else
                    SystemManager.UIState.RevertState();

                if (keyed)
                {
                    // stop the ability to pause until key is released
                    m_mapDelay = true;
                }
            }
        }

        public void Stop()
        {
            SystemManager.UIState.SetState(UIState.Popup);
        }

        public void PauseRelease()
        {
            m_pauseDelay = false;
        }

        public void MapRelease()
        {
            m_mapDelay = false;
        }

        #endregion
    }
}

