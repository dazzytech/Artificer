using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Data.Space;

namespace Editor.Components
{
    public class SocketController
    {
        private static ComponentAttributes _att;

        /// <summary>
        /// Sets the utilties socket list pointer
        /// </summary>
        /// <param name="list">List.</param>
        public static void SetAttributes(ComponentAttributes att)
        {
            _att = att;
            _att.Sockets = new List<ComponentObject>();
        }

        /// <summary>
        /// Populates the socket list.
        /// using the sockets from the game object provided
        /// </summary>
        /// <returns>The total socket count.</returns>
        /// <param name="baseObj">Base object.</param>
        /// <param name="bounds">Bounds.</param>
        public static void PopulateSocketList(GameObject baseObj, Texture2D bounds)
        {
            // clear local list to populate fresh
            _att.SocketCount = 0;

            // check that the local socketList has been defined
            if (_att == null)
            {
                return; 
            }

            _att.Sockets.Clear();

            // search for child sockets
            foreach (Transform child in baseObj.transform)
            {
                if(child.name.Contains("socket_"))
                {
                    // child is a socket
          
                    ComponentSocket sock =                              // Create the correct component object to store the socket
                        (ComponentSocket)ScriptableObject.
                            CreateInstance("ComponentSocket");

                    sock.Position =                                     // convert unit to pixels and store location locally
                        new Vector2(child.localPosition.x*100 
                                    + bounds.width/2, 
                                    -(child.localPosition.y*100)
                                    + bounds.height/2);

                    int SockID = 0;

                    if(int.TryParse(Regex.Match(child.name, @"\d+").Value, out SockID))
                        sock.ID = SockID;

                    else
                    {
                        Debug.Log("SocketController: PopulateSocketList - Socket Not Found");
                        continue;
                    }

                    _att.SocketCount++;                                 // increment socket counter

                    SocketData behaviour =                              // Retreive SocketData if exists
                        child.GetComponent<SocketData>();

                    if(behaviour != null)       
                    {                   
                        // Socket data exists
                        // copy data into our object
                        sock.socketDirection = behaviour.Direction;
                        sock.socketType = behaviour.Type;
                    }
                    else
                    {
                        // Socket data doesn't exist, set defaults
                        sock.socketDirection = "up";
                        sock.socketType = 0;
                    }

                    _att.Sockets.Add(sock);                             // Add component socket object to 
                                                                        // socket list
                }
                if(child.name.Contains("firePort"))
                {
                    // Object is infact a FirePoint
                    FirePoint FP = (FirePoint)ScriptableObject.              // Create the Component FirePoint
                        CreateInstance("FirePoint");

                    FP.Position =                                  // Convert Units to Pixels and store position
                        new Vector2(child.localPosition.x*100 + bounds.width/2,
                                    -(child.localPosition.y*100) + bounds.height/2);

                    _att.Sockets.Add(FP);                          // Add object to socket list
                }
                if(child.name.Contains("Engine"))
                {
                    _att.EP = (EnginePoint)ScriptableObject. // Create the Component EnginePoint
                        CreateInstance("EnginePoint");

                    _att.EP.Position =                       // Convert Units to Pixels and store position
                        new Vector2(child.localPosition.x*100 + bounds.width/2,
                                    -(child.localPosition.y*100) + bounds.height/2);

                    _att.Sockets.Add(_att.EP);               // Add object to socket list
                }
            }
        }

        /// <summary>
        /// Loops through each socket stored and invokes the draw
        /// function
        /// </summary>
        /// <param name="center">Center.</param>
        public static void DrawSockets()
        {
            foreach(ComponentObject socket in _att.Sockets)
            {
                socket.Draw(_att.CenterPoint);
            }
        }

        public static ComponentSocket GetSocket(int ID)
        {
            foreach (ComponentObject sock in _att.Sockets)
            {
                if(sock is ComponentSocket)
                {
                    if(((ComponentSocket)sock).ID == ID)
                        return (ComponentSocket)sock;
                }
            }
            return null;
        }
    }
}

