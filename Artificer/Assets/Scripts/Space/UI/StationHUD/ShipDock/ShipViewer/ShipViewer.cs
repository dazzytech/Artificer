using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Space.Ship;
using Space.Ship.Components.Listener;
using UI;
using UnityEngine.UI;

namespace Space.UI.Station.Viewer
{
    // Used by both component types when mouse interacts with component 
    // so both items highlight etc
    public delegate void SelectEvent(int ID);

    /// <summary>
    /// Draw texture of each component to the 
    /// viewer Panel
    /// </summary>
    public class ShipViewer : MonoBehaviour 
    {
        #region ATTRIBUTES

        #region HUD ELEMENTS

        #region SHIP VIEWER

        // viewer window
        [SerializeField]
        private ComponentBuilderUtility ViewerPanel;

        // Prefab for component viewer
        [SerializeField]
        private GameObject PiecePrefab;

        #endregion

        #region COMPONENT LIST

        [SerializeField]
        private GameObject m_selectionListPrefab;
        [SerializeField]
        private GameObject m_selectionListPanel;
        [SerializeField]
        private Scrollbar m_selectionListScroll;

        [HideInInspector]
        public List<ComponentListItem> Items;

        #endregion

        #endregion

        public ShipAttributes Ship;

        // If the viewer is currently doing something?
        public bool Busy;

        [HideInInspector]
        // list of components selected to repair
        public List<int> SelectedIDs;

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// stores reference to the ship and builds
        /// each component within viewer
        /// </summary>
        /// <param name="Ship"></param>
        public void BuildShip(ShipAttributes ship)
        {
            Ship = ship;

            ViewerPanel.BuildShip(Ship, PiecePrefab);

            // Initialize Manage Elements
            BuildComponentPanel();
        }

        /// <summary>
        /// When components are highlighted
        /// this function will clear 
        /// </summary>
        public void ClearHighlights()
        {
            foreach (ViewerItem item in ViewerPanel.ViewerItems)
                item.Reset(true);
        }

        public void ClearItem(int ID)
        {
            foreach (ViewerItem item in ViewerPanel.ViewerItems)
                if (item.ID == ID)
                {
                    item.Reset(true);
                    break;
                }

            foreach (ComponentListItem item in Items)
                if (item.ID == ID)
                {
                    item.Reset(true);
                    break;
                }
        }

        public void SelectItem(int ID)
        {
            foreach (ViewerItem item in ViewerPanel.ViewerItems)
                if (item.ID == ID)
                {
                    item.Select();
                    break;
                }

            foreach (ComponentListItem item in Items)
                if (item.ID == ID)
                {
                    item.Select();
                    break;
                }
        }

        public void HoverItem(int ID)
        {
            foreach (ViewerItem item in ViewerPanel.ViewerItems)
                if (item.ID == ID)
                {
                    item.Highlight();
                    break;
                }
            foreach (ComponentListItem item in Items)
                if (item.ID == ID)
                {
                    item.Highlight();
                    break;
                }
        }

        public void LeaveItem(int ID)
        {
            foreach (ViewerItem item in ViewerPanel.ViewerItems)
                if (item.ID == ID)
                {
                    item.Reset(false);

                    
                    break;
                }
            foreach (ComponentListItem item in Items)
                 if (item.ID == ID)
                 {
                      item.Reset(false);
                      break;
                 }

        }

        #endregion

        #region INTERNAL INTERACTION

        private void BuildComponentPanel()
        {
            Busy = false;

            Items = new List<ComponentListItem>();

            // Clear previous components
            foreach (Transform child in m_selectionListPanel.transform)
                Destroy(child.gameObject);

            // Reset scroller positoning
            Vector2 newPos = new Vector2
                (m_selectionListPanel.transform.localPosition.x, 0f);

            m_selectionListScroll.value = 0;

            m_selectionListPanel.transform.localPosition = newPos;

            // loop for each component currently stored
            foreach (ComponentListener comp in Ship.Components)
            {
                // Create a component list item
                GameObject newCLI = Instantiate(m_selectionListPrefab);

                // Set Parent
                newCLI.transform.SetParent
                    (m_selectionListPanel.transform, false);

                // Retrieve behavior to initialize
                ComponentListItem lItem =
                    newCLI.GetComponent<ComponentListItem>();

                // Now initialise
                lItem.DefineComponent(comp.GetAttributes());

                Items.Add(lItem);
            }
        }

        /// <summary>
        /// Removes selected components and resets color of 
        /// each component item
        /// </summary>
        private void ClearSelection()
        {
            SelectedIDs.Clear();

            // Get each list item and revert its colour
            foreach (ComponentListItem child in Items)
                child.Reset(true);

            ClearHighlights();
        }

        #endregion

        #region COROUTINES

        /// <summary>
        /// Loop through components and slowly heal each
        /// </summary>
        /// <returns></returns>
        public IEnumerator HealComponents()
        {
            Busy = true;

            foreach (ComponentListener comp in
                Ship.Components)
            {
                if (SelectedIDs.Contains
                    (comp.GetAttributes().ID))
                {
                    while (comp.GetAttributes().NormalizedHealth < 1f)
                    {
                        comp.HealComponent(0.3f);

                        yield return null;
                    }
                }

                yield return null;
            }

            Busy = false;

            ClearSelection();

            StopCoroutine("HealComponents");

            yield return null;
        }

        #endregion
    }
}
