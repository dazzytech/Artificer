using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ArtificerEditor.Components
{
    public class ComponentCallback
    {
        // Attributes Pointer
        private static ComponentAttributes _att;
        
        /// <summary>
        /// Sets the attributes.
        /// invoked in CC constructor
        /// </summary>
        /// <param name="att">Att.</param>
        public static void SetAttributes(ComponentAttributes att)
        {
            _att = att;
        }

        /// <summary>
        /// Context callback.
        /// used by the input controller for handling mouse
        /// events
        /// </summary>
        /// <param name="obj">Object.</param>
        public static void ContextCallback(object obj)
        {
            string clb = obj.ToString();
            
            if (clb.Equals("createSocket"))
            {
                _att.SelectedSocket = (ComponentSocket)ScriptableObject.CreateInstance("ComponentSocket");
                _att.SelectedSocket.Position = _att.RightClickPos - _att.CenterPoint; // change to store mousepos at click
                _att.SelectedSocket.socketType = 0;
                _att.SelectedSocket.socketDirection = "up";
                ((ComponentSocket)_att.SelectedSocket).ID = _att.SocketCount++;
                _att.Sockets.Add(_att.SelectedSocket);
            } else if (clb.Equals("createFirePoint")) 
            {
                // Add a firepoint to the object
                FirePoint FP = new FirePoint();
                FP = (FirePoint)ScriptableObject.CreateInstance("FirePoint");
                FP.Position = _att.RightClickPos - _att.CenterPoint;
                _att.SelectedSocket = FP;
                _att.Sockets.Add(_att.SelectedSocket);
            } 
            else if (clb.Equals("createEnginePoint")) {
                if(_att.EP == null)
                {
                    _att.EP = (EnginePoint)ScriptableObject.CreateInstance("EnginePoint");
                    _att.EP.Position = _att.RightClickPos - _att.CenterPoint;
                    _att.SelectedSocket = _att.EP;
                    _att.Sockets.Add(_att.SelectedSocket);
                }
            } else if (clb.Equals("deleteSocket"))
            {
                _att.Sockets.Remove(_att.SelectedSocket);
                if(_att.SelectedSocket == _att.EP)
                    _att.EP = null;
                else
                {
                    // loop through each socket 
                    // numbering correctly
                    _att.SocketCount = 0;
                    foreach(ComponentObject sock in _att.Sockets)
                    {
                        if(sock is ComponentSocket)
                            ((ComponentSocket)sock).ID = _att.SocketCount++;
                    }
                }
                _att.SelectedSocket = null;
            } else {
                _att.SelectedSocket.socketDirection = clb;
            }
        }
    }
}

