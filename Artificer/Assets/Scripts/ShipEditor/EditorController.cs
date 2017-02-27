using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
// Artificer
using Data.Shared;
using Space.Ship.Components.Listener;

namespace Construction.ShipEditor
{
    [RequireComponent(typeof(EditorAttributes))]
    [RequireComponent(typeof(EditorListener))]
    public class EditorController : MonoBehaviour
    {
        private EditorAttributes _att;
        private EditorListener _listener;
        
        void Awake()
        {
            _att = GetComponent<EditorAttributes>();
            _listener = GetComponent<EditorListener>();

            _att.PiecesPlaced = 0;
            _att.ShipsCreated = 0;
            _att.ShipsDeleted = 0;
        }

        void Start()
        {
            _att.ComponentList = new List<GameObject>();
        }

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
        /// Initiates the building of the Component Creator window
        /// </summary>
        public void BuildPanel()
        {          
            // Populate the top tab bar with component types
            // Retreive all the directories
            TextAsset ShipKey = Resources.Load("Space/Keys/ship_key", 
                                     typeof(TextAsset)) as TextAsset;

            // Add component type to header bar
            foreach (string item in ShipKey.text.Split(","[0]))
            {
                GameObject newTab = Instantiate(_att.TabPrefab);
                newTab.transform.SetParent(_att.TabHeader.transform);
                
                ComponentTabPrefab cmpTab = newTab.GetComponent<ComponentTabPrefab>();
                
                cmpTab.SetTab(item, _listener);
            }

            UpdateShipList();
        }

        /// <summary>
        /// Populates the component grid
        /// with components of selected type
        /// </summary>
        public void PopulateComponentList()
        {
            // Clear previous components
            foreach (Transform child in _att.ItemPanel.transform)
                Destroy(child.gameObject);

            Vector2 newPos = new Vector2
                (_att.ItemPanel.transform.localPosition.x, 0f);

            _att.ItemScroll.value = 0;

            _att.ItemPanel.transform.localPosition = newPos;
            
            foreach (GameObject GO in _att.ComponentList)
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
                }*/
            }
        }

        public void UpdateShipList()
        {
            // Clear header panel of current ships
            foreach (Transform child in _att.ShipItemPanel.transform)
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
            }*/

            // Create new item icon
            GameObject newItem = Instantiate(_att.NewItemPrefab);
            newItem.transform.SetParent(_att.ShipItemPanel.transform);
            
            NewItemPrefab newItemItem = newItem.GetComponent<NewItemPrefab>();
            newItemItem.SetNewItem(_listener);
        }
    }
}
