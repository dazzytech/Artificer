using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Space.UI.IDE
{
    /// <summary>
    /// Stores lists and categories of nodes that may be 
    /// added to script
    /// </summary>
    public class PrefabNodeList : MonoBehaviour
    {
        #region ATTIBUTES

        private EditorManager m_con;

        [Header("UI Prefabs")]

        [SerializeField]
        private GameObject m_nodeCreatePrefab;

        [SerializeField]
        private GameObject m_tabPrefab;

        [Header("UI Elements")]

        [SerializeField]
        private Transform m_tabList;

        [SerializeField]
        private Transform m_prefabList;

        #endregion

        #region PUBLIC INTERACTION
        
        /// <summary>
        /// Generates the list of prefabs
        /// </summary>
        /// <param name="nodeCreate"></param>
        public void GeneratePrefabs()
        {

        }

        #endregion
    }
}
