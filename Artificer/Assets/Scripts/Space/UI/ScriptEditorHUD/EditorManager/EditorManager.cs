using Data.Space.Library;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.UI.IDE
{
    /// <summary>
    /// is the manager for both the editor and the 
    /// Node list and populates them with nodes
    /// Adds and removes nodes from the script viewer and saves and loads
    /// </summary>
    public class EditorManager : MonoBehaviour
    {
        public delegate void Create(GameObject GO);

        #region ATTIBUTES

        [SerializeField]
        private EditorAttributes m_att;

        //[SerializeField]
        //private IDEController m_con;

        #endregion

        #region ACCESSOR

        /// <summary>
        /// Read only access for the available prefab
        /// </summary>
        public NodeLibrary Prefabs
        {
            get
            {
                return m_att.NodePrefabs;
            }
        }

        #endregion


        #region PUBLIC INTERACTION

        /// <summary>
        /// Load attributes and output to script and list
        /// </summary>
        public void Initialize(NodeLibrary nodePrefabs)
        {
            m_att.NodePrefabs = nodePrefabs;

            m_att.PrefabListHUD.GeneratePrefabs();
        }

        /// <summary>
        /// Clears and draws the new nodes to the script viewer
        /// </summary>
        /// <param name="agent"></param>
        public void LoadAgent(string agent)
        {

        }

        public void DeleteNode(int ID)
        {

        }
        
        /// <summary>
        /// Creates an instance of 
        /// </summary>
        /// <param name="nID"></param>
        public void AddNode(GameObject prefab)
        {

        }

        #endregion

        
    }
}