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
        #region ATTIBUTES

        [SerializeField]
        private EditorAttributes m_att;

        [SerializeField]
        private EditorAssetIO m_IO;

        [SerializeField]
        private IDEController m_con;

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Load attributes and output to script and list
        /// </summary>
        public void Initialize()
        {
            
        }

        /// <summary>
        /// Clears and draws the new nodes to the script viewer
        /// </summary>
        /// <param name="agent"></param>
        public void LoadAgent(string agent)
        {

        }

        /// <summary>
        /// Creates an instance of 
        /// </summary>
        /// <param name="nID"></param>
        public void AddNode(int nID)
        {

        }

        public void DeleteNode(int ID)
        {

        }

        #endregion
    }
}