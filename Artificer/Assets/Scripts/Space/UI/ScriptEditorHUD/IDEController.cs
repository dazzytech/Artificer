using Data.UI;
using Space.AI;
using Space.Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.UI.IDE
{
    /// <summary>
    /// Manages the windows for the IDE editor
    /// e.g. Overview script, function, load and assign agent
    /// as well as interacts with the compiler
    /// </summary>
    public class IDEController : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private IDEAttributes m_att;

        [SerializeField]
        private IDEEventListener m_event;

        [SerializeField]
        private IDEAssetIO m_IO;

        #endregion

        /// <summary>
        /// Initilize the IDE object
        /// </summary>
        /// <param name="ship"></param>
        public void Initialize(ShipAccessor ship)
        {
            m_att.Ship = ship;

            m_att.Editor.Initialize(m_IO.LoadPrefabData());
        }

        /// <summary>
        /// Creates the compiled ICustomScript object 
        /// from the player created script
        /// </summary>
        /// <param name="customScript"></param>
        public void CompileScript()
        {
            if (m_att.Editor.ScriptEntry == null)
                return;

            ICustomScript script =
                m_att.Generator.GenerateCodeGraph
                (m_att.Editor.ScriptEntry);

            m_att.PlayerScript = script;

            Debug.Log("Script Saved");
        }

        /// <summary>
        /// If the script is already compiled
        /// then create the c# script
        /// </summary>
        public void GenerateCSharp()
        {
            if (m_att.Generator.IsCompiled)
                m_att.Generator.GenerateCSharp();
        }

        /// <summary>
        /// Triggers the player ship to create
        /// the NPC object
        /// </summary>
        public void TriggerControlModule()
        {
            // Only trigger the component if there is 
            // a script to build
            if(m_att.PlayerScript != null)
                m_att.Ship.Control.SpawnNPC(m_att.PlayerScript);
        }
    }
}
