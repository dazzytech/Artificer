using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
// Artificer
using Data.Space;
using Space.Ship.Components.Listener;

namespace Construction.ShipEditor
{
    [RequireComponent(typeof(EditorAttributes))]
    [RequireComponent(typeof(EditorListener))]
    public class EditorController : MonoBehaviour
    {
        

        /*#region ATTRIBUTES

        [SerializeField]
        private EditorAttributes m_att;
        [SerializeField]
        private EditorListener m_listener;

        #endregion

        #region MONO BEHAVIOUR

        void OnEnable()
        {
            m_att.PiecesPlaced = 0;
            m_att.ShipsCreated = 0;
            m_att.ShipsDeleted = 0;

            m_att.ComponentList = new List<GameObject>();

            BuildPanel();
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Initializes the construction.
        /// Entry Point
        /// </summary>
        /// <param name="param">Parameter.</param>
        public void InitializeEditor()
        {
            gameObject.SetActive(true);
            BuildPanel();

            // music
            SoundController.PlayMusic(new int[3]{1, 2, 3});
        }

        /// <summary>
        /// Populates the component grid
        /// with components of selected type
        /// </summary>
        public void PopulateComponentList()
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
                /*
                if(SystemManager.GetPlayer.Components.Contains
                   (string.Format("{0}/{1}", Con.ComponentType, GO.name))
                   || _att.StarterList.Contains(GO))
                {
                    // Create object
                    GameObject itemObj = Instantiate(_att.ItemPrefab);
                    itemObj.transform.SetParent(_att.ItemPanel.transform);
                    
                    ComponentItemPrefab item = itemObj.GetComponent<ComponentItemPrefab>();
                    item.CreateItem(GO, _listener);
               }
            }
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Initiates the building of the Component Creator window
        /// </summary>
        private void BuildPanel()
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
                
                cmpTab.SetTab(item, m_listener);
            }

            //UpdateShipList();
        }

        #endregion

        public void UpdateShipList()
        {
            // Clear header panel of current ships
            foreach (Transform child in m_att.ShipItemPanel.transform)
                Destroy(child.gameObject);

            // populate header with existing ship items.
            /*if (SystemManager.GetPlayer.ShipList != null)
            {
                // Build Ship item prefabs using shiplist
                foreach (ShipData ship in SystemManager.GetPlayer.ShipList)
                {
                    GameObject item = Instantiate(_att.ShipItemPrefab);
                    item.transform.SetParent(_att.ShipItemPanel.transform);
                    
                    ShipItemPrefab shipItem = item.GetComponent<ShipItemPrefab>();
                    shipItem.SetShipItem(ship, _listener);
                }
            }

            // Create new item icon
            GameObject newItem = Instantiate(m_att.NewItemPrefab);
            newItem.transform.SetParent(m_att.ShipItemPanel.transform);
            
            NewItemPrefab newItemItem = newItem.GetComponent<NewItemPrefab>();
            newItemItem.SetNewItem(m_listener);
        }*/
    }
}
