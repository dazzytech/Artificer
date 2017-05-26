using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
//Artificer
using Data.Space;
using Space.Ship.Components;
using Space.Ship.Components.Attributes;

namespace Editor
{
    [XmlInclude(typeof(ShipComponent))]
    [XmlInclude(typeof(HeadComponent))]
	public class BaseShipComponent: ScriptableObject{

        // Xml Ignored Data
        [XmlIgnore]
        private GameObject selectedPiece;
        [XmlIgnore]
        // store a dictionary of sprites
        protected Dictionary<string, Sprite> componentSprites;
        [XmlIgnore]
        protected ComponentType type;
        // Socket related
        [XmlIgnore]
        public List<SocketBehaviour> sockets;
        [XmlIgnore]
        ConnectionBehaviour connect;
        // texture data
        [XmlIgnore] 
        protected Texture2D open;
        [XmlIgnore] 
        protected Texture2D closed;
        [XmlIgnore] 
        protected Texture2D pending;
        [XmlIgnore] 
        public Sprite NoObject;

        // Save editor data
        public Rect Bounds;
        public float rotation;
        public bool IsHead;

        // Game data
        [XmlAttribute("instanceID")]
        public string instanceID;

        [XmlAttribute("folder")]
        public string pieceType;

        [XmlAttribute("name")]
        public string pieceName;

        [XmlAttribute("direction")]
        public string direction = "up";

        [XmlAttribute("trigger")]
        public string triggerKey = "";

        [XmlAttribute("combat")]
        public string combatKey = "";

        [XmlAttribute("style")]
        public string current;

        private void OnEnable()
        {
            connect = ScriptableObject.CreateInstance<Editor.ConnectionBehaviour>()
                as ConnectionBehaviour;

            // initialize sockets
            sockets = new List<SocketBehaviour>();


            // initialize texture and position
            componentSprites = new Dictionary<string, Sprite>();
            Bounds = new Rect(0, 0, 50, 50);

            // initialize socket textures
            open = Resources.Load("Textures/ShipEditor/socket_open",
                                  typeof(Texture2D)) as Texture2D;

            pending = Resources.Load("Textures/ShipEditor/socket_pending",
                                      typeof(Texture2D)) as Texture2D;

            closed = Resources.Load("Textures/ShipEditor/socket_closed",
                                    typeof(Texture2D)) as Texture2D;

            pieceName = "Unassigned";
            pieceType = "Unassigned";
            IsHead = false;

            NoObject = Resources.Load("Textures/ShipEditor/noimage",
                                      typeof(Sprite)) as Sprite;
            //default tex size
            Bounds.size = NoObject.
                textureRect.size;
        }

        [XmlIgnore]
        public GameObject SetGO
		{
            set
            {
                if(selectedPiece != null)
                    if(selectedPiece.Equals(value))
                        return;

                if(type == null)
                    type = new ComponentType();

                type.ImportType(value);
                selectedPiece = value;
                pieceName = value.name;
                pieceType = type.SelectedName;

                AssignTexture();
                AssignSockets();
            }

            get
            {
                return selectedPiece;
            }
		}

        public virtual void Tick()
        {
            if (selectedPiece != null)
            {
                connect.Tick();
                connect.LateTick();
            }

        }

        public virtual void Connect()
        {
            if (selectedPiece != null)
            {
                connect.Confirm();
            }
            foreach(SocketBehaviour s in sockets)
               s.Tick();
        }

        public void OnDrawGizmos()
        {
            foreach (Editor.SocketBehaviour socket in sockets) 
            {
                Rect sRect = new Rect(socket.position-new Vector2(4,4), new Vector2(8,8));
                if (socket.state == SocketAttributes.SocketState.CLOSED) 
                {
                    GUI.DrawTexture (sRect, closed);
                }
                else if (socket.state == SocketAttributes.SocketState.PENDING)
                {
                    GUI.DrawTexture (sRect, pending);
                }
                else
                {
                    GUI.DrawTexture (sRect, open);
                }
            }
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
            sockets.Clear();

            foreach (Transform t in selectedPiece.transform)
            {
                if(t.name.Contains("socket_"))
                {
                    SocketBehaviour s = ScriptableObject.CreateInstance<Editor.SocketBehaviour>()
                        as SocketBehaviour;

                    s.Init(this);

                    Vector2 newPos = new Vector2();
                    newPos.y = (-t.localPosition.y*100f) + Bounds.height/2;
                    newPos.x = (t.localPosition.x*100f) + Bounds.width/2;
                    newPos = RotateAroundPoint(newPos, (Bounds.center - Bounds.position), Quaternion.Euler(0, 0, rotation));

                    s.position = newPos;

                    // If socketdata exists then use that otherwise 
                    // calc direction
                    SocketData sock = t.GetComponent<SocketData>();
                    if(sock != null)
                    {
                        SocketAttributes.Alignment up = SocketAttributes.Alignment.UP;
                        SocketAttributes.Alignment down = SocketAttributes.Alignment.DOWN;
                        SocketAttributes.Alignment left = SocketAttributes.Alignment.LEFT;
                        SocketAttributes.Alignment right = SocketAttributes.Alignment.RIGHT;

                        switch(GetDirection())
                        {
                            case 0:
                                // up
                                break;
                            case 1:
                                up = SocketAttributes.Alignment.DOWN;
                                down = SocketAttributes.Alignment.UP;
                                left = SocketAttributes.Alignment.RIGHT;
                                right = SocketAttributes.Alignment.LEFT;
                                // down
                                break;
                            case 2:
                                up = SocketAttributes.Alignment.LEFT;
                                down = SocketAttributes.Alignment.RIGHT;
                                left = SocketAttributes.Alignment.DOWN;
                                right = SocketAttributes.Alignment.UP;
                                // left
                                break;
                            case 3:
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
                                s.alignment = up;
                                break;
                            case "down":
                                s.alignment = down;
                                break;
                            case "left":
                                s.alignment = left;
                                break;
                            case "right":
                                s.alignment = right;
                                break;
                        }
                    }
                    else
                    {
                        Vector2 dir = newPos - (Bounds.center - Bounds.position);
                        if (Mathf.Abs (dir.y) > Mathf.Abs (dir.x)) 
                            // Collision on Y Axis
                            // been set to -1
                            if(Mathf.Sign (dir.y) == -1)                   // Object has a collision on top 
                                s.alignment = SocketAttributes.Alignment.UP;
                            else                                          // object has a collision on bottom 
                                s.alignment = SocketAttributes.Alignment.DOWN;
                        else 
                            // Collision on X Axis
                            if(Mathf.Sign (dir.x) == 1)                   // object has a collision on right    
                                s.alignment = SocketAttributes.Alignment.RIGHT;
                            else                                                // object has a collision on left 
                                s.alignment = SocketAttributes.Alignment.LEFT;
                    }

                    string[] spl = t.name.Split('_');
                    s.SocketID = spl[1];
                    s.ObjectID = instanceID;
                    sockets.Add(s);

                }
            }

            connect.Init(sockets);
        }

