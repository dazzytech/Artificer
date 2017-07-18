using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Space;
using Data.Space.Library;
using UI.Effects;
using Space.UI.Station.Editor.Component;

namespace Space.UI.Station.Editor
{
    [RequireComponent(typeof(EditorUI))]
    /// <summary>
    /// Manages the data side of the ship
    /// editing. Sends messages to ui editor
    /// </summary>
    public class ShipEditor : MonoBehaviour
    {
        #region DELEGATES

        public delegate void DelegateHead(BaseComponent newHead);

        #endregion

        #region ATTRIBUTES

        #region SHIP VARIABLES

        /// <summary>
        /// Container for the ship we're editing
        /// </summary>
        public ShipContainer Ship;

        /// <summary>
        /// Whether or not the ship has been changed
        /// </summary>
        private bool Changed;

        [HideInInspector]
        public static BaseComponent DraggedObj;

        [HideInInspector]
        public static BaseComponent HighlightedObj;

        [HideInInspector]
        public static BaseComponent SelectedObj;

        #endregion

        #region REFERENCES

        /// <summary>
        /// UI utilities for the ship editor
        /// </summary>
        [SerializeField]
        private EditorUI m_UI;

        //private ShipTextureUtil _tex;

        //private ShipRequirementsUtility _req;

        //public RequirementInventory Inventory;

        #endregion

        #endregion

        #region MONO BEHAVIOUR

        private void Awake()
        {
            //_util.ClearWeight();
            //_req = new ShipRequirementsUtility();
            //_tex = new ShipTextureUtil();
            Changed = false;
        }

