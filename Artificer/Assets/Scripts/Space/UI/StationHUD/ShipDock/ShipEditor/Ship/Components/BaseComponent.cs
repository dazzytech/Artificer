using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Space;
using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;
using System;
using UI.Effects;
using Space.UI.Station.Editor.Socket;

namespace Space.UI.Station.Editor.Component
{
    public enum WeaponType {PRIMARY, SECONDARY, TERTIARY};

    // UI Component that shows draggable ship image
    public class BaseComponent : MonoBehaviour, 
        IPointerEnterHandler, IPointerExitHandler,
        IDragHandler
    {
        #region EVENT

        public delegate void MouseEvent(BaseComponent Reference);

        public event MouseEvent OnMouseOver;

        public event MouseEvent OnMouseOut;

        public event MouseEvent OnMouseDown;

        public event MouseEvent OnMouseUp;

        public event MouseEvent OnDragComponent;

        #endregion

        #region ATTRIBUTES

        #region COMPONENT

        /// <summary>
        /// Reference to the component data
        /// </summary>
        [HideInInspector]
        public ComponentData ShipComponent;

        /// <summary>
        /// Reference to the Resource
        /// Component GameOBject
        /// </summary>
        public GameObject GO;

        /// <summary>
        /// Direction that the component
        /// is facing
        /// </summary>
        [HideInInspector]
        public string direction;

        /// <summary>
        /// Primary, secondary or tertiary 
        /// trigger
        /// </summary>
        public WeaponType WType;

        /// <summary>
        /// detects if selected but not 
        /// dragged
        /// </summary>
        private bool m_selected = true;

        /// <summary>
        /// If our component is currently
        /// being dragged
        /// </summary>
        private bool m_isDragging = false;

        /// <summary>
        /// Detect if the component is over 
        /// our component
        /// </summary>
        private bool m_mouseOver = false;

        #endregion

        #region HUD ELEMENTS

        /// <summary>
        /// Image that appears 
        /// on the component item
        /// </summary>
        [SerializeField]
        private RawImage m_componentImage;

        #endregion

        #region SOCKETS

        /// <summary>
        /// List of all sockets
        /// </summary>
        [HideInInspector]
        public List<SocketBehaviour> Sockets;

        /// <summary>
        /// Manages the connections of the component
        /// </summary>
        ConnectionBehaviour Connect;

        #endregion

        #endregion

        #region ACCESSORS

        /// <summary>
        /// Returns whether or not an object has been
        /// successfully connected
        /// </summary>
        public bool Connected
        {
            get
            {
                bool retVal = false;
                foreach (SocketBehaviour socket in Sockets)
                {
                    if (socket.connected)
                    {
                        retVal = true;
                        break;
                    }
                }

                return retVal;
            }

        }

        /// <summary>
        /// Returns whether or not an object is pending
        /// </summary>
        public bool Pending
        {
            get
            {
                bool retVal = false;
                foreach (SocketBehaviour socket in Sockets)
                {
                    if (socket.connectedSocket != null)
                    {
                        retVal = true;
                        break;
                    }
                }

                return retVal;
            }

        }

        /// <summary>
        /// convert comp texture to texture2D
        /// </summary>
        public Texture2D GetTex
        {
            get { return m_componentImage.texture as Texture2D; }
        }

        /// <summary>
        /// Returns the bounding box of the 
        /// component from a center origin
        /// </summary>
        public Rect Bounds
        {
            get
            {
                float halfWidth = GetComponent<RectTransform>().rect.width / 2;
                float halfHeight = GetComponent<RectTransform>().rect.height / 2;
                return new Rect(transform.localPosition.x - halfWidth, transform.localPosition.y - halfHeight,
                    GetComponent<RectTransform>().rect.width, GetComponent<RectTransform>().rect.height);
            }
        }

        /// <summary>
        /// Returns if there are any sockets pending
        /// a connection
        /// </summary>
        public bool ChangePending
        {
            get { return Connect.ChangedSockets; }
            set
            {
                if (Connect == null)
                    return;
                Connect.ChangedSockets = value;
            }
        }

        /// <summary>
        /// style of the texture
        /// for the built component
        /// </summary>
        public string Style
        {
            get
            {
                return ShipComponent.Style;
            }
            set
            {
                ShipComponent.Style = value;
                Sprite sprite = null;
                // swap out texture
                foreach (StyleInfo info in
                        GO.GetComponent<ComponentAttributes>().componentStyles)
                {
                    if (info.name == value)
                        sprite = info.sprite;
                }

                if (sprite == null)
                {
                    // There is an error here
                    m_componentImage.texture = null;
                    return;
                }

                m_componentImage.texture = sprite.texture;
                GetComponent<RectTransform>().sizeDelta
                    = new Vector2(sprite.texture.width, sprite.texture.height);
                ChangePending = true;
            }
        }

        public bool Dragging
        {
            get { return m_isDragging; }
            set { m_isDragging = value; }
        }

        #endregion

        #region MONO BEHAVIOUR

        private void Update()
        {
            if (m_isDragging)
            {
                if (!Input.GetMouseButton(0))
                {
                    if (OnMouseUp != null)
                        OnMouseUp(this);

                    m_isDragging = false;

                    // Change our colour to a highlight
                    m_componentImage.color = new Color(.75f, .75f, .2f, .4f);

                    return;
                }

                UpdatePending();

                UpdateDirection();

                UpdatePosition();
            }
            else if (m_selected)
            {
                if (ShipEditor.SelectedObj == this)
                    m_componentImage.color = new Color(.2f, .1f, .867f, .8f);
                else
                    m_selected = false;
            }
            else
            {
                // Reset rotation for selection fix
                RectTransform self = GetComponent<RectTransform>();
                Quaternion orig = self.rotation;
                self.rotation = Quaternion.Euler(0, 0, 0);

                // Proceed with selection testing
                if (RectTransformExtension.InBounds
                   (self, Input.mousePosition))
                {
                    if (m_mouseOver && ShipEditor.HighlightedObj == this)
                    {
                        if (Input.GetMouseButton(0) && ShipEditor.DraggedObj == null)
                        {
                            if (OnMouseDown != null)
                                OnMouseDown(this);

                            m_selected = true;
                        }
                    }
                }
                else
                {
                    if (m_mouseOver)
                    {
                        if (OnMouseOut != null)
                            OnMouseOut(this);

                        m_mouseOver = false;

                        m_componentImage.color = new Color(1, 1, 1, 1);
                    }
                }

                // Revert to original rotation
                self.rotation = orig;
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Initialize the component using the provided data
        /// </summary>
        /// <param name="param"></param>
        public void InitComponent(ComponentData param)
        {
            // Set data
            ComponentData temp = new ComponentData();
            temp.Folder = param.Folder;
            temp.Direction = param.Direction;
            temp.Name = param.Name;
            //temp.Sockets = param.Sockets;
            temp.InstanceID = param.InstanceID;
            temp.AutoLock = param.AutoLock;
            temp.AutoFire = param.AutoFire;
            temp.behaviour = param.behaviour;
            ShipComponent = temp;
            direction = ShipComponent.Direction;

            // assign default
            WType = WeaponType.PRIMARY;
            // check to see if existing key
            WType = param.Trigger == "fire" || param.CTrigger == "fire"? WeaponType.PRIMARY:
                    param.Trigger == "secondary" || param.CTrigger == "secondary"? WeaponType.SECONDARY:
                    param.Trigger == "tertiary" || param.CTrigger == "tertiary"? WeaponType.TERTIARY: WType;


            // Obtain image using Data
            GO = Resources.Load(
                string.Format("Space/Ships/{0}/{1}",ShipComponent.Folder, ShipComponent.Name),
                    typeof(GameObject)) as GameObject;

            Style = param.Style;

            // Import the sockets from gameobject
            Sockets = new List<SocketBehaviour>();
            AssignSockets();
            Connect = new ConnectionBehaviour();
            Connect.Init(Sockets);

            ApplyDirection();

            AssignKey();

            transform.localScale = new Vector3(1, 1, 1);
            ChangePending = false;
        }

        /// <summary>
        /// used to snap a component 
        /// into a position
        /// </summary>
        /// <param name="newPos"></param>
        public void SnapMove(Vector3 newPos)
        {
            gameObject.GetComponent<RectTransform>().position += newPos;
        }

        /// <summary>
        /// Adds the collided object to the connection system.
        /// </summary>
        /// <param name="collider">Collider.</param>
        public void AddCollision(BaseComponent collider)
        {
            if (!Connect.collidedObjects.Contains(collider))
                Connect.collidedObjects.Add(collider);
        }

        /// <summary>
        /// Auto-assigns key depending 
        /// on component type
        /// </summary>
        public void AssignKey()
        {
            switch (ShipComponent.Folder)
            {
                case "Rotors":
                    switch (direction)
                    {
                        case "left":
                            ShipComponent.Trigger = "turnRight";
                            ShipComponent.CTrigger = "turnRight";
                            break;
                        case "right":
                            ShipComponent.Trigger = "turnLeft";
                            ShipComponent.CTrigger = "turnRight";
                            break;
                        default:
                            break;
                    }
                    break;
                case "Engines":
                    switch (direction)
                    {
                        case "up":
                            ShipComponent.Trigger = "moveUp";
                            ShipComponent.CTrigger = "moveUp";
                            break;
                        case "down":
                            ShipComponent.Trigger = "moveDown";
                            ShipComponent.CTrigger = "moveDown";
                            break;
                        case "left":
                            ShipComponent.Trigger = "strafeRight";
                            ShipComponent.CTrigger = "strafeRight";
                            break;
                        case "right":
                            ShipComponent.Trigger = "strafeLeft";
                            ShipComponent.CTrigger = "strafeLeft";
                            break;
                    }
                    break;
                case "Weapons":
                    switch (WType)
                    {
                        case WeaponType.PRIMARY:
                            ShipComponent.Trigger = "fire";
                            ShipComponent.CTrigger = "fire";
                            break;
                        case WeaponType.SECONDARY:
                            ShipComponent.Trigger = "secondary";
                            ShipComponent.CTrigger = "secondary";
                            break;
                        case WeaponType.TERTIARY:
                            ShipComponent.Trigger = "tertiary";
                            ShipComponent.CTrigger = "tertiary";
                            break;
                    }
                    break;
                case "Launchers":
                    switch (WType)
                    {
                        case WeaponType.PRIMARY:
                            ShipComponent.Trigger = "fire";
                            ShipComponent.CTrigger = "fire";
                            break;
                        case WeaponType.SECONDARY:
                            ShipComponent.Trigger = "secondary";
                            ShipComponent.CTrigger = "secondary";
                            break;
                        case WeaponType.TERTIARY:
                            ShipComponent.Trigger = "tertiary";
                            ShipComponent.CTrigger = "tertiary";
                            break;
                    }
                    break;
                case "Warps":
                    ShipComponent.Trigger = "use";
                    break;
                case "Wells":
                    ShipComponent.Trigger = "well";
                    break;
                case "Shields":
                    ShipComponent.Trigger = "Activate Shield";
                    ShipComponent.CTrigger = "Activate Shield";
                    break;
            }
            ChangePending = true;
        }

        #region SOCKETS

        /// <summary>
        /// Adds a reference to the connected component
        /// and informs the other comp of our comp
        /// </summary>
        /// <param name="sockID"></param>
        /// <param name="other"></param>
        /// <param name="otherSockID"></param>
        public void AddConnection(int sockID, BaseComponent other, int otherSockID)
        {
            if (Connect == null)
                Connect = new ConnectionBehaviour();

            foreach (SocketBehaviour s in other.Sockets)
            {
                int sID = int.Parse(s.SocketID);
                if (sID == otherSockID)
                {
                    Connect.ConnectToPiece(sockID, s);
                    break;
                }
            }
        }

        /// <summary>
        /// Removes all connections
        /// </summary>
        public void ClearConnections()
        {
            if (Connect == null)
                return;

            foreach (SocketBehaviour s in Sockets)
            {
                s.Wipe();
            }
        }

        /// <summary>
        /// Confirms and pending sockets
        /// to connect components
        /// </summary>
        public virtual void ConfirmConnect()
        {
            if (this != null)
            {
                Connect.Confirm();
            }
        }

        /// <summary>
        /// Each tick snaps the sockets together
        /// and finds pending sockets
        /// </summary>
        public void Tick()
        {
            Connect.Tick();
            Connect.LateTick();

            foreach (SocketBehaviour s in Sockets)
                s.Tick();

            if (!m_isDragging)
            {
                ConfirmConnect();
            }
        }

        #endregion

        #endregion

        #region PRIVATE UTILITIES

        #region INTERACTIVE

        /// <summary>
        /// Changes the colour 
        /// depending on if any sockets are pending
        /// </summary>
        private void UpdatePending()
        {
            if (Pending)
            {
                // Turn the RawImage to a faded green
                m_componentImage.color = new Color(.2f, 1, .2f, .4f);
            }
            else
            {
                // Turn the RawImage to a faded red
                m_componentImage.color = new Color(1, .2f, .2f, .4f);
            }
        }

        /// <summary>
        /// If dragging then allow the 
        /// component to be rotated with arrowkeys
        /// </summary>
        private void UpdateDirection()
        {
                HintBoxController.Display("When dragging an object - use the directional keys " +
                    "to change the direction the object is facing");

                if (Input.GetKeyUp(KeyCode.UpArrow))
                    AssignDirection("up");
                if (Input.GetKeyUp(KeyCode.DownArrow))
                    AssignDirection("down");
                if (Input.GetKeyUp(KeyCode.LeftArrow))
                    AssignDirection("left");
                if (Input.GetKeyUp(KeyCode.RightArrow))
                    AssignDirection("right");
        }

        /// <summary>
        /// Clips the posittion to the mouse
        /// </summary>
        private void UpdatePosition()
        {
            transform.position = Input.mousePosition;
        }

        #endregion

        /// <summary>
        /// Assigns the sockets.
        /// using either socketdata
        /// from the objects 
        /// or uses their positional data
        /// </summary>
        private void AssignSockets()
        {
            Sockets.Clear();

            // Remove all socket objects that exist currently
            foreach (Transform child in this.transform)
            {
                if(child.name.Equals("Socket"))
                    Destroy(child.gameObject);
            }
            
            foreach (Transform t in GO.transform)
            {
                if (t.name.Contains("socket_"))
                {
                    GameObject newSock = new GameObject();
                    newSock.name="Socket";
                    newSock.transform.SetParent(this.transform);
                    newSock.AddComponent<RectTransform>().localPosition 
                        = new Vector3(t.transform.position.x *100f, t.transform.position.y *100f);
                    newSock.transform.localScale = new Vector2(1, 1);
                    SocketBehaviour socket = newSock.AddComponent<SocketBehaviour>();
                    socket.Init(this);

                    // If socketdata exists then use that otherwise 
                    // calc direction
                    Data.UI.SocketData sock = t.GetComponent<Data.UI.SocketData>();
                    if(sock != null)
                    {
                        SocketAttributes.Alignment up = SocketAttributes.Alignment.UP;
                        SocketAttributes.Alignment down = SocketAttributes.Alignment.DOWN;
                        SocketAttributes.Alignment left = SocketAttributes.Alignment.LEFT;
                        SocketAttributes.Alignment right = SocketAttributes.Alignment.RIGHT;
                        
                        switch(direction)
                        {
                            case "up":
                                // up
                                break;
                            case "down":
                                up = SocketAttributes.Alignment.DOWN;
                                down = SocketAttributes.Alignment.UP;
                                left = SocketAttributes.Alignment.RIGHT;
                                right = SocketAttributes.Alignment.LEFT;
                                // down
                                break;
                            case "left":
                                up = SocketAttributes.Alignment.LEFT;
                                down = SocketAttributes.Alignment.RIGHT;
                                left = SocketAttributes.Alignment.DOWN;
                                right = SocketAttributes.Alignment.UP;
                                // left
                                break;
                            case "right":
                                up = SocketAttributes.Alignment.RIGHT;
                                down = SocketAttributes.Alignment.LEFT;
                                left = SocketAttributes.Alignment.UP;
                                right = SocketAttributes.Alignment.DOWN;
                                // right
                                break;
                        }
                        switch(sock.Direction)
                        {
                            case "up":
                                socket.alignment = up;
                                break;
                            case "down":
                                socket.alignment = down;
                                break;
                            case "left":
                                socket.alignment = left;
                                break;
                            case "right":
                                socket.alignment = right;
                                break;
                        }
                    }
                    else
                    {
                        Vector2 dir = t.transform.localPosition;

                        if (Mathf.Abs (dir.y) > Mathf.Abs (dir.x)) 
                            // Collision on Y Axis
                            // been set to -1
                            if(Mathf.Sign (dir.y) == 1)                   // Object has a collision on top 
                                socket.alignment = SocketAttributes.Alignment.UP;
                        else                                          // object has a collision on bottom 
                            socket.alignment = SocketAttributes.Alignment.DOWN;
                        else 
                            // Collision on X Axis
                            if(Mathf.Sign (dir.x) == 1)                   // object has a collision on right    
                                socket.alignment = SocketAttributes.Alignment.RIGHT;
                        else                                                // object has a collision on left 
                            socket.alignment = SocketAttributes.Alignment.LEFT;
                    }

                    string[] spl = t.name.Split('_');
                    socket.SocketID = spl[1];
                    Sockets.Add(socket);
                }
            }
        }

        /// <summary>
        /// Resets the sockets and rotation
        /// to face the new direction
        /// </summary>
        /// <param name="dir"></param>
        private void AssignDirection(string dir)
        {
            ClearConnections();
            direction = dir;
            ApplyDirection();
            AssignSockets();
            AssignKey();
            ShipComponent.Direction = direction;
        }

        /// <summary>
        /// Rotate the element depending on 
        /// the direction
        /// </summary>
        private void ApplyDirection()
        {
            GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);

            switch (direction)
            {
                case "up":
                    GetComponent<RectTransform>().Rotate(new Vector3(0,0,0f));
                    break;
                case "down":
                    GetComponent<RectTransform>().Rotate(new Vector3(0,0,180f));
                    break;
                case "left":
                    GetComponent<RectTransform>().Rotate(new Vector3(0, 0, 90f));
                    break;
                case "right":
                    GetComponent<RectTransform>().Rotate(new Vector3(0, 0, 270f));
                    break;
            }
        }

        #endregion

        #region IPOINTEREVENT

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (OnMouseOver != null)
                OnMouseOver(this);

            m_mouseOver = true;

            // Change our colour to a highlight
            if(ShipEditor.DraggedObj == null)
                m_componentImage.color = new Color(.75f, .75f, .2f, .4f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (ShipEditor.DraggedObj == null &&
                eventData.button == PointerEventData.InputButton.Left)
            {
                if (OnDragComponent != null)
                    OnDragComponent(this);

                m_isDragging = true;
            }
        }

        #endregion
    }
}

