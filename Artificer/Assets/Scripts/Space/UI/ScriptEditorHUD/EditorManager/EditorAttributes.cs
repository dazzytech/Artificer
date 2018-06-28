using Data.Space.Library;
using Data.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Space.UI.IDE
{
    public class LinkRender
    {
        public IOPrefab Start;

        public IOPrefab End;

        public Color Colour;

        public LineRenderer Line;
    }

    public class EditorAttributes : MonoBehaviour
    {
        public NodeLibrary NodePrefabs;
        
        #region UI REFERENCES

        public PrefabNodeList PrefabListHUD;

        public ScriptUI ScriptHUD;

        /// <summary>
        /// The UI panel in which
        /// compiler errors are displayed
        /// </summary>
        public Transform DebugHUD;

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

        public List<LinkRender> RenderList = 
            new List<LinkRender>();

        /// <summary>
        /// Preset node that the script generation stems from
        /// </summary>
        public NodePrefab EntryNode;

        /// <summary>
        /// Node that is invokd when an object is reached
        /// </summary>
        public NodePrefab InRange;

        /// <summary>
        /// The prefab object to display errors
        /// </summary>
        public GameObject DebugMsgPrefab;

        #endregion
    }
}
