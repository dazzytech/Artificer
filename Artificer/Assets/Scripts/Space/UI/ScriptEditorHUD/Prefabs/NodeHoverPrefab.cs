using Data.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Space.UI.IDE
{
    public class NodeHoverPrefab : MonoBehaviour
    {
        #region ATTRIBUTES

        #region HUD ELEMENTS

        [Header("Hover HUD Elements")]

        [SerializeField]
        private Text m_label;

        [SerializeField]
        private Text m_category;

        [SerializeField]
        private Text m_description;

        #endregion

        #endregion

        #region PUBLIC INTERACTION

        public void Display(NodeData node)
        {
            m_label.text = node.Label;
            m_category.text = node.Category;
            m_description.text = node.Description;
        }

        #endregion
    }
}