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
        }

        protected override void OnDisable()
        {
            m_viewer.gameObject.SetActive(false);
        }

        protected override void Awake()
        {
            base.Awake();

            m_viewer.InitializeMap();

            if (SystemManager.Space.TeamID == 1)
                m_viewer.RotateMap(new Vector2(0, -1));
        }

        #endregion

        #region COROUTINE

        

        #endregion
    }
}