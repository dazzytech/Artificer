using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using Space.Ship;
using Space.Ship.Components.Listener;

namespace Space.UI.Ship
{
    public class ShieldHUD : MonoBehaviour
    {
        // Ship attributes for player ship
        private ShipAttributes _shipData;
        
        // GUI HUD
        public Transform _ShieldHUD;
        public Transform _listHUD;

        // prefabs
        public GameObject ItemPrefab;
        
        // List
        private List<ShieldItemPrefab> _ListItems;

        // Use this for initialization
        void Awake()
        {
            _ListItems = new List<ShieldItemPrefab>();
        }

        public void SetShipData(ShipAttributes data)
        {
            _shipData = data;
            ClearAll();
            // Update the color state to the existing buttons
            // create pieces for each new ship
            if (_shipData.Shields.Count != 0)
                BuildList();
            else if(_ShieldHUD.gameObject.activeSelf)
                    _ShieldHUD.gameObject.SetActive(false);
        }
    	
        // Update is called once per frame
        void Update()
        {
            // Do not update if ship data is null
            if (_shipData == null)
            {
                // hide HUD
                if(_ShieldHUD.gameObject.activeSelf)
                    _ShieldHUD.gameObject.SetActive(false);
                
                
                return; 
            }
            else if (_shipData.Shields.Count != 0)
                // show HUD if hidden
                if(!_ShieldHUD.gameObject.activeSelf)
                    _ShieldHUD.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Adds a new item to the integrity list
        /// </summary>
        private void BuildList()
        {
            foreach (ShieldListener comp in _shipData.Shields)
            {
                if(comp != null)
                {
                    CreateListboxItem(comp);
                }
            }
        }
        
        //CreateListboxItem(comp, att);
        private void CreateListboxItem
            (ShieldListener comp)
        {
            // Game object
            GameObject newItem = Instantiate(ItemPrefab) as GameObject;
            newItem.transform.SetParent (_listHUD.transform, false);
            
            // List item
            ShieldItemPrefab item = newItem.GetComponent<ShieldItemPrefab>();
            item.SetShield(comp);
            
            // Add to LB list
            _ListItems.Add(item);
        }

        private void ClearAll()
        {
            foreach (ShieldItemPrefab i in _ListItems)
            {
                Destroy(i.gameObject);
            }
            _ListItems.Clear();
        }
    }
}
