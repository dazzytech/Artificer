using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Space.Ship;
using Space.Ship.Components.Listener;
using UI;

namespace Space.UI.Station.Viewer
{
    /// <summary>
    /// Draw texture of each component to the 
    /// viewer Panel
    /// </summary>
    public class ShipViewer : MonoBehaviour
    {
        #region ATTRIBUTES

        #region HUD ELEMENTS

        // viewer window
        [SerializeField]
        private ComponentBuilderUtility ViewerPanel;

        // Prefab for component viewer
        [SerializeField]
        private GameObject PiecePrefab;

       // [SerializeField]
       // private Vector2 StartingPos;

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
            ViewerPanel.BuildShip(Ship, PiecePrefab);
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
        }

        public void SelectItem(int ID)
        {
            foreach (ViewerItem item in ViewerPanel.ViewerItems)
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
        }

        public void LeaveItem(int ID)
        {
            foreach (ViewerItem item in ViewerPanel.ViewerItems)
                if (item.ID == ID)
                {
                    item.Reset(false);
                    break;
                }
        }

        #endregion
    }
}
