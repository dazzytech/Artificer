using Data.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Space.UI
{
    /// <summary>
    /// Selectable item that displays
    /// data about ship spawns
    /// </summary>
    public class BuildStationPrefab : MonoBehaviour
    {
        #region ATTRIBUTES

        [Header("HUD Elements")]

        [SerializeField]
        private Text m_label;

        [SerializeField]
        private Text m_desc;

        [SerializeField]
        private Text m_type;

        [SerializeField]
        private Text m_cost;

        [SerializeField]
        private RawImage m_icon;

        #endregion

        #region PUBLIC INTERACTION

        public void Display(DeployData station)
        {
            if(m_label != null) m_label.text = station.Name;
            if (m_desc != null) m_desc.text = station.Description;
            if (m_type != null) m_type.text = station.Type;
            if (m_cost != null) m_cost.text = string.Format("¤ {0}", station.Cost);
            if (m_icon != null) m_icon.texture = Resources.Load("Textures/StationTextures/" +
                station.IconPath, typeof(Texture2D)) as Texture2D;
        }

        #endregion
    }
}