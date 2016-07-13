using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace Editor
{
    [XmlRoot("info")]
    public class Info
    {
        [XmlElement("name")]
        public string name = "";
        [XmlElement("cat")]
        public string category = "";
        [XmlElement("desc")]
        public string description = "";
    }
	/// <summary>
	/// Component container.
	/// Create serializable data for storage
	/// </summary>
	[XmlRoot("ship")]
	public class ShipContainer
    {
        [XmlAttribute("player")]
        public bool Player;

        [XmlAttribute("rotorFollow")]
        public bool RotorFollow;

        [XmlArray("pieces")]
        [XmlArrayItem("body", typeof(ShipComponent))]
        [XmlArrayItem("head", typeof(HeadComponent))]
        public List<BaseShipComponent> Pieces 
            = new List<BaseShipComponent>();

        [XmlArray("links")]
        [XmlArrayItem("link")]
        public List<Socket> Links
            = new List<Socket>();

        public Info info = new Editor.Info();

        [XmlIgnore]
        public string Name;

        [XmlIgnore]
        public TextAsset Asset;

        [XmlIgnore] 
        HeadComponent head;

        public void Save()
		{
            bool hasHead = false;
            foreach (BaseShipComponent b in Pieces)
                if (b.IsHead)
                    hasHead = true;

            if (!hasHead)
            {
                if (UnityEditor.EditorUtility.DisplayDialog("Incomplete",
                    "Your ship has no head piece", "Ok"))
                    return;
            }
            socketIDs.Clear();

            foreach (BaseShipComponent b in Pieces)
            {
                if(b.IsHead)
                {        
                    head = ScriptableObject.CreateInstance("HeadComponent")
                            as HeadComponent;

                    head.instanceID = b.instanceID;
                    head.direction = b.direction;
                    head.Bounds = b.Bounds;
                    head.pieceName = b.pieceName;
                    head.pieceType = b.pieceType;
                    head.SetStyle(b.current);
                    head.SetGO = b.SetGO;
                    head.IsHead = true;
                    head.sockets = b.sockets;
                    head.rotation = b.rotation;
                    Pieces[Pieces.IndexOf(b)] = head;
                    
                    Links = FormatSockets(b);
                }
            }

            if(Player)
                ShipExternalUtility.CreateIcon(Pieces, info.name != ""? info.name: Name);
            ShipExternalUtility.SaveShipContainer(this, Name);
	    }

        public void Load()
        {
            if (Asset == null)
                return;

            ShipContainer newShip = ShipExternalUtility.LoadShipContainer(Asset.name);

            if (newShip == null)
            {
                Debug.Log("Ship could not be loaded: " + Asset.name);
            }

            Clear();

            Name = Asset.name;
            Player = newShip.Player;
            RotorFollow = newShip.RotorFollow;
            info = newShip.info;

            // add all the pieces together into the list
            Pieces.AddRange(newShip.Pieces.ToArray());

            // SetGO of each component using it's component ID
            foreach (BaseShipComponent p in Pieces)
            {
                p.SetGO = (GameObject)Resources.Load("Space/Ships/" + 
                            p.pieceType + "/" + p.pieceName);
                p.SetStyle(p.current);
            }

            // Assign and connect socket
            foreach (Socket s in newShip.Links)
            {
                // find first object
                BaseShipComponent firstObj = null;
                // find second object
                BaseShipComponent scndObj = null;

                foreach(BaseShipComponent p in Pieces)
                {
                    if(p.instanceID == s.objectID)
                        firstObj = p;
                    else if(p.instanceID == s.objToID)
                        scndObj = p;
                }

                if(firstObj != null && scndObj != null)
                {
                    firstObj.LoadConnection(int.Parse(s.socketID), scndObj, int.Parse(s.sockToID));
                }
            }
        }

        List<string> socketIDs = new List<string>();
        public List<Socket> FormatSockets(BaseShipComponent piece, BaseShipComponent origin = null)
        {
            List<Socket> newList = new List<Socket>();
            foreach (SocketBehaviour s in piece.sockets)
            {
                if(socketIDs.Contains(s.SocketID + s.ObjectID))
                {
                    continue;
                }
                if(s.state == SocketAttributes.SocketState.CLOSED)
                {
                    BaseShipComponent nextPiece;
                    // We don't want connection list looping on itself
                    if(s.connectedSocket.container.Equals(origin) ||
                       s.connectedSocket.container.Equals(head))
                        continue;

                        // we are good. 
                    Socket newSocket = new Socket();
                    newSocket.objectID = piece.instanceID;
                    newSocket.socketID = s.SocketID;

                    nextPiece = s.connectedSocket.container;
                    newSocket.objToID = nextPiece.instanceID;
                    newSocket.sockToID = s.connectedSocket.SocketID;
                    newList.Add(newSocket);

                    socketIDs.Add(s.SocketID + s.ObjectID);
                    newList.AddRange(FormatSockets(nextPiece, piece));
                }
            }
            return newList;
        }

        public void Clear()
        {
            Pieces.Clear();
            Links.Clear();
            Name = "";
            info = new Info();
        }
	}
}

