using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;
using Menu; // use material list item till we create our own

namespace Construction.ShipEditor
{
    public class RequirementInventory : MonoBehaviour
    {/*
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

        void Start()
        {
            InitializeList();
        }

        /// <summary>
        /// Creates an empty list with only the players components.
        /// </summary>
        /// <param name="import">Import.</param>
        private void InitializeList()
        {
            /*
            // Add player cargo into current
            Dictionary<MaterialData, float> current = SystemManager.GetPlayer.Cargo;

            if (current == null)
                return;

            foreach (MaterialData mat in current.Keys)
            {
                // Update storage variables
                if(!_Materials.ContainsKey(mat))
                    _Materials.Add(mat, 0);
                
                // Update GUI ListBox
                foreach (MaterialListItem item in _MatListItems)
                {
                    if(item.MatIs(mat))
                    {
                        item.SetAmount(0f);
                    }
                } CreateListboxItem(mat).SetPlayerAmount(current [mat]);;
            }
        }
        
        /// <summary>
        /// Adds materials from ship storage
        /// to the gui listbox.
        /// </summary>
        /// <param name="import">Import.</param>
        public void AddMatsToList(Dictionary<MaterialData, float> requirements)
        {
            
            // Add player cargo into current
            Dictionary<MaterialData, float> current = SystemManager.GetPlayer.Cargo;

            // Clear each requirement for rebuilding
            foreach(Transform child in _Scroller.transform)
                    Destroy(child.gameObject);

            _Materials.Clear();
            _MatListItems.Clear();

            foreach (MaterialData mat in requirements.Keys)
            {
                // Update storage variables
                _Materials.Add(mat, requirements[mat]);

                // All lists have been cleared so we will need to just create items as we go along
                CreateListboxItem(mat);
            }

            if (current == null)
                return;

            foreach (MaterialData mat in current.Keys)
            {
                if(!_Materials.ContainsKey(mat))
                    _Materials.Add(mat, 0);

                // Update GUI ListBox
                bool itemExists = false;    // if item already exists in list
                // Iterate though each list item to see which already exists
                foreach (MaterialListItem item in _MatListItems)
                {
                    if(item.MatIs(mat))
                    {
                        item.SetPlayerAmount(current [mat]);
                        itemExists = true;
                        if(current [mat] >= _Materials [mat])
                           item.button.image.color = new Color(0f,1f,0f,.2f);
                    }
                }
                if(!itemExists) 
                    CreateListboxItem(mat).SetPlayerAmount(current [mat]);
            }
        }
        
        private MaterialListItem CreateListboxItem(MaterialData mat)
        {
            // Game object
            GameObject newItem = Instantiate(MatButtonPrefab) as GameObject;
            newItem.transform.SetParent (_Scroller.transform, false);
            
            // List item
            MaterialListItem matItem = newItem.GetComponent<MaterialListItem>();
            matItem.SetMaterial(mat);
            matItem.SetAmount(_Materials [mat]);
            
            // Button functionality
            if(_Materials[mat] == 0)
                matItem.button.image.color = new Color(0f,0f,0f,0f);
            else
                matItem.button.image.color = new Color(1f,0f,0f,.2f);
            
            // Add to LB list
            _MatListItems.Add(matItem);
            return matItem;
        }*/
    }
}