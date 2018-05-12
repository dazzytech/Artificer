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
        
        #region MONO BEHAVIOUR

        protected override void OnEnable()
        {
            m_viewer.gameObject.SetActive(true);
            // Begin tracking process if active
            StartCoroutine("Step");
        }

        protected override void OnDisable()
        {
            m_viewer.gameObject.SetActive(false);
        }

        protected override void Awake()
        {
            base.Awake();

            m_viewer.InitializeMap();
        }

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