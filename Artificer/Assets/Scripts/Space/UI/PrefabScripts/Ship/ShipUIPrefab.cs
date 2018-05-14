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
        protected int m_shipReference;

        [SerializeField]
        protected bool m_spawnReady;

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

        /// <summary>
        /// While listening to ship
        /// ready, display the loading
        /// time for the ship
        /// </summary>
        [SerializeField]
        private HUDBar m_loading;

        /// <summary>
        /// When ship is ready to spawn,
        /// use to display ready
        /// </summary>
        [SerializeField]
        private Transform m_readyLabel;

        #endregion

        #endregion

        #region ACCESSOR

        public int ID
        { get { return m_shipReference; } }

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

        public ShipData Ship
        {
            get
            {
                return SystemManager.PlayerShips
                      [m_shipReference].Ship;
            }
        }

        public int Cost
        {
            get
            {
                return SystemManager.PlayerShips
                      [m_shipReference].Ship.Cost;
            }
        }

        public float Progress
        {
            get
            {
                return SystemManager.PlayerShips
                      [m_shipReference].SpawnTimer / Ship.SpawnTime;
            }
        }

        #endregion

        #region MONO BEHAVIOUR

        private void OnDisable()
        {
            if(SystemManager.Space != null)
                SystemManager.Space.OnShipSpawnUpdate -= DisplaySpawnProgress;
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

            if (m_readyLabel != null && m_loading != null)
            {
                // Engage listener and display initial value
                SystemManager.Space.OnShipSpawnUpdate
                += DisplaySpawnProgress;

                DisplaySpawnProgress(ID);
            }
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

            m_img.transform.localScale = new Vector3(1, 1, 1);
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

            if(m_readyLabel != null)
                m_readyLabel.gameObject.SetActive(false);
        }

        /// <summary>
        /// Progress clamped between 1 and 0
        /// Displays spawn progress on HUD bar
        /// </summary>
        /// <param name="id"></param>
        /// <param name="progress"></param>
        private void DisplaySpawnProgress(int id)
        {
            if (id != ID)
                return;

            // Check to see if 
            if(Progress >= 1f)
            {
                // we have successfully spawned
                m_loading.gameObject.SetActive(false);
                m_readyLabel.gameObject.SetActive(true);

                SystemManager.Space.OnShipSpawnUpdate -= DisplaySpawnProgress;
                DisplayIcon();
            }
            else
            {
                if (m_loading.gameObject.activeSelf != true)
                    m_loading.gameObject.SetActive(true);

                if (m_readyLabel.gameObject.activeSelf != false)
                    m_readyLabel.gameObject.SetActive(false);

                m_loading.Value = Progress;
            }
        }

        #endregion
    }
}