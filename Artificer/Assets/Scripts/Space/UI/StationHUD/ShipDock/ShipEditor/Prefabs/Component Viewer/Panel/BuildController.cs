using System.Collections;
using System.Collections.Generic;
using Space.Ship.Components.Attributes;
using Space.UI.Station.Editor.Component;
using UnityEngine;
using UnityEngine.UI;
using Data.UI;

namespace Space.UI.Station.Utility
{
    /// <summary>
    /// Creates a list of 
    /// construction options for a builder
    /// component
    /// </summary>
    public class BuildController : BaseRCPanel
    {
        #region ATTRIBUTES

        /// <summary>
        /// UI Element that displays 
        /// the construction 
        /// </summary>
        [Header("Build Controller")]
        [SerializeField]
        private GameObject m_buildPrefab;

        #endregion

        #region MONO BEHAVIOUR

        private void OnDisable()
        {
            // delete previously made styles
            foreach (Transform child in m_panel)
                Destroy(child.gameObject);
        }

        #endregion

        #region PUBLIC INTERACTION

        public override void Display(ComponentAttributes att, BaseComponent bC = null)
        {
            // delete previously made styles
            foreach (Transform child in m_panel)
                Destroy(child.gameObject);

            BuildAttributes build = att as BuildAttributes;
            
            // Create a prefab instance for each template
            foreach(DeployData template in build.SpawnableStations)
            {
                GameObject inst = Instantiate(m_buildPrefab);
                inst.transform.SetParent(m_panel);

                inst.GetComponent<BuildStationPrefab>().Display(template);
            }
        }

        #endregion
    }
}
