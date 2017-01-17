using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Space.Ship;
using Space.Ship.Components.Listener;

namespace Space.UI.Station.Viewer
{
    /// <summary>
    /// Draw texture of each component to the 
    /// viewer Panel
    /// </summary>
    public class ShipViewer : MonoBehaviour
    {
        #region ATTRIBUTES

        private List<ViewerItem> Items;

        #region HUD ELEMENTS

        // viewer window
        [SerializeField]
        private GameObject ViewerPanel;

        // Prefab for component viewer
        [SerializeField]
        private GameObject PiecePrefab;

        [SerializeField]
        private Vector2 StartingPos;

        #endregion

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// stores reference to the ship and builds
        /// each component within viewer
        /// </summary>
        /// <param name="Ship"></param>
        public void BuildShip(ShipAttributes Ship)
        {
            Items = new List<ViewerItem>();

            foreach(ComponentListener comp in Ship.Components)
            {
                // Create prefab and apply components
                BuildComponent(comp);
            }
        }

        /// <summary>
        /// When components are highlighted
        /// this function will clear 
        /// </summary>
        public void ClearHighlights()
        {
            foreach (ViewerItem item in Items)
                item.Reset(true);
        }

        public void ClearItem(int ID)
        {
            foreach (ViewerItem item in Items)
                if (item.ID == ID)
                {
                    item.Reset(true);
                    break;
                }
        }

        public void SelectItem(int ID)
        {
            foreach (ViewerItem item in Items)
                if (item.ID == ID)
                {
                    item.Select();
                    break;
                }
        }

        public void HoverItem(int ID)
        {
            foreach (ViewerItem item in Items)
                if (item.ID == ID)
                {
                    item.Highlight();
                    break;
                }
        }

        #endregion

            #region PRIVATE UTILITIES

            /// <summary>
            /// Create a piece for component
            /// </summary>
            /// <param name="comp"></param>
        private void BuildComponent(ComponentListener comp)
        {
            // Rather than lock sockets, just copy local position
            Vector2 location = (comp.transform.localPosition * 100f);
            location += StartingPos;
            GameObject newObj = Instantiate(PiecePrefab);

            newObj.transform.SetParent(ViewerPanel.transform, false);
            newObj.transform.localPosition = location;

            ViewerItem item = newObj.GetComponent<ViewerItem>();

            item.Define(comp.gameObject, comp.ID);

            Items.Add(item);
        }

        #endregion
    }
}
