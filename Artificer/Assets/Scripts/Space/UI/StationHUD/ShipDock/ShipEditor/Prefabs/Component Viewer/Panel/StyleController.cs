using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;
using Space.UI.Station.Editor.Component;

namespace Space.UI.Station.Utility
{
    public class StyleController : BaseRCPanel
    {
        #region ATTRIBUTES

        /// <summary>
        /// Used for iterating through style list
        /// </summary>
        private List<string> m_styleChoices;

        /// <summary>
        /// Used for selecting a style
        /// </summary>
        [SerializeField]
        private GameObject m_togglePrefab;

        #endregion

        #region PUBLIC INTERACTION

        public override void Display(ComponentAttributes att, BaseComponent bC)
        {
            m_styleChoices = new List<string>();

            m_toggles = new Toggle[att.componentStyles.Length];

            // create list of toggles for each colour
            for (int i = 0; i < att.componentStyles.Length; i++)
            {
                StyleInfo style = att.componentStyles[i];

                GameObject newToggle = Instantiate(m_togglePrefab);
                newToggle.transform.SetParent(transform);

                Toggle toggle = newToggle.transform
                    .Find("HeadToggle").GetComponent<Toggle>();

                newToggle.transform.Find("HeadToggle")
                    .Find("Label").GetComponent<Text>().text = style.name;

                m_toggles[i] = toggle;

                m_styleChoices.Add(style.name);

                if (bC.Style == style.name)
                    m_index = i;
            }

            base.Display(att, bC);
        }

        #endregion

        #region PRIVATE UTILITIES

        protected override void ApplyToggle(int index)
        {
            m_BC.Style = m_styleChoices[index];
        }

        #endregion
    }
}
