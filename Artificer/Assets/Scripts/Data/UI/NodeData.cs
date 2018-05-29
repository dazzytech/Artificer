﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using System.Linq;

namespace Data.UI
{
    /// <summary>
    /// stores information about the node prefab
    /// and generates the code snippet from the node data
    /// </summary>
    public class NodeData: IndexedObject
    {
        #region NESTED

        public class IO: IndexedObject
        {
            #region NODE TYPES

            public enum IOType { LINK, PARAM };

            public enum VarType { NUM, BOOL, OBJECT, ARRAY, UNDEF };

            /// <summary>
            /// Base class for mutliple types, either a link between the two
            /// nodes or 
            /// </summary>
            public IOType Type;

            /// <summary>
            /// If IO os param, define the type of parameter the 
            /// IO is
            /// </summary>
            public VarType Var;

            #endregion

            #region REFERENCE

            /// <summary>
            /// ID of other nodes that this connects to
            /// </summary>
            public int LinkID;

            /// <summary>
            /// ID of the node that this IO object belongs to
            /// </summary>
            public int NodeID;

            #region GROUPS

            /// <summary>
            /// The group that this node belongs to, allows for 
            /// mass duplication or deletion
            /// </summary>
            public int GroupID;

            /// <summary>
            /// When a group is duplicated, this ID determines the Instance ID of the 
            /// copy
            /// </summary>
            public int GroupInstanceID;

            /// <summary>
            /// When assigned, this node will duplicate all nodes
            /// of this group ID defined here
            /// </summary>
            public int GroupCreateID;

            /// <summary>
            /// When unassigned and this ID is not the highest group instance ID
            /// </summary>
            public int GroupRemoveID;

            #endregion

            #endregion

            /// <summary>
            /// Display the label on a graphic
            /// </summary>
            public string Label;

            /// <summary>
            /// Returns a cloned IO object
            /// with the same parameters
            /// </summary>
            /// <returns></returns>
            public IO Clone()
            {
                IO clone = new IO();
                clone.ID = ID;
                clone.Label = Label;

                clone.Type = Type;
                clone.Var = Var;
                clone.LinkID = LinkID;
                clone.NodeID = NodeID;

                clone.GroupID = GroupID;
                clone.GroupInstanceID = GroupInstanceID;
                clone.GroupCreateID = GroupCreateID;
                clone.GroupRemoveID = GroupRemoveID;

                return clone;
            }
        }

        #endregion

        #region ATTRIBUTES

        /// <summary>
        /// Conditional, sequence, etc..
        /// </summary>
        public string Category;

        /// <summary>
        /// When generated, created instance ID?
        /// </summary>
        public int InstanceID;

        /// <summary>
        /// The display name of the graphic and the identifier
        /// </summary>
        public string Label;

        /// <summary>
        /// If an object 
        /// </summary>
        public List<string> SupportedTypes = new List<string>();

        /// <summary>
        /// links and params that travel into the node
        /// </summary>
        private IndexedList<IO> m_in = new IndexedList<IO>();

        /// <summary>
        /// links and params that travel out of the object
        /// </summary>
        private IndexedList<IO> m_out = new IndexedList<IO>();

        public string Description = "";

        /// <summary>
        /// The script object that is generated by the params and 
        /// settings, generated and returned when called
        /// </summary>
        public string CodeSnippet
        {
            get
            {
                return "";
            }
        }

        #endregion

        #region ACCESSORS

        /// <summary>
        /// The input parameters for the Node
        /// </summary>
        public IndexedList<IO> Input
        {
            get
            {
                return m_in;
            }
        }

