using Space.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.UI.Map
{
    /// <summary>
    /// 
    /// </summary>
    public class MapController : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private MapViewer m_map;

        #endregion

        #region MONO BEHAVIOUR

        private void Awake()
        {
            m_map.InitializeMap();
        }

        #endregion
    }
}
