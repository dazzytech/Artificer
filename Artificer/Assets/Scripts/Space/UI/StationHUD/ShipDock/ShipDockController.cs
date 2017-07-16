using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Space.Ship;
using Space.Ship.Components.Attributes;
using Space.Ship.Components.Listener;
using Space.UI.Station.Editor;
using Space.UI.Station.Prefabs;
using System.Linq;
using Data.Space;

namespace Space.UI.Station
{
    /// <summary>
    /// Performs tasks for the station HUD
    /// such as initializing views
    /// </summary>
    [RequireComponent(typeof(ShipDockAttributes))]
    public class ShipDockController : MonoBehaviour
    {
        #region DELEGATES

        public delegate void Select(string tab);

        public delegate void Create(GameObject GO);

        #endregion

        #region ATTRIBUTES

        [SerializeField]
        private ShipDockAttributes m_att;

        [SerializeField]
        private ShipDockEventListener m_event;

        #endregion

        #region PUBLIC INTERACTION

        #region INITIALIZE

        /// <summary>
        /// Initializes the station HUD.
        /// for now only pass ship atts
        /// Entry Point
        /// </summary>
        /// <param name="param">Parameter.</param>
        public void InitializeHUD(ShipAttributes ship)
        {
            m_att.Ship = ship;

            m_att.State = DockState.MANAGE;
            InitializeManage();
        }

        /// <summary>
        /// Builds the manage specific HUD
        /// and initializes it
        /// </summary>
        public void InitializeManage()
        {
            foreach (GameObject GO in m_att.EditGOs)
                GO.SetActive(false);

            foreach (GameObject GO in m_att.ManageGOs)
                GO.SetActive(true);

            m_att.Viewer.BuildShip(m_att.Ship);
        }

        public void InitializeEdit()
        {
            foreach (GameObject GO in m_att.ManageGOs)
                GO.SetActive(false);

            foreach (GameObject GO in m_att.EditGOs)
                GO.SetActive(true);

            BuildEditorPanel();

            // Initialize Edit Elements
            m_att.Editor.LoadExistingShip
                (m_att.Ship.Ship);
        }

        #endregion

        #endregion

        #region PRIVATE UTILITIES

        #region EDITOR CONTROLS

        /// <summary>
        /// Creates the ship editor window
        /// e.g component lists etc
        /// </summary>
        private void BuildEditorPanel()
        {
            // Populate the top tab bar with component types
            // Retreive all the directories
            TextAsset ShipKey = Resources.Load("Space/Keys/ship_key",
                                     typeof(TextAsset)) as TextAsset;

            // Add component type to header bar
            foreach (string item in ShipKey.text.Split(","[0]))
            {
                GameObject newTab = Instantiate(m_att.TabPrefab);
                newTab.transform.SetParent(m_att.TabHeader.transform);

                ComponentTabPrefab cmpTab = newTab.GetComponent<ComponentTabPrefab>();

                cmpTab.SetTab(item, new Select(SelectTab));
            }
        }

        /// <summary>
        /// Populates the component grid
        /// with components of selected type
        /// </summary>
        private void PopulateComponentList()
        {
            // Clear previous components
            foreach (Transform child in m_att.ItemPanel.transform)
                Destroy(child.gameObject);

            Vector2 newPos = new Vector2
                (m_att.ItemPanel.transform.localPosition.x, 0f);

            m_att.ItemScroll.value = 0;

            m_att.ItemPanel.transform.localPosition = newPos;

            foreach (GameObject GO in m_att.ComponentList)
            {
                // Only display component if in player unlock list or starter list
                ComponentListener Con = GO.GetComponent<ComponentListener>();

                // Change so that this uses a team list

                if (SystemManager.StarterList.Contains(GO))
                //if(SystemManager.GetPlayer.Components.Contains
                //   (string.Format("{0}/{1}", Con.ComponentType, GO.name))
                //   || m_att.StarterList.Contains(GO))
                {
                    // Create object
                    GameObject itemObj = Instantiate(m_att.ItemPrefab);
                    itemObj.transform.SetParent(m_att.ItemPanel.transform);

                    ComponentCreatePrefab item = itemObj.GetComponent<ComponentCreatePrefab>();
                    item.CreateItem(GO, new Create(CreateItem));
                }
            }
        }

        /// <summary>
        /// Rebuilds the component selector
        /// with the provided component category
        /// </summary>
        /// <param name="tab"></param>
        private void SelectTab(string tab)
        {
            m_att.ComponentList.Clear();

            m_att.ComponentList = Resources.LoadAll("Space/Ships/" + tab, typeof(GameObject))
                .Cast<GameObject>()
                    .ToList();

            PopulateComponentList();
        }

        /// <summary>
        /// Build an instance of the game object
        /// provided in the editor
        /// </summary>
        /// <param name="GO"></param>
        private void CreateItem(GameObject GO)
        {
            ComponentListener Con = GO.GetComponent<ComponentListener>();
            ComponentData component = new ComponentData();

            component.Folder = Con.ComponentType;
            component.Name = GO.name;
            component.Direction = "up";
            component.Style = "default";

            // We have a match
            m_att.Editor.BuildComponent(component);
        }

        #endregion

        #endregion
    }
}
