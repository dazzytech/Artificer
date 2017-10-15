using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Artificer
using Space.Ship;
using Data.Space.Library;
using Space.Ship.Components.Listener;
using System.Linq;
using UI;

namespace Space.UI.Ship
{
    /// <summary>
    /// displays the current storage attributes 
    /// within the player ship
    /// </summary>
    public class StorageHUD : HUDPanel
    {
        #region ATTRIBUTES

        /// <summary>
        /// reference to the player ship
        /// </summary>
        private ShipAccessor m_shipRef;

        /// <summary>
        /// Stores a reference to a 
        /// storage item using their ID
        /// </summary>
        private Dictionary<int, MaterialViewerPrefab> m_storageItems;

        private MaterialViewerPrefab m_capacity;

        #region HUD ELEMENTS

        [Header("HUD Elements")]

        [SerializeField]
        private Transform m_storageList;

        [SerializeField]
        private GameObject m_storagePrefab;

        #endregion

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Add refernce to the ship and add event
        /// listeners for storage update
        /// </summary>
        /// <param name="data"></param>
        public void SetShip(ShipAccessor data)
        {
            m_shipRef = data;

            foreach (Transform child in m_storageList)
                Destroy(child.gameObject);

            m_storageItems = new Dictionary<int, MaterialViewerPrefab>();

            m_shipRef.OnShipCompleted += OnShipCompleted;

            m_shipRef.OnStorageChanged += OnStorageUpdate;
        }

        #endregion

        #region PRIVATE UTILITIES

        private void OnShipCompleted()
        {
            // Hide if there is no storage capacity
            if (m_shipRef.Storage.Count == 0)
                // delete this
                gameObject.SetActive(false);
            else
                gameObject.SetActive(true);
        }

        /// <summary>
        /// When a material is added to the 
        /// storage, 
        /// </summary>
        /// <param name="yield"></param>
        private void OnStorageUpdate()
        {
            // firstly discover the total capacity of the ship
            float capacity = 0;
            float used = 0;

            // total storage 0 - 1f
            float total = 0;

            Dictionary<int, float> current = new Dictionary<int, float>();

            foreach (StorageListener storage in m_shipRef.Storage)
            {
                capacity += storage.Capacity;
                used += storage.Used;

                foreach (int key in storage.Materials.Keys)
                {
                    if (!current.ContainsKey(key))
                        current.Add(key, 0);

                    current[key] += storage.Materials[key];
                }
            }

            // Create or update a list item displaying the 
            // current capacity
            if (m_capacity == null)
            {
                // if not add a new prefab to the HUD
                GameObject newStorageItem = Instantiate(m_storagePrefab);
                newStorageItem.transform.SetParent(m_storageList);
                // Store our capacity
                m_capacity = newStorageItem.GetComponent
                    <MaterialViewerPrefab>();
                m_capacity.Initialize(null);
            }

            total = used / capacity;

            if (total > 0)
                m_capacity.DisplayValue("Total", new float[1] { total });
            else
            {
                GameObject.Destroy(m_capacity.gameObject);
                m_capacity = null;
            }

            foreach (int item in current.Keys)
            {
                // See if we have previously built this
                if (!m_storageItems.ContainsKey(item))
                {
                    // if not add a new prefab to the HUD
                    GameObject newStorageItem = Instantiate(m_storagePrefab);
                    newStorageItem.transform.SetParent(m_storageList);
                    // Store our item using index
                    MaterialViewerPrefab storage = newStorageItem.
                        GetComponent<MaterialViewerPrefab>();
                    m_storageItems.Add(item, storage);
                    storage.Initialize(null);
                }

                // Add added amount to display
                m_storageItems[item].DisplayValue(SystemManager.Items[item].Name,
                    new float[2] { current[item] / capacity, total });
            }


            // Remove any requirements that are no longer 
            // included
            for (int i = 0; i < m_storageItems.Count; i++)
            {
                if (!current.ContainsKey(m_storageItems.Keys.ToArray()[i]))
                {
                    Destroy(m_storageItems.Values.ToArray()[i].gameObject);
                    m_storageItems.Remove(m_storageItems.Keys.ToArray()[i]);
                }
            }
        }

        #endregion
    }
}
