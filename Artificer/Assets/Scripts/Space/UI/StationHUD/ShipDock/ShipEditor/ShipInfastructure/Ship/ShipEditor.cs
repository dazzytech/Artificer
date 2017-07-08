using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;
using Data.Space.Library;
using UI.Effects;

namespace Space.UI.Station.Editor
{
    [RequireComponent(typeof(EditorUI))]
    /// <summary>
    /// Ship editor controller.
    /// script that controls and manages all the editor window
    /// </summary>
    public class ShipEditor : MonoBehaviour
    {
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

        #region HUD ELEMENTS

        [Header("UI Elements")]

        /// <summary>
        /// The panel that the ship is built on
        /// </summary>
        [SerializeField]
        private Transform m_shipConstructPanel;

        /// <summary>
        /// The transform that limits the movement
        /// of the ship construct panel
        /// </summary>
        [SerializeField]
        private Transform m_shipBoundsPanel;

        /// <summary>
        /// Prefab interactable object for base component
        /// object
        /// </summary>
        [SerializeField]
        private GameObject m_componentPrefab;

        /// <summary>
        /// Text field for editing the ship name
        /// </summary>
        [SerializeField]
        private InputField m_shipName;

        #endregion

        #endregion

        #region MONO BEHAVIOUR

        // MonoBehavours
        void Awake()
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
                        if (!RectTransformExtension.InBounds
                            (m_shipBoundsPanel.GetComponent<RectTransform>(), 
                            DraggedObj.transform.position))
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

        // Reset Data function and clear panel here
        /*public void ResetShipData()
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
 */

        #region PUBLIC INTERACTION
            
        /// <summary>
        /// Event listener
        /// loads the selected ship to the panel
        /// </summary>
        /// <param name="newShip">New ship.</param>
        public void LoadExistingShip(ShipData newShip)
        {
            //if (_ship != null)
                //ResetShipData();
            //else
                Ship = new ShipContainer();
           
            //_req.StoreExisting = true;
            DraggedObj = null;
            Ship.Ship = newShip;
            m_UI.Initialize(this);

            // initialize head object 
            Ship.Head = PlaceComponentGO(newShip.Head);

            // Align ship head to be 3/4s up center of screen
            Ship.Head.GetComponent<RectTransform>().localPosition = new Vector3(
                0, 0, 0); 

            // Add the components that are connected to the head
            LoadConnectedObjects(Ship.Head, newShip.Head);

            LoadConnections();

            m_shipName.text = newShip.Name;

            // finish adding existing
            //_req.StoreExisting = false;
            Changed = false;

            //_util.UpdateWeight();
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Places the component GameObject to the panel.
        /// and put component in the list
        /// </summary>
        /// <returns>The component G.</returns>
        /// <param name="component">Component.</param>
        private BaseComponent PlaceComponentGO(Data.Shared.Component component)
        {
            // Create Ship Object and apply transform parent
            GameObject newObj = Instantiate(m_componentPrefab);
            newObj.transform.SetParent(m_shipConstructPanel);

            // Create BaseShipComponent and add it to our list
            BaseComponent BC = newObj.GetComponent<BaseComponent>();
            BC.InitComponent(component);
            BC.OnMouseDown += StartDrag;
            BC.OnMouseOver += MouseEnter;
            BC.OnMouseOut += MouseLeave;
            //_req.AddComponent(component);
            //Inventory.AddMatsToList(_req.Requirements);
            Ship.Components.Add(BC);

            // Add the ID to the existing list
            Ship.AddedIDs.Add(component.InstanceID);

            //_util.UpdateWeight();

            return BC;
        }

        /// <summary>
        /// Loads the connected objects to a component recursively
        /// </summary>
        /// <param name="component">Component.</param>
        private void LoadConnectedObjects(BaseComponent home, Data.Shared.Component component)
        {
            if (component.sockets == null)
                return;

            foreach (Socket socket in component.sockets)
            {
                // find the second piece through the socket
                //print(string.Format("Component: {0} attemping to load component ID: {1}", component.Name, socket.OtherID));
                Data.Shared.Component piece = Ship.Ship.GetComponent (socket.OtherID);
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
                BaseComponent BC = PlaceComponentGO(piece);

                // Add Socket to our link list for loading
                Ship.Links.Add(socket, home);

                // Load connected objects of the new component
                LoadConnectedObjects(BC, piece);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadConnections()
        {
            // Assign and connect socket
            foreach (Socket s in Ship.Links.Keys)
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

            BC.OnMouseDown -= StartDrag;
            BC.OnMouseOver -= MouseEnter;
            BC.OnMouseOut -= MouseLeave;

            //_util.UpdateWeight();
            Changed = true;
        }

        #endregion

        #region EVENTS

        public void StartDrag(BaseComponent comp)
        {
            DraggedObj = comp;
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

        if (SystemManager.GetPlayer.ShipList.Contains(_ship.Ship))
        {
            SystemManager.GetPlayer.ShipList.Remove(_ship.Ship);
        }
        if (SystemManager.GetPlayer.Ship.Equals(_ship.Ship))
        {
            SystemManager.GetPlayer.SetShip(0);
        }
        ResetShipData();

        _util.ClearWeight();
    }*/
    }
}
