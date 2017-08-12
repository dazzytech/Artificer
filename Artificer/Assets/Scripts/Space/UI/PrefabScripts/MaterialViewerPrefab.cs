using System.Collections;
using System.Collections.Generic;
using UI;
using UI.Effects;
using UnityEngine;
using UnityEngine.UI;

namespace Space.UI
{
    /// <summary>
    /// Used to display the amount of a material 
    /// stored in the ship
    /// </summary>
    public class MaterialViewerPrefab : SelectableHUDItem
    {
        #region ATTRIBUTES

        [Header("Storage")]

        /// <summary>
        /// displays the name
        /// </summary>
        [Header("HUD Elements")]
        [SerializeField]
        private Text m_label;

        /// <summary>
        /// Array of HUDbars values 
        /// </summary>
        [SerializeField]
        private HUDBar[] m_bars;

        [SerializeField]
        private Text[] m_labels;

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Visually updates bars to display 
        /// storage value
        /// value 0f - 1f
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public void DisplayValue(string name, float[] values, string[] labels = null)
        {
            // display text
            m_label.text = name;

            for (int i = 0; i < values.Length; i++)
                m_bars[i].Value = values[i];

            if(labels != null)
                for (int i = 0; i < labels.Length; i++)
                    m_labels[i].text = labels[i];
        }

        #endregion
    }
}
