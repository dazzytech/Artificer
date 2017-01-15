using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;
using Data.Space;
using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;

namespace Construction.ShipEditor
{
    public enum WeaponType {PRIMARY, SECONDARY, TERTIARY};

    // UI Component that shows draggable ship image
    public class BaseShipComponent : MonoBehaviour
    {
        // reference to component data
        [HideInInspector]
        public Data.Shared.Component ShipComponent;

        // reference to GO
        public GameObject GO;

        // Store each socket
        [HideInInspector]
        public List<SocketBehaviour> Sockets;

        [HideInInspector]
        // Direction the component is facing
        public string direction;

        // Manages the connections of the component
        ConnectionBehaviour Connect;

        // UI ELEMENTS
        public RawImage CompImage;

        public WeaponType WType;

        // Initialize the component using the provided data
        public void InitComponent(Data.Shared.Component param)
        {
            // Set data
            Data.Shared.Component temp = new Data.Shared.Component();
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

            transform.localScale = new Vector2(1, 1);
            ChangePending = false;
        }

        /// <summary>
        /// Assigns the sockets.
        /// using either socketdata
        /// from the objects 
        /// or uses their positional data
        /// 
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
                    SocketData sock = t.GetComponent<SocketData>();
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
                    GetComponent<RectTransform>().Rotate(new Vector3(0,0,90f));
                    break;
                case "right":
                    GetComponent<RectTransform>().Rotate(new Vector3(0,0,270f));
                    break;
            }
        }

        public void AssignKey()
        {
            switch (ShipComponent.Folder)
            {
                case "Rotors":
                    switch(direction)
                    {
                        case "left":
                            ShipComponent.Trigger = "turnRight";
                            break;
                        case "right":
                            ShipComponent.Trigger = "turnLeft";
                            break;
                        default:
                            break;
                    }
                    break;
                case "Engines":
                    switch(direction)
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
                    switch(WType)
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
                    switch(WType)
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

        // create an assign texture
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
                foreach(StyleInfo info in
                        GO.GetComponent<ComponentAttributes>().componentStyles)
                {
                    if(info.name == value)
                        sprite = info.sprite;
                }

                if(sprite == null)
                {
                    // There is an error here
                    CompImage.texture = null;
                    return;
                }

                CompImage.texture = sprite.texture;
                GetComponent<RectTransform>().sizeDelta 
                    = new Vector2(sprite.texture.width, sprite.texture.height);
                ChangePending = true;
            }
        }


        // Event functions

        public void ProcessDirection(string dir)
        {
            ClearConnections();
            direction = dir;
            ApplyDirection();
            AssignSockets();
            AssignKey();
            ShipComponent.Direction = direction;
        }
    	
        /// <summary>
        /// Adds the collided object to the connection system.
        /// </summary>
        /// <param name="collider">Collider.</param>
        public void AddCollision(BaseShipComponent collider)
        {
            if (!Connect.collidedObjects.Contains(collider))
                Connect.collidedObjects.Add(collider);
        }

        public void AddConnection(int sockID, BaseShipComponent other, int otherSockID)
        {
            if (Connect == null)
                Connect = new ConnectionBehaviour();
            
            foreach (SocketBehaviour s in other.Sockets)
            {
                int sID = int.Parse(s.SocketID);
                if(sID == otherSockID)
                {
                    Connect.ConnectToPiece(sockID, s);
                    break;
                }
            }
        }

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
        /// Returns whether or not an object has been
        /// successfully connected
        /// </summary>
        public bool Connected
        {
            get
            {
                bool retVal = false;
                foreach(SocketBehaviour socket in Sockets)
                {
                    if(socket.connected)
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
                foreach(SocketBehaviour socket in Sockets)
                {
                    if(socket.connectedSocket != null)
                    {
                        retVal = true;
                        break;
                    }
                }
                
                return retVal;
            }
            
        }

        public void Tick()
        {
            Connect.Tick();
            Connect.LateTick();

            foreach(SocketBehaviour s in Sockets)
                s.Tick();
        }
        
        public virtual void ConfirmConnect()
        {
            /*if (ShipComponent != null)
            {
                Connect.Confirm();
            }*/
        }

        public void LerpMove(Vector3 newPos)
        {
            gameObject.GetComponent<RectTransform>().position += newPos;
        }

        public Texture2D GetTex
        {
            get{ return CompImage.texture as Texture2D;}
        }

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

        public bool ChangePending
        {
            get{ return Connect.ChangedSockets;}
            set{ 
                if(Connect == null)
                    return;
                Connect.ChangedSockets = value;}
        }
    }
}

