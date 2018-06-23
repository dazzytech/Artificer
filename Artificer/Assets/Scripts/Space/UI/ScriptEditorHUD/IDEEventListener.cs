using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generator;
using Space.AI;

namespace Space.UI.IDE
{
    /// <summary>
    /// Events that manage the IDE state or sub objects
    /// </summary>
    public class IDEEventListener : MonoBehaviour
    {
        [SerializeField]
        private IDEController m_con;

        #region BUTTON EVENTS

        /// <summary>
        /// Close IDE and return to game, apply any changes
        /// and re-enable ship
        /// </summary>
        public void ExitIDE()
        {
            SystemManager.UIState.RevertState();

            SystemManager.Space.Ship.EnableShip();
        }

        /// <summary>
        /// Invoke the compiler with the start of the 
        /// node script 
        /// </summary>
        public void CompileAgent()
        {
            m_con.CompileScript();
        }

        /// <summary>
        /// Build the C# script of the compiled graph
        /// </summary>
        public void CreateScriptCSharp()
        {
            m_con.GenerateCSharp();
        }

        /// <summary>
        /// When the button is pressed
        /// trigger the controller to create a NPC instance
        /// </summary>
        public void SpawnAgent()
        {
            m_con.TriggerControlModule();
        }

        #endregion
    }
}