        /// <summary>
        /// The output paramters of the Node
        /// </summary>
        public IndexedList<IO> Output
        {
            get
            {
                return m_out;
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Adds an IO object to the lists
        /// and assigns the data that is relevent
        /// </summary>
        /// <param name="xmlIO"></param>
        /// <param name="groupID"></param>
        public void AddIO(XmlNode xmlIO, int groupID)
        {
            // Initialze base data 
            NodeData.IO io = new NodeData.IO();
            io.Label = xmlIO.Attributes["label"].Value;
            io.NodeID = ID;

            // grouping assignment and group editing
            io.GroupID = groupID;

            if(xmlIO.Attributes["createId"] != null)
                io.GroupCreateID = Convert.ToInt32(xmlIO.Attributes["createId"].Value);

            // Initialize data depending on IO type
            switch (xmlIO.Name)
            {
                case "link":
                    io.Type = NodeData.IO.IOType.LINK;
                    break;
                case "param":

                    io.Type = NodeData.IO.IOType.PARAM;

                    switch (xmlIO.Attributes["type"].Value)
                    {
                        case "int":
                            io.Var = NodeData.IO.VarType.NUM;
                            break;
                        case "bool":
                            io.Var = NodeData.IO.VarType.BOOL;
                            break;
                        case "object":
                            io.Var = NodeData.IO.VarType.OBJECT;
                            break;
                        case "objectarray":
                            io.Var = NodeData.IO.VarType.ARRAY;
                            break;
                        case "undef":
                            io.Var = NodeData.IO.VarType.UNDEF;
                            break;
                    }
                    break;
            }

            if (xmlIO.Attributes["in"].Value == "true")
                m_in.Add(io);
            else
                m_out.Add(io);
        }

        /// <summary>
        /// Adds the reference to the node and,
        /// if applicable, increment a group object
        /// </summary>
        /// <param name="other"></param>
        public void AssignToNode(int ID, NodeData other)
        {
            m_in[ID].LinkID = other.InstanceID;
            
            if(m_in[ID].GroupCreateID != -1)
            {
                int maxGroupID = MaxGroupInstance(m_in[ID].GroupCreateID);

                foreach (IO io in m_in.FindAll(x => x.GroupID == m_in[ID].GroupID))
                {
                    // if this is the latest instance, create a copy
                    if (io.GroupInstanceID == maxGroupID)
                    {
                        // Add this copy to list
                        m_in.Add(CreateGroupCopy(maxGroupID + 1, io));
                    }
                    // add the reference to the id to the input param
                    m_in[ID].GroupRemoveID = maxGroupID + 1;
                }

                foreach (IO io in m_out.FindAll(x => x.GroupID == m_out[ID].GroupID))
                {
                    // if this is the latest instance, create a copy
                    if (io.GroupInstanceID == maxGroupID)
                    {
                        // Add this copy to list
                        m_out.Add(CreateGroupCopy(maxGroupID + 1, io));
                    }

                    // add the reference to the id to the input param
                    m_in[ID].GroupRemoveID = maxGroupID + 1;
                }
            }
        }

        /// <summary>
        /// Remove node refernce to other, if applicable, delete
        /// group instance and update all other group instances
        /// </summary>
        /// <param name="ID"></param>
        public void DereferenceNode(int ID)
        {

        }

        /// <summary>
        /// Creates a new DataNode with the same input 
        /// and output abilities
        /// </summary>
        /// <returns></returns>
        public NodeData Clone()
        {
            NodeData clone = new NodeData();
            clone.Category = Category;
            clone.Label = Label;
            clone.SupportedTypes.AddRange(SupportedTypes);
            clone.Description = Description;

            foreach(IO input in m_in)
            {
                clone.m_in.Add(input);
            }
            foreach(IO output in m_out)
            {
                clone.m_out.Add(output);
            }

            return clone;
        }

        #endregion

        #region PRIVATE UTILITIES

        #region GROUP

        /// <summary>
        /// Returns the highest instance ID of a given group
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        private int MaxGroupInstance(int groupID)
        {
            int maxGroup = -1;

            foreach(IO io in m_in.FindAll(x => x.GroupID == groupID))
            {
                if (maxGroup < io.GroupInstanceID)
                    maxGroup = io.GroupInstanceID;
            }

            foreach (IO io in m_out.FindAll(x => x.GroupID == groupID))
            {
                if (maxGroup < io.GroupInstanceID)
                    maxGroup = io.GroupInstanceID;
            }

            return maxGroup;
        }

        /// <summary>
        /// Creates and returns a copy of the given IO object
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="io"></param>
        /// <returns></returns>
        private IO CreateGroupCopy(int groupID, IO io)
        {
            IO copy = new IO();
            copy.Label = io.Label;
            copy.NodeID = io.NodeID;
            copy.GroupID = io.GroupID;
            copy.GroupInstanceID = groupID;
            copy.Type = io.Type;
            copy.Var = io.Var;

            return copy;
        }

        private void DecrementGroupIDs(int groupID)
        {

        }

        #endregion

        #endregion
    }
}