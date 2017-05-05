using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.Map
{
    /// <summary>
    /// Attached to the UI Transfrom 
    /// that displays the map
    /// </summary>
    public class MapViewer : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private Transform m_baseMap;

        #endregion


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        #region PRIVATE UTILITIES

        /// <summary>
        /// Clears all the icons within the map UI
        /// </summary>
        private void ClearIcons()
        {

        }

        #endregion

        #region COROUTINE

        private IEnumerator BuildIcons()
        {

        }

        #endregion
    }
}
