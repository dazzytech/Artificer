using Data.Space.Library;
using Data.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.UI.IDE
{
    public class EditorAttributes : MonoBehaviour
    {
        public NodeLibrary NodePrefabs;

        
        #region UI REFERENCES

        public PrefabNodeList PrefabListHUD;

        public ScriptUI ScriptHUD;

        #endregion

        #region SCRIPT INTERACTION

        /// <summary>
        /// Whether or not the script has been changed
        /// </summary>
        public bool Changed;

        /// <summary>
        /// Keeps a reference to the node instance IDs added 
        /// to the script
        /// </summary>
        public List<int> AddedIDs = new List<int>();

        /// <summary>
        /// Nodes that are added to the script by the player
        /// </summary>
        public Dictionary<int, NodePrefab> AddedNodes = 
            new Dictionary<int, NodePrefab>();

        /// <summary>
        /// Preset node that the script generation stems from
        /// </summary>
        public NodePrefab EntryNode;

        #endregion
    }
}
