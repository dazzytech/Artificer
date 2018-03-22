using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using UI;
using Space.Ship;
using Space.Map;

namespace Space.UI.Proxmity
{
    public class MapHUD : HUDPanel
    {
        #region ATTRIBUTES

        /// <summary>
        /// The minimap object attached to this HUD
        /// </summary>
        [SerializeField]
        private MapViewer m_viewer;

        #endregion

        protected override void OnEnable()
        {
            if (SystemManager.Space.PlayerCamera != null)
                Initialize();
        }

        #region PRIVATE UTILITIES

        #region EVENT LISTENER

        /// <summary>
        /// Begins the process of following the ship position on map
        /// visual 
        /// </summary>
        private void Initialize()
        {
            // Begin tracking process if active
            if (isActiveAndEnabled)
               StartCoroutine("Step");

            m_viewer.InitializeMap();
        }

        #endregion

        #endregion

        #region COROUTINE

        private IEnumerator Step()
        {
            while (true)
            {
                if (SystemManager.Space.PlayerCamera == null)
                    yield break;

                m_viewer.CenterAt(SystemManager.Space.PlayerCamera.position);

                yield return new WaitForSeconds(.5f);
            }
            
        }

        #endregion
    }
}