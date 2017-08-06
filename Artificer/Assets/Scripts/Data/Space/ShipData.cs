using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;
using Data.Space.Collectable;
using Data.Space.Library;

namespace Data.Space
{
    [System.Serializable]
    public struct SocketData
    {
        public int SocketID;
        public int OtherID;
        public int OtherLinkID;
    }

    [System.Serializable]
    public struct ComponentData
    {
        /// <summary>
        /// Unique ID of piece
        /// </summary>
        public int InstanceID;
        /// <summary>
        /// direction obj is facing
        /// </summary>
        public string Direction;
        /// <summary>
        /// key that triggers obj
        /// </summary>
        public string Trigger;
        /// <summary>
        /// folder that the piece is stored in (component type)
        /// </summary>
        public string CTrigger;
        /// <summary>
        /// folder that the piece is stored in
        /// </summary>
        public string Folder;
        /// <summary>
        /// name of the prefab for the component
        /// </summary>
        public string Name;
        /// <summary>
        /// visual colour of component
        /// </summary> 
        public string Style;
        /// <summary>
        /// currency cost of the item
        /// </summary>
        public int Cost;
        /// <summary>
        /// list of socket links to other pieces
        /// </summary> 
        public SocketData[] sockets;
        /// <summary>
        /// materials required to 
        /// build item
        /// </summary>
        public ItemCollectionData[] requirements;

        // Targeter and listener specific data
        public bool AutoLock;
        public int behaviour;
        public bool AutoFire;

        public string Path
        {
            get { return Folder + "/" + Name; }
        }

        /*public void Init()
        {
            Direction = "null";
            Trigger = "null";
            CTrigger = "null";
            Folder = "null";
            Name = "null";
            Style = "null";

            sockets = new SocketData[0];
            requirements = new ItemCollectionData[0];
        }*/
    }

    [System.Serializable]
    public struct ShipData 
    {
        public ComponentData[] components;
        public ComponentData _head;
        public string Name;
        public string Description;
        public string Category;
        public bool PlayerMade;
        public bool CombatResponsive;

        /// <summary>
        /// Ship being controlled will act differently in combat
        /// </summary> 
        public bool CombatActive;
        /// <summary>
        /// Ship is aligned with mouse
        /// </summary>
        public bool Aligned;

        /*public void Init()
        {
            if (components == null)
                components = new ComponentData[0];

            Name = "null";
            Description = "null";
            Category = "null";

            _head.Init();
        }*/

    	/// <summary>
    	/// Adds the component.
    	/// to the Ships Data
    	/// </summary>
    	/// <param name="newComponent">New component.</param>
    	/// <param name="isHead">If set to <c>true</c> is head.</param>
    	public void AddComponent
    		(ComponentData newComponent,
    		 bool isHead)
    	{
            if(components == null)
                components = new ComponentData[0];

            if(newComponent.sockets == null)
                newComponent.sockets = new SocketData[0];

            if (isHead)
            {
                _head = newComponent;
                return;
            }

            // Initialize the list
            int index = 0;
            ComponentData[] temp = components;
            components = new ComponentData[components.Length+1];
            while(index < components.Length-1)
            {
                components[index] = temp[index++];
            }
            
            components[index] = newComponent;
        }

        public void Clear()
        {
            components = new ComponentData[0];
            _head = new ComponentData();
        }

    	public ComponentData Head
    	{
            get { return _head;}
    	}

    	/// <summary>
    	/// Gets a copy component.
    	/// Based on the provided instance ID
    	/// </summary>
    	/// <returns>The component.</returns>
    	/// <param name="ID">I.</param>
    	public ComponentData GetComponent
    		(int ID)
    	{
            if(components != null)
            {
        		foreach (ComponentData piece
        		        in components)
        		{
        			if(piece.InstanceID == ID)
        				return piece;
        		}
            }

    		if (_head.InstanceID == ID)
    			return _head;

    		return new ComponentData();
    	}

        public void AddSocket(int toID, int linkID, 
                              int otherLinkID, int OtherID)
        {
            SocketData newSocket = new SocketData ();
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
                        SocketData[] temp = components[i].sockets;
                        components[i].sockets = new SocketData[components[i].sockets.Length + 1];
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
                SocketData[] temp = _head.sockets;
                _head.sockets = new SocketData[_head.sockets.Length + 1];
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
    	public ComponentData[] GetComponents   		
    	{
    		get { return components; }
    	}

        /// <summary>
        /// Returns total cost of all the items
        /// </summary>
        public int Cost
        {
            get
            {
                int cost = 0;

                foreach(ComponentData data in components)
                {
                    cost += data.Cost;
                }

                cost += Head.Cost;

                return cost;
            }
        }

        /// <summary>
        /// Iterates through each component
        /// and creates a spawn time based on their
        /// value 
        /// </summary>
        public float SpawnTime
        {
            get
            {
                float time = 0;

                foreach (ComponentData data in GetComponents)
                {
                    time += data.Cost * 0.001f;
                }

                time += Head.Cost * 0.001f;

                return time;
            }
        }

        /// <summary>
        /// returns complete list 
        /// of item requirements
        /// </summary>
        public ItemCollectionData[] Requirements
        {
            get
            {
                ItemCollectionData[] totalRequirements
                    = new ItemCollectionData[Head.requirements.Length];

                // copy current requirments into list
                int index = 0;
                while (index > totalRequirements.Length)
                    totalRequirements[index] = Head.requirements[index++];

                foreach (ComponentData data in components)
                {
                    ItemCollectionData[] temp =
                        new ItemCollectionData
                        [totalRequirements.Length + data.requirements.Length];

                    // copy current requirments into list
                    index = 0;
                    while (index > totalRequirements.Length)
                        temp[index] = totalRequirements[index++];

                    // copy the new list to our list
                    int a = 0;
                    while (a < data.requirements.Length)
                        temp[index++] = totalRequirements[a++];

                    totalRequirements = temp;
                }

                return totalRequirements;
            }

        }
    }
}

