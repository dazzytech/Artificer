using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
// Artificer Defined
using Space.Ship;
using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;

namespace Space.UI.Ship
{
    public class IntegrityHUD : MonoBehaviour
    {
        // Ship attributes for player ship
        private ShipAttributes _shipData;

        // GUI HUD
        private Transform _IntegrityHUD;
        private Transform _Scroller;

        // prefabs
        public GameObject IntButtonPrefab;

        // List
        private List<IntegrityListItem> _IntListItems;
        private List<ComponentListener> _CompList;

        // Use this for initialization
        void Awake()
        {
            _IntegrityHUD = transform.Find("IntegrityHUD");
            _Scroller = _IntegrityHUD.Find("ScrollView");

            _IntListItems = new List<IntegrityListItem>();
            _CompList = new List<ComponentListener>();
        }
    	
        public void SetShipData(ShipAttributes data)
        {
            _shipData = data;
            ClearAll();
            // Update the color state to the existing buttons
            // create pieces for each new ship
            if (_shipData.Components.Count != 0)
                UpdateComponentList();
        }

        // Update is called once per frame
        void Update()
        {
            // Do not update if ship data is null
            if (_shipData == null)
            {
                // hide HUD
                if(_IntegrityHUD.gameObject.activeSelf)
                    _IntegrityHUD.gameObject.SetActive(false);


                return; 
            }
            else
                // show HUD if hidden
                if(!_IntegrityHUD.gameObject.activeSelf)
                    _IntegrityHUD.gameObject.SetActive(true);

            if (_CompList.Count != 0)
                UpdateIntegrityList();
        }

        /// <summary>
        /// Adds a new item to the integrity list
        /// </summary>
        private void UpdateComponentList()
        {
            foreach (ComponentListener comp in _shipData.Components)
            {
                if(comp != null)
                {
                    ComponentAttributes att = comp.GetAttributes();

                    if(att.TrackIntegrity && !_CompList.Contains(comp))
                    {
                        _CompList.Add(comp);
                    }
                }
            }
        }

        private void UpdateIntegrityList()
        {
            List<ComponentListener> currentComps = new List<ComponentListener>();
            List<IntegrityListItem> removeItem = new List<IntegrityListItem>();
            List<ComponentListener> removeComp = new List<ComponentListener>();
            foreach (IntegrityListItem item in _IntListItems)
            {
                if(item.Component == null)
                {
                        Destroy(item.gameObject);
                        removeItem.Add(item);
                        continue;
                }

                ComponentAttributes att = item.Component.GetAttributes();
                float healthPerc = (att.Integrity / item.Component.GetAttributes().MaxIntegrity * 100);
                
                if(att.Integrity > 0)
                    item.Status.text = att.Name + ": " + 
                        healthPerc.ToString("F0") + "%\n";
                else if(item != null)
                {
                    removeItem.Add(item);
                    Destroy(item.gameObject);
                }
                
                if(item.button != null)
                {
                    if(healthPerc < 40.0f)
                    {
                        item.button.image.color = new Color(.8f,.2f,.2f,.2f);
                    } 
                    else if(healthPerc < 60.0f)
                    {
                        item.button.image.color = new Color(.8f,.5f,.2f,.2f);
                    }
                }

                currentComps.Add(item.Component);
            }

            foreach (ComponentListener comp in _CompList)
            {
                if(comp == null)
                {
                    removeComp.Add(comp);
                    continue;
                }
                if (!currentComps.Contains(comp))
                    CreateListboxItem(comp);
            }

            foreach (ComponentListener c in removeComp)
                _CompList.Remove(c);
            foreach (IntegrityListItem l in removeItem)
                _IntListItems.Remove(l);
        }

        //CreateListboxItem(comp, att);
        private void CreateListboxItem
            (ComponentListener comp)
        {
            ComponentAttributes att = comp.GetAttributes();

            // Game object
            GameObject newItem = Instantiate(IntButtonPrefab) as GameObject;
            newItem.transform.SetParent (_Scroller.Find("BtnList").transform, false);
            
            // List item
            IntegrityListItem intItem = newItem.GetComponent<IntegrityListItem>();
            intItem.Status.text = att.Name + ": " + 
                (att.Integrity / comp.GetAttributes().MaxIntegrity * 100).ToString("F0") + "%\n";
            intItem.Component = comp;
                
            // Button functionality
            intItem.button.image.color = new Color(0f,0f,0f,0f);
            
            // Add to LB list
            _IntListItems.Add(intItem);
        }

        private void ClearAll()
        {
            _CompList.Clear();
            foreach (IntegrityListItem i in _IntListItems)
            {
                Destroy(i.gameObject);
            }
            _IntListItems.Clear();
        }
    }
}
