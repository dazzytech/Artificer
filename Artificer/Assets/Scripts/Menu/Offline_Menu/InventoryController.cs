using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;

namespace Menu
{
    public class InventoryController : MonoBehaviour
    {
        // Panel to manipulate
        public Transform _Scroller;
        
        // prefabs
        public GameObject MatButtonPrefab;
        
        // GUI Lists
        private Dictionary<MaterialData, float> _Materials;
        private List<MaterialListItem> _MatListItems;

        void Awake () 
        {
            // Set Lists
            _Materials = new Dictionary<MaterialData, float>();
            _MatListItems = new List<MaterialListItem>();
        }
    	
        // Update is called once per frame
        void Update () 
        {           
            /*if (GameManager.GetPlayer.Cargo != null)
            {
                if (GameManager.GetPlayer.Cargo.Count == 0)
                {
                    foreach(Transform child in _Scroller.transform)
                        Destroy(child.gameObject);
                }
                UpdateStorageContents();
            } */
        }

        /// <summary>
        /// Updates the storage contents
        /// within the GUI.
        /// </summary>
        private void UpdateStorageContents()
        {
            // Clear the current storage each time
            // to recount
            _Materials.Clear();

            //AddMatsToList(GameManager.GetPlayer.Cargo);
        }
        
        /// <summary>
        /// Adds materials from ship storage
        /// to the gui listbox.
        /// </summary>
        /// <param name="import">Import.</param>
        private void AddMatsToList(Dictionary<MaterialData, float> import)
        {
            foreach (MaterialData mat in import.Keys)
            {
                // Update storage variables
                if(_Materials.ContainsKey(mat))
                    _Materials[mat] += import[mat];
                else
                    _Materials.Add(mat, import[mat]);
                
                // Update GUI ListBox
                bool itemExists = false;                        // if item already exists in list
                foreach (MaterialListItem item in _MatListItems)
                {
                    /*if(item.MatIs(mat))
                    {
                        item.SetAmount(_Materials[mat]);
                        itemExists = true;
                    }*/
                }
                if(!itemExists) CreateListboxItem(mat);
            }
        }
        
        private void CreateListboxItem(MaterialData mat)
        {
            // Game object
            GameObject newItem = Instantiate(MatButtonPrefab) as GameObject;
            newItem.transform.SetParent (_Scroller.transform, false);
            
            // List item
            MaterialListItem matItem = newItem.GetComponent<MaterialListItem>();
            matItem.SetMaterial(mat);
            matItem.SetAmount(_Materials [mat]);
            
            // Button functionality
            matItem.button.image.color = new Color(0f,0f,0f,0f);
            
            // Add to LB list
            _MatListItems.Add(matItem);
        }
    }
}