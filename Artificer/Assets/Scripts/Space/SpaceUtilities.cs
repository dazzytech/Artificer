using UnityEngine;
using System.Collections;

using Space.UI;
using Data.Space;

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

        #endregion

        #region INITIALIZATION

        /// <summary>
        /// Sets default values
        /// </summary>
        public void Initialize()
        {
            m_isPaused = false;
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

        /*public void Map(bool keyed)
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
        }*/

        public void Stop()
        {
            SystemManager.UIState.SetState(UIState.Popup);
        }

        public void PauseRelease()
        {
            m_pauseDelay = false;
        }

        /*public void MapRelease()
        {
            m_mapDelay = false;
        }*/

        #endregion

        #region PLAYER MANAGEMENT

        public void AddShipSpawn(ShipSpawnData newShip)
        {
            if (SystemManager.PlayerShips == null)
            {
                SystemManager.PlayerShips = new ShipSpawnData[1];
                SystemManager.PlayerShips[0] = newShip;
            }
            else
            {
                ShipSpawnData[] temp = SystemManager.PlayerShips;

                SystemManager.PlayerShips = new ShipSpawnData[temp.Length + 1];

                int i = 0;
                foreach (ShipSpawnData t in temp)
                {
                    SystemManager.PlayerShips[i++] = t;
                }

                SystemManager.PlayerShips[i] = newShip;
            }
        }

        #endregion
    }
}

