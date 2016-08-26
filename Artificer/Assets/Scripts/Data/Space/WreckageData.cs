using UnityEngine;
using System.Collections;
using Data.Space.Library;

using Data.Shared;
using Space.Ship.Components.Attributes;

namespace Data.Space
{
    [System.Serializable]
    public struct WreckageData
    {
        public Data.Shared.Component[] components;

        /// <summary>
        /// Adds the component.
        /// to the Wreckage Data
        /// </summary>
        /// <param name="newComponent">New component.</param>
        /// <param name="isHead">If set to <c>true</c> is head.</param>
        public void AddComponent
            (Data.Shared.Component newComponent)
        {
            if (components == null)
                components = new Data.Shared.Component[0];

            if (newComponent.sockets == null)
                newComponent.sockets = new Socket[0];

            // Initialize the list
            int index = 0;
            Data.Shared.Component[] temp = components;
            components = new Data.Shared.Component[components.Length + 1];
            while (index < components.Length - 1)
            {
                components[index] = temp[index++];
            }
            newComponent.InstanceID = index;
            components[index] = newComponent;
        }

        /// <summary>
        /// Gets a copy component.
        /// Based on the provided instance ID
        /// </summary>
        /// <returns>The component.</returns>
        /// <param name="ID">I.</param>
        public Data.Shared.Component GetComponent
            (int ID)
        {
            if (components != null)
            {
                foreach (Data.Shared.Component piece
                        in components)
                {
                    if (piece.InstanceID == ID)
                        return piece;
                }
            }

            return new Data.Shared.Component();
        }

        public void AddSocket(int toID, int linkID,
                              int otherLinkID, int OtherID)
        {
            Socket newSocket = new Socket();
            newSocket.SocketID = linkID;
            newSocket.OtherLinkID = otherLinkID;
            newSocket.OtherID = OtherID;

            if (components != null)
            {
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].InstanceID == toID)
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
        }
    }
}