        /// <summary>
        /// Used for loading ships
        /// finds the correct socket to connect
        /// to and sends it to the connection
        /// behaviour to directly connect
        /// </summary>
        /// <param name="sockID">Sock I.</param>
        /// <param name="other">Other.</param>
        /// <param name="otherSockID">Other sock I.</param>
        public void LoadConnection(int sockID, BaseShipComponent other, int otherSockID)
        {
            if (connect == null)
                connect = new ConnectionBehaviour();

            foreach (SocketBehaviour s in other.sockets)
            {
                int sID = int.Parse(s.SocketID);
                if(sID == otherSockID)
                {
                    connect.ConnectToPiece(sockID, s);
                    break;
                }
            }

        }

        public Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle)
        {
            return angle * ( point - pivot) + pivot;
        }

        private void AssignTexture()
        {
            SpriteRenderer rend = null;
            componentSprites.Clear();

            //default tex size
            Bounds.size = NoObject.
                textureRect.size;

            ComponentAttributes att 
                = selectedPiece.GetComponent<ComponentAttributes>();

            if (att == null)
                return;

            if (att.componentStyles.Count() > 0)
            {
                foreach (StyleInfo info in att.componentStyles)
                {
                    componentSprites.Add(info.name, info.sprite);
                }
            } else
            {
                foreach (Transform t in selectedPiece.transform)
                {
                    rend = t.transform.GetComponent
                    <SpriteRenderer>();
                   
                    if (rend != null)
                    if (rend.sprite != null)
                         componentSprites.Add(current, rend.sprite);
                    else
                        Debug.Log("Failed to load component sprite or renderer");
                }
            }

            if(current == null)
                current = "default";

            if (componentSprites.ContainsKey(current))
            {
                Bounds.size = componentSprites [current].
                    textureRect.size;
            } else
            {
                Bounds.size = NoObject.textureRect.size;
            }
        }

        public void MovePiece(Vector3 newPos)
        {
            Bounds.x = newPos.x - Bounds.width * .5f;
            Bounds.y = newPos.y - Bounds.height * .5f;
        }

        public void LerpMove(Vector3 newPos)
        {
            Bounds.x += newPos.x;
            Bounds.y += newPos.y;
        }

        public void AddCollision(BaseShipComponent collider)
        {
           if (!connect.collidedObjects.Contains(collider))
               connect.collidedObjects.Add(collider);
        }

        public void SetDirection(int dir)
        {
            string oldDir = direction;
            switch (dir)
            {
                case 0:
                    direction = "up";
                    rotation = 0f;
                    break;
                case 1:
                    direction = "down";
                    rotation = 180f;
                    break;
                case 2:
                    direction = "left";
                    rotation = 270f;
                    break;
                case 3:
                    direction = "right";
                    rotation = 90f;
                    break;
            }
            if(selectedPiece != null 
               && !oldDir.Equals(direction))
                   AssignSockets();
        }

        public int GetDirection()
        {
            switch (direction)
            {
                case "up":
                    return 0;
                case "down":
                    return 1;
                case "left":
                    return 2;
                case "right":
                    return 3;
            }
            return 0;
        }

        public int StyleSelect
        {
            get
            {
                int i = 0;
                foreach(string style in componentSprites.Keys)
                {
                    if(style == current)
                        return i;
                    else
                        i++;
                }

                return 0;
            }
            set
            {
                int i = 0;
                foreach(string style in componentSprites.Keys)
                {
                    if(value == i)
                    {
                        current = style;
                        return;
                    }
                    else
                        i++;
                }
            }
        }

        public void SetStyle(string newStyle)
        {
            current = newStyle;
        }

        public string[] StyleList()
        {
            return componentSprites.Keys.ToArray();
        }

        public Texture2D GetTex()
        {
            if (current == null)
                return NoObject.texture;

            if (componentSprites.ContainsKey(current))
            {
                if (componentSprites [current] != null)
                    return componentSprites [current].texture;
            }

            return NoObject.texture;
        }

        public string GetName()
        {
            return pieceName;
        }

        public float Rotation
        {
            get { return rotation;}
        }
	}
}
