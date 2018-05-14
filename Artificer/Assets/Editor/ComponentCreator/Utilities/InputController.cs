using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ArtificerEditor.Components
{
    public class InputController
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
    	
        // Update is called once per frame
        public static void Update()
        {
            if (_att.Dragging)
            {
                _att.SelectedSocket.Position = _att.MousePos - _att.CenterPoint;
            }

            // launcher and weapon are allow firepoints
            if (_att.Type.SelectedName != "Weapons" 
                && _att.Type.SelectedName != "Launchers")
            {
                // Copy lift of firepoints
                ArrayList FPs = new ArrayList();
                foreach(ComponentObject sock in _att.Sockets)
                    if(sock is FirePoint)
                        FPs.Add(sock);

                // if firepoints exist, destroy them
                if(FPs.Count != 0)
                {
                    foreach(FirePoint FP in FPs)
                    {
                        _att.Sockets.Remove(FP);
                        if(_att.SelectedSocket == FP)
                            _att.SelectedSocket = null;
                    }
                }
            }
            
            if (_att.Type.SelectedName != "Engines" 
                && _att.Type.SelectedName != "Rotors")
            {
                if(_att.EP != null)
                {
                    _att.Sockets.Remove(_att.EP);
                    if(_att.SelectedSocket == _att.EP)
                        _att.SelectedSocket = null;
                    _att.EP = null;
                }
            }
        }

        /// <summary>
        /// Handles mouse
        /// input from the user on the window
        /// </summary>
        /// <param name="e">E.</param>
        /// <param name="position">Position.</param>
        public static void MouseHandle(Event e)
        {
            // store mouse position
            _att.MousePos = e.mousePosition;

                // right click handle
                if (e.button == 1)
                {
                    if (e.type == EventType.MouseDown)
                    {
                        bool clickedOnSocket = false;
                        _att.RightClickPos = _att.MousePos;
                        
                        foreach (ComponentObject socket in _att.Sockets)
                        {
                            if (Vector2.Distance(socket.Position, _att.MousePos - _att.CenterPoint) < 10f)
                            {
                                _att.SelectedSocket = socket;
                                clickedOnSocket = true;
                            }
                        }

                        if(clickedOnSocket)
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Delete"), false, ComponentCallback.ContextCallback, "deleteSocket"); 
                            menu.AddSeparator("");
                            menu.AddItem(new GUIContent("Up"), false, ComponentCallback.ContextCallback, "up"); 
                            menu.AddItem(new GUIContent("Down"), false, ComponentCallback.ContextCallback, "down");
                            menu.AddItem(new GUIContent("Left"), false, ComponentCallback.ContextCallback, "left");
                            menu.AddItem(new GUIContent("Right"), false, ComponentCallback.ContextCallback, "right");
                            menu.ShowAsContext();
                            e.Use();
                        }
                        else
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Create New Socket"),
                                         false, ComponentCallback.ContextCallback, "createSocket");

                            // only allow corresponding points
                            if(_att.Type.SelectedName == "Weapons" || _att.Type.SelectedName == "Launchers")
                                menu.AddItem(new GUIContent("Create New FirePoint"),
                                             false, ComponentCallback.ContextCallback, "createFirePoint");

                            if(_att.Type.SelectedName == "Engines" || _att.Type.SelectedName == "Rotors")
                                menu.AddItem(new GUIContent("Create Engine Prefab"), 
                                             false, ComponentCallback.ContextCallback, "createEnginePoint");

                            menu.ShowAsContext();
                            e.Use();
                        }
                    }
                }
                // left click handler
                else if (e.button == 0)
                {
                    if(e.type == EventType.MouseDown)
                    {
                        foreach (ComponentObject socket in _att.Sockets)
                        {
                            if (Vector2.Distance(socket.Position, _att.MousePos - _att.CenterPoint) < 10f)
                            {
                                if(_att.SelectedSocket == socket)
                                    _att.Dragging = true;
                                else
                                    _att.SelectedSocket = socket;
                            }
                        }
                    }
                    else if(e.type == EventType.MouseUp)
                    {
                        _att.Dragging = false;
                    }
                }
        }
    }
}
