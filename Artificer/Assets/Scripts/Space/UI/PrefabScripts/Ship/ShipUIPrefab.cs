using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UI;
using System;
using Data.Space;

namespace Space.UI
{
    public class ShipUIPrefab : SelectableHUDItem
    {
        #region ATTRIBUTES

        [Header("Ship Information")]

        /// <summary>
        /// used to access the ship list
        /// within playerdata
        /// </summary>
        [SerializeField]
        private int m_shipReference;

        #region HUD ELEMENTS

        [Header("HUD Elements")]
        [SerializeField]
        private Text m_header;

        [SerializeField]
        private Text m_type;

        [SerializeField]
        private Text m_desc;

        [SerializeField]
        private RawImage m_img;

        #endregion

        #endregion

        #region ACCESSOR

        public string Name
        {
            get { return SystemManager.PlayerShips
                    [m_shipReference].ShipName; }
        }

        public string Description
        {
            get { return SystemManager.PlayerShips
                    [m_shipReference].Ship.Description; }
        }

        public string Category
        {
            get { return SystemManager.PlayerShips
                    [m_shipReference].Ship.Category; }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Uses name to access ship from 
        /// player list and visualises the data
        /// </summary>
        /// <param name="newShip"></param>
        public virtual void AssignShip(int shipIndex)
        {
            // keep reference
            m_shipReference = shipIndex;

            // Display info
            DisplayIdentifier();

            DisplayIcon();            
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Retrieves the icon stored for that
        /// particular ship and displays it to our image
        /// </summary>
        private void DisplayIcon()
        {
            // child objects may not have image
            if (m_img == null)
                return;

            // Access the image
            Texture2D shipIcon = Resources.Load("Space/Ships/Icon/"
                + Name, typeof(Texture2D)) as Texture2D;

            m_img.texture = shipIcon;

            // scales the ship down to fit in the image
            float sizeScale = m_img.GetComponent<RectTransform>()
                .rect.height / m_img.texture.height;

            m_img.transform.localScale = new Vector3(sizeScale, sizeScale, 1);
        }

        /// <summary>
        /// displays information regarding the ship
        /// </summary>
        private void DisplayIdentifier()
        {
            // Display Title and description
            if(m_header != null)
             m_header.text = Name;

            if (m_desc != null)
                m_desc.text = Description;

            if (m_type != null)
                m_type.text = Category;
        }

        #endregion
    }
}