        void Update()
        {
            if (Ship != null)
            {
                m_UI.UpdateUI();

                //_util.UpdateSavedMsg(Changed);
                for ( int i = 0; i < Ship.Components.Count; i++)
                {
                    BaseComponent component = Ship.Components[i];
                    if (component.Dragging)
                    {
                        // Grabs all other components for testing overlapping and connecting
                        foreach (BaseComponent other in Ship.Components)
                        {
                            // First detect collision with head
                            if (!component.Equals(other))
                            {
                                if (component.GetComponent<RectTransform>().rect.Overlaps
                                    (other.GetComponent<RectTransform>().rect))
                                {
                                    component.AddCollision(other);
                                }
                            }
                        }
                    }

                    component.Tick();

                    if(DraggedObj == component && !component.Dragging)
                    {
                        if (!m_UI.IsWithinBounds(DraggedObj))
                        {
                            DeletePiece(DraggedObj);
                            i--;
                        }
                        else if (!DraggedObj.Equals(Ship.Head) && !DraggedObj.Connected)
                        {
                            DeletePiece(DraggedObj);
                            i--;
                        }

                        // now stop checking
                        DraggedObj = null;
                    }
                    
                    if(component.ChangePending)
                        Changed = true;
                }
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Called from external source when
        /// we want to clear the component list
        /// </summary>
        public void ClearShip()
        {
            if (Ship == null)
                return;

            //_tex.Delete(_ship.Ship.Name);

            ResetShip();

            //_util.ClearWeight();
        }

        /// <summary>
        /// Delegate function: sets the head of 
        /// ship
        /// </summary>
        /// <param name="newHead"></param>
        public void SetHead(BaseComponent newHead)
        {
            Ship.Head = newHead;
        }

        /// <summary>
        /// Event listener
        /// loads the selected ship to the panel
        /// </summary>
        /// <param name="newShip">New ship.</param>
        public void LoadExistingShip(ShipData newShip)
        {
            if (Ship != null)
                ResetShip();
            else
                Ship = new ShipContainer();
           
            //_req.StoreExisting = true;
            DraggedObj = null;
            Ship.Ship = newShip;
            m_UI.Initialize(this, newShip.Name);

            // initialize head object 
            Ship.Head = CreateComponent(newShip.Head);

            // Align ship head to be 3/4s up center of screen
            Ship.Head.GetComponent<RectTransform>().localPosition = new Vector3(
                0, 0, 0); 

            // Add the components that are connected to the head
            LoadConnectedObjects(Ship.Head, newShip.Head);

            LoadConnections();

            // finish adding existing
            //_req.StoreExisting = false;
            Changed = false;

            //_util.UpdateWeight();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        public void BuildComponent(ComponentData component)
        {
            //if (Ship == null)
                //CreateNewShip();
            // Assign component data
            // Comp ID
            component.InstanceID = ReturnNextAvailableInstance();

            // Create Base Ship Component and Add to window
            BaseComponent BC = CreateComponent(component);

            if (Ship.Head == null && BC.ShipComponent.Folder == "Components")
                Ship.Head = BC;

            DraggedObj = BC;
            DraggedObj.Dragging = true;

            Changed = true;
        }

        #endregion

        #region PRIVATE UTILITIES

        #region COMPONENTS

        /// <summary>
        /// Places the component GameObject to the panel.
        /// and put component in the list
        /// </summary>
        /// <returns>The component G.</returns>
        /// <param name="component">Component.</param>
        private BaseComponent CreateComponent(ComponentData component)
        {
            // Create BaseShipComponent 
            // using UI
            BaseComponent BC = m_UI.PlaceComponentGO(component);
            // assign events
            BC.OnDragComponent += StartDrag;
            BC.OnMouseDown += MouseDown;
            BC.OnMouseOver += MouseEnter;
            BC.OnMouseOut += MouseLeave;

            //_req.AddComponent(component);
            //Inventory.AddMatsToList(_req.Requirements);
            Ship.Components.Add(BC);

            // Add the ID to the existing list
            Ship.AddedIDs.Add(component.InstanceID);

            return BC;
        }

        /// <summary>
        /// Loads the connected objects to a component recursively
        /// </summary>
        /// <param name="component">Component.</param>
        private void LoadConnectedObjects(BaseComponent home, ComponentData component)
        {
            if (component.sockets == null)
                return;

            foreach (SocketData socket in component.sockets)
            {
                // find the second piece through the socket
                //print(string.Format("Component: {0} attemping to load component ID: {1}", component.Name, socket.OtherID));
                ComponentData piece = Ship.Ship.GetComponent (socket.OtherID);
                if(piece.Path == "")
                {
                    Debug.Log("Ship Editor - " +
                              "LoadConnectedObjects: other " +
                              "socket not found!");
                    continue;
                }
                
                // test we haven't already added this piece.
                // stops unending loops
                if(Ship.AddedIDs.Contains(piece.InstanceID))
                    continue;

                // Place the component onto the editor
                BaseComponent BC = CreateComponent(piece);

                // Add Socket to our link list for loading
                Ship.Links.Add(socket, home);

                // Load connected objects of the new component
                LoadConnectedObjects(BC, piece);
            }
        }

        private void DeletePiece(BaseComponent BC)
        {
            Ship.AddedIDs.Remove(BC.ShipComponent.InstanceID);

            // Clear assign bSC Data
            BC.ShipComponent.Direction = "up";
            BC.ShipComponent.InstanceID = -1;
            BC.ClearConnections();

            // remove component from list
            Ship.Components.Remove(BC);

            // destroy object
            GameObject.Destroy(BC.gameObject);

            //_req.RemoveComponent(BC.ShipComponent);
            //Inventory.AddMatsToList(_req.Requirements);

            // nullify pointer 
            if (Ship.Head != null)
            {
                if (Ship.Head.Equals(BC))
                    Ship.Head = null;
            }

            BC.OnDragComponent -= StartDrag;
            BC.OnMouseDown -= MouseDown;
            BC.OnMouseOver -= MouseEnter;
            BC.OnMouseOut -= MouseLeave;

            //_util.UpdateWeight();
            Changed = true;
        }

        #endregion

        #region SHIP

        /// <summary>
        /// 
        /// </summary>
        private void LoadConnections()
        {
            // Assign and connect socket
            foreach (SocketData s in Ship.Links.Keys)
            {
                // find first object
                BaseComponent firstObj = Ship.Links[s];
                // find second object
                BaseComponent scndObj = null;
                
                foreach(BaseComponent BC in Ship.Components)
                {
                    if(BC.ShipComponent.InstanceID == s.OtherID)
                        scndObj = BC;
                }
                
                if(scndObj != null)
                {
                    scndObj.AddConnection(s.OtherLinkID, firstObj, s.SocketID);
                }
            }
        }

        /// <summary>
        /// Delete component list and 
        /// resets the container
        /// </summary>
        private void ResetShip()
        {
            // Each child should be a ship component
            while (Ship.Components.Count > 0)
            {
                DeletePiece(Ship.Components[0]);
            }

            m_UI.ClearUI();

            //_req.Clear(false);
            //Inventory.AddMatsToList(_req.Requirements);

            Ship = new ShipContainer();

            //_util.ClearWeight();
        }

        #endregion

        #region CREATION

        // CREATE NEW SHIP
        /*
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
        }*/

        private int ReturnNextAvailableInstance()
        {
            int ID = 0;
            // Sort IDs numerically
            Ship.AddedIDs.Sort();
            // compare ID to each ID end result showed be the next available ID
            foreach (int otherID in Ship.AddedIDs)
            {
                if (otherID == ID)
                    ID = otherID + 1;
            }

            return ID;
        }

        #endregion

        #endregion

        #region EVENTS

        public void StartDrag(BaseComponent comp)
        {
            DraggedObj = SelectedObj = comp;
        }

        public void MouseDown(BaseComponent comp)
        {
            SelectedObj = comp;
        }

        public void MouseEnter(BaseComponent comp)
        {
            HighlightedObj = comp;
        }

        public void MouseLeave(BaseComponent comp)
        {
            if (HighlightedObj == comp)
                HighlightedObj = null;
        }

        #endregion

        // CREATE A SHIP COMPONENT
        /*
        
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
            }
        
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
        _req.StoreExisting = false;
    }

    private void SaveCompSockets(BaseShipComponent bSC, ShipData ship, List<int> AddedIDs)
    {
        // Create ComponentData from bSC
        Component tempComp;
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
    }*/

    
    }
}
