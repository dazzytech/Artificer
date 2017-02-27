using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;
using Data.Space.Library;

namespace Construction.ShipEditor
{
    [RequireComponent(typeof(ShipUIUtility))]
    /// <summary>
    /// Ship editor controller.
    /// script that controls and manages all the editor window
    /// </summary>
    public class ShipEditorController : MonoBehaviour
    {
        // Ship is stored in a designated container
        /*private ShipContainer _ship;

        private ShipUIUtility _util;

        private ShipTextureUtil _tex;

        private ShipRequirementsUtility _req;

        // UI Elements
        public Transform ShipConstructPanel;
        public Transform ShipBoundsPanel;
        public RequirementInventory Inventory;
        public GameObject ShipCompPrefab;
        public InputField ShipName;
        public bool Changed; 

        // MonoBehavours
        void Awake()
        {
            _util = GetComponent<ShipUIUtility>();
            _util.ClearWeight();
            _req = new ShipRequirementsUtility();
            _tex = new ShipTextureUtil();
            Changed = false;
        }

        void Update()
        {
            if (_ship != null)
            {
                _util.UpdateUI();

                _util.UpdateSavedMsg(Changed);

                foreach (BaseShipComponent component in _ship.Components)
                {
                    component.Tick();
                    
                    if (!_util.IsDragging)
                    {
                        component.ConfirmConnect();
                    }

                    if(component.ChangePending)
                        Changed = true;
                }

                // if Dragged obj is not the ships head or successfully connected it must be 
                // removed
                if(_util.DraggedObj != null && !_util.IsDragging)
                {
                    if(!_util.InBounds(ShipBoundsPanel.GetComponent<RectTransform>(), _util.DraggedObj.transform.position))
                    {
                        DeletePiece(_util.DraggedObj);
                    }
                    else if(!_util.DraggedObj.Equals(_ship.Head) && !_util.DraggedObj.Connected)
                    {
                        DeletePiece(_util.DraggedObj);
                    }

                    // now stop checking
                    _util.DraggedObj = null;
                }
            }
        }

        // Reset Data function and clear panel here
        public void ResetShipData()
        {
            // Each child should be a ship component
            foreach (Transform child in ShipConstructPanel)
            {
                if(child.name != "Head")
                    Destroy(child.gameObject);
            }

            _req.Clear(false);
            Inventory.AddMatsToList(_req.Requirements);

            _ship.Components.Clear();
            _ship.AddedIDs.Clear();
            _ship.Links.Clear();
            _ship.Head = null;
            _ship.Ship = null;
            Changed = false;

            _util.ClearWeight();
        }

        public void DeletePiece(BaseShipComponent bSC)
        {
            _ship.AddedIDs.Remove(bSC.ShipComponent.InstanceID);

            // Clear assign bSC Data
            bSC.ShipComponent.Direction = "up";
            bSC.ShipComponent.InstanceID = -1;
            bSC.ClearConnections();
            
            // remove component from list
            _ship.Components.Remove(bSC);
            
            // destroy object
            GameObject.Destroy(bSC.gameObject);

            _req.RemoveComponent(bSC.ShipComponent);
            Inventory.AddMatsToList(_req.Requirements);

            // nullify pointer 
            if(_util.DraggedObj.Equals(bSC))
                _util.DraggedObj = null;
            if (_ship.Head != null)
            {
                if (_ship.Head.Equals(bSC))
                    _ship.Head = null;
            }

            _util.UpdateWeight();
            Changed = true;
        }

        // BUILD EXISTING SHIP HERE
        /// <summary>
        /// Event listener
        /// loads the selected ship to the panel
        /// </summary>
        /// <param name="newShip">New ship.</param>
        public void LoadExistingShip(ShipData newShip)
        {
            if (_ship != null)
                ResetShipData();
            else
                _ship = new ShipContainer();
           
            _req.StoreExisting = true;
            _util.DraggedObj = null;
            _ship.Ship = newShip;
            _util.SetShip(_ship);

            // initialize head object 
            _ship.Head = PlaceComponentGO(newShip.Head);

            // Align ship head to be 3/4s up center of screen
            _ship.Head.GetComponent<RectTransform>().localPosition = new Vector3(
                0, 0, 0); 

            // Add the components that are connected to the head
            LoadConnectedObjects(_ship.Head, newShip.Head);

            LoadConnections();

            ShipName.text = newShip.Name;

            // finish adding existing
            _req.StoreExisting = false;
            Changed = false;

            _util.UpdateWeight();
        }

        /// <summary>
        /// Places the component GameObject to the panel.
        /// and put component in the list
        /// </summary>
        /// <returns>The component G.</returns>
        /// <param name="component">Component.</param>
        private BaseShipComponent PlaceComponentGO(Data.Shared.Component component)
        {
            // Create Ship Object and apply transform parent
            GameObject newObj = Instantiate(ShipCompPrefab);
            newObj.transform.SetParent(ShipConstructPanel);

            // Create BaseShipComponent and add it to our list
            BaseShipComponent bSC = newObj.GetComponent<BaseShipComponent>();
            bSC.InitComponent(component);
            _req.AddComponent(component);
            Inventory.AddMatsToList(_req.Requirements);
            _ship.Components.Add(bSC);

            // Add the ID to the existing list
            _ship.AddedIDs.Add(component.InstanceID);

            _util.UpdateWeight();

            return bSC;
        }

        /// <summary>
        /// Loads the connected objects to a component recursively
        /// </summary>
        /// <param name="component">Component.</param>
        private void LoadConnectedObjects(BaseShipComponent home, Data.Shared.Component component)
        {
            if (component.Sockets == null)
                return;

            foreach (Socket socket in component.GetSockets())
            {
                // find the second piece through the socket
                //print(string.Format("Component: {0} attemping to load component ID: {1}", component.Name, socket.OtherID));
                Data.Shared.Component piece = _ship.Ship.GetComponent (socket.OtherID);
                if(piece == null)
                {
                    Debug.Log("Ship Editor - " +
                              "LoadConnectedObjects: other " +
                              "socket not found!");
                    continue;
                }
                
                // test we haven't already added this piece.
                // stops unending loops
                if(_ship.AddedIDs.Contains(piece.InstanceID))
                    continue;

                // Place the component onto the editor
                BaseShipComponent bSC = PlaceComponentGO(piece);

                // Add Socket to our link list for loading
                _ship.Links.Add(socket, home);

                // Load connected objects of the new component
                LoadConnectedObjects(bSC, piece);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadConnections()
        {
            // Assign and connect socket
            foreach (Socket s in _ship.Links.Keys)
            {
                // find first object
                BaseShipComponent firstObj = _ship.Links[s];
                // find second object
                BaseShipComponent scndObj = null;
                
                foreach(BaseShipComponent bSC in _ship.Components)
                {
                    if(bSC.ShipComponent.InstanceID == s.OtherID)
                        scndObj = bSC;
                }
                
                if(scndObj != null)
                {
                    scndObj.AddConnection(s.OtherLinkID, firstObj, s.SocketID);
                }
            }
        }

        // CREATE NEW SHIP

        public void CreateNewShip()
        {
            if (_ship != null)
                ResetShipData();
            else
                _ship = new ShipContainer();

            _ship.Ship = new ShipData();
            _util.SetShip(_ship);
            _util.DraggedObj = null;

            ShipName.text = _util.ReturnNextAvailableName();
            _ship.Ship.CombatResponsive = true;
            Changed = false;

            _util.ClearWeight();
        }

        // CREATE A SHIP COMPONENT
        public void BuildComponent(Data.Shared.Component component)
        {
            if (_ship == null)
                CreateNewShip();
            // Assign component data
            // Comp ID
            component.InstanceID = _util.ReturnNextAvailableInstance();

            // Create Base Ship Component and Add to window
            BaseShipComponent bSC = PlaceComponentGO(component);

            if (_ship.Head == null && bSC.ShipComponent.Folder == "Components")
                _ship.Head = bSC;
            _util.DraggedObj = bSC;
            _util.IsDragging = true;
            Changed = true;
        }

        public void SaveShipData()
        {
            if (_ship == null)
                return;

            if (_ship.Head == null)
                return;

            _util.DraggedObj = null;
            Changed = false;

            // TEST THAT WE HAVE ENOUGH MATERIAL
            /*
            foreach (MaterialData mat in _req.Requirements.Keys)
            {
                if(SystemManager.GetPlayer.Cargo == null)
                    SystemManager.GetPlayer.Cargo = 
                        new System.Collections.Generic.Dictionary<MaterialData, float>();
                
                if(!SystemManager.GetPlayer.Cargo.ContainsKey(mat))
                    return;
                
                if(!(SystemManager.GetPlayer.Cargo[mat] >= _req.Requirements[mat]))
                    return;
            }
            
            // EXPEND MATERIAL IN CARGO
            foreach (MaterialData mat in _req.Requirements.Keys)
            {
                if(SystemManager.GetPlayer.Cargo == null)
                    SystemManager.GetPlayer.Cargo = 
                        new System.Collections.Generic.Dictionary<MaterialData, float>();

                SystemManager.GetPlayer.Cargo[mat] -=  _req.Requirements[mat];
            }*/
            /*
            // Clear requirements as the ship will be saved
            _req.Clear(true);

            // Change requirement bool to add existing parts as these parts are now current
            _req.StoreExisting = true;

            // create temporary shipdata we will save current components to
            ShipData temp = new ShipData();

            SaveCompSockets(_ship.Head, temp, new List<int>());

            // maybe test that name does not already exist?
            List<ShipData> exist = new List<ShipData>(ShipLibrary.GetAll());
            if (exist.FindIndex(x => x.Name == ShipName.text) != -1)        
                // Name already exists, change it
                temp.Name = _util.ReturnNextAvailableName();
            else
                temp.Name = ShipName.text;

            temp.CombatResponsive = _ship.Ship.CombatResponsive;

            /*int shipIndex = SystemManager.GetPlayer.ShipList.FindIndex(x => x==_ship.Ship);
            // Replace ship if currently stored in player base
            if (shipIndex != -1)
            {
                SystemManager.GetPlayer.ShipList [shipIndex] = _ship.Ship = temp;
            } else
            {
                // we have a new creation
                SystemManager.GetPlayer.AddShip(temp);
                _ship.Ship = temp;
            }

            SystemManager.GetPlayer.SetShip(temp.Name);

            _tex.SaveIcon(_ship.Components, temp);

            // Change requirement bool
            _req.StoreExisting = false;*//*
        }

        private void SaveCompSockets(BaseShipComponent bSC, ShipData ship, List<int> AddedIDs)
        {
            // Create ComponentData from bSC
            Data.Shared.Component tempComp;
            tempComp = bSC.ShipComponent;
            tempComp.Sockets = new List<Socket>();
            bSC.ChangePending = false;
            
            // Add socket behaviour based on added ship component
            foreach(SocketBehaviour socket in bSC.Sockets)
            {
                // add sockets that are not currently existing
                if(socket.connected)
                {
                    if(!AddedIDs.Contains(socket.connectedSocket.ObjectID))
                    {
                        tempComp.AddSocket(int.Parse(socket.SocketID), 
                                           int.Parse(socket.connectedSocket.SocketID),
                                           socket.connectedSocket.ObjectID);

                        AddedIDs.Add(bSC.ShipComponent.InstanceID);

                        _req.AddComponent(bSC.ShipComponent);
                        SaveCompSockets(socket.connectedSocket.container, ship, AddedIDs);
                    }
                }
            }

            ship.AddComponent(tempComp, bSC.Equals(_ship.Head));
        }

        public void ClearShip()
        {
            if (_ship == null)
                return;

            _tex.Delete(_ship.Ship.Name);

            /*if (SystemManager.GetPlayer.ShipList.Contains(_ship.Ship))
            {
                SystemManager.GetPlayer.ShipList.Remove(_ship.Ship);
            }
            if (SystemManager.GetPlayer.Ship.Equals(_ship.Ship))
            {
                SystemManager.GetPlayer.SetShip(0);
            }*//*
            ResetShipData();

            _util.ClearWeight();
        }*/
    }
}
