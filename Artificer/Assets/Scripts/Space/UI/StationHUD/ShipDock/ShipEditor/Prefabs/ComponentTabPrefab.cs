using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Space.UI.Station.Prefabs
{
    /// <summary>
    /// Allows the user to select 
    /// different component categories
    /// </summary>
    public class ComponentTabPrefab : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private Text m_label;
        [SerializeField]
        private Button m_button;

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Initializes the tab and 
        /// assigns behaviour
        /// </summary>
        /// <param name="item"></param>
        /// <param name="select"></param>
        public void SetTab(string item,
            ShipDockController.Select select)
        {
            m_label.text =  item;
            m_button.onClick.AddListener
                (delegate{select(item);});
        }

        #endregion
    }
}
