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

        // add an accessor for the selected script

        public void Initialize(ShipAccessor ship)
        {
            m_att.Ship = ship;

            m_att.Editor.Initialize(m_IO.LoadPrefabData());
        }
    }
}
