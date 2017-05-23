using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;

namespace Data.Shared
{
    [System.Serializable]
    public struct Socket
    {
        public int SocketID;
        public int OtherID;
        public int OtherLinkID;
    }

    [System.Serializable]
    public struct Component
    {
        // Unique ID of piece
        public int InstanceID;
        // direction obj is facing
        public string Direction;
        // key that triggers obj
        public string Trigger;
        // folder that the piece is stored in (component type)
        public string CTrigger;
        // folder that the piece is stored in (component type)
        public string Folder;
        // name of the prefab for the component
        public string Name;
        // visual colour of component
        public string Style;
        // list of socket links to other pieces
        public Socket[] sockets;

        // Targeter and listener specific data
        public bool AutoLock;
        public int behaviour;
        public bool AutoFire;

        public string Path
        {
            get { return Folder + "/" + Name; }
        }

        public void Init()
        {
            Direction = "null";
            Trigger = "null";
            CTrigger = "null";
            Folder = "null";
            Name = "null";
            Style = "null";

            sockets = new Socket[0];
        }
    }

    [System.Serializable]
    public struct ShipData 
    {
        public Component[] components;
        private Component _head;
        public string Name;
        //private Dictionary<string, float> _requirements;
        public string Description;
        public string Category;
        public bool PlayerMade;
        public bool CombatResponsive;

        //-                                     // Ship being controlled will act differently in combat
        public bool CombatActive;
        // -                                    // Ship is aligned with mouse
        public bool Aligned;

        //[NonSerialized]
        //public Texture2D IconTex;

        public void Init()
        {
            if (components == null)
                components = new Component[0];

            Name = "null";
            Description = "null";
            Category = "null";

            _head.Init();
        }

    	/// <summary>
    	/// Adds the component.
    	/// to the Ships Data
    	/// </summary>
    	/// <param name="newComponent">New component.</param>
    	/// <param name="isHead">If set to <c>true</c> is head.</param>
    	public void AddComponent
    		(Component newComponent,
    		 bool isHead)
    	{
            if(components == null)
                components = new Component[0];
            GameObject GO = Resources.Load("Space/Ships/" + newComponent.Folder + "/" + newComponent.Name, typeof(GameObject)) as GameObject;
            ComponentAttributes att = GO.GetComponent<ComponentAttributes>();
            if(att.RequiredMats != null)
            {
                foreach(ConstructInfo mat in att.RequiredMats)
                    AddRequirement(mat.material, mat.amount);
            }

            if(newComponent.sockets == null)
                newComponent.sockets = new Socket[0];

            if (isHead)
            {
                _head = newComponent;
                return;
            }

            // Initialize the list
            int index = 0;
            Component[] temp = components;
            components = new Component[components.Length+1];
            while(index < components.Length-1)
            {
                components[index] = temp[index++];
            }
    		components[index] = newComponent;
    	}

        public void AddRequirement(string mat, float amt)
        {
            /*if (mat != null && amt > 0)
            {
                if(_requirements == null)
                    _requirements = new Dictionary<string, float>();

                if(_requirements.ContainsKey(mat))
                {
                    _requirements[mat] += amt;
                }
                else
                {
                    _requirements.Add(mat, amt);
                }
            }*/
        }

        public Dictionary<string, float> GetRequirements()
        {
            /*if(_requirements == null)
                _requirements = new Dictionary<string, float>();

            return _requirements;*/
            return null;
        }

    	public Component Head
    	{
            get { return _head;}
            set { _head = value; }
    	}

    	/// <summary>
    	/// Gets a copy component.
    	/// Based on the provided instance ID
    	/// </summary>
    	/// <returns>The component.</returns>
    	/// <param name="ID">I.</param>
    	public Component GetComponent
    		(int ID)
    	{
            if(components != null)
            {
        		foreach (Component piece
        		        in components)
        		{
        			if(piece.InstanceID == ID)
        				return piece;
        		}
            }

    		if (_head.InstanceID == ID)
    			return _head;

    		return new Component();
    	}

        public void AddSocket(int toID, int linkID, 
                              int otherLinkID, int OtherID)
        {
            Socket newSocket = new Socket ();
            newSocket.SocketID = linkID;
            newSocket.OtherLinkID = otherLinkID;
            newSocket.OtherID = OtherID;

            if (components != null)
            {
                for(int i = 0; i < components.Length; i++)
                {
                    if(components[i].InstanceID == toID)
                    {
                        int index = 0;
                        Socket[] temp = components[i].sockets;
                        components[i].sockets = new Socket[components[i].sockets.Length + 1];
                        while (index < components[i].sockets.Length - 1)
                        {
                            components[i].sockets[index] = temp[index++];
                        }

                        components[i].sockets[index] = newSocket;
                        break;
                    }
                }
            }
            
            if (_head.InstanceID == toID)
            {
                int index = 0;
                Socket[] temp = _head.sockets;
                _head.sockets = new Socket[_head.sockets.Length + 1];
                while (index < _head.sockets.Length - 1)
                {
                    _head.sockets[index] = temp[index++];
                }

                _head.sockets[index] = newSocket;
            }
        }
        /// <summary>
        /// Gets all the components stored.
        /// </summary>
        /// <returns>The components.</returns>
    	public Component[] GetComponents
    		()
    	{
    		return components;
    	}
    }
}

