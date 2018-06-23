using Generator;
using Space.AI;
using Space.Ship;
using Space.Ship.Components.Listener;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.UI.IDE
{
    public class IDEAttributes : MonoBehaviour
    {
        #region SUB WINDOWS

        public EditorManager Editor;

        #endregion

        /// <summary>
        /// Reference to the ship that allows the IDE to access
        /// the Controller 
        /// </summary>
        public ShipAccessor Ship;

        /// <summary>
        /// When the player script is generated
        /// then it is stored here.
        /// </summary>
        public ICustomScript PlayerScript;

        /// <summary>
        /// The utility that will generate the user script
        /// </summary>
        public ScriptGenerator Generator;
    }
}