using UnityEngine;
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

        public class IO : IndexedObject
        {
            #region NODE TYPES

            public enum IOType { UNDEF, UNDEFSINGLE, NUM, STRING, BOOL, VEC2, OBJECT, NUMARRAY, STRINGARRAY, OBJARRAY, VEC2ARRAY, LINK };

            /// <summary>
            /// If IO os param, define the type of parameter the 
            /// IO is
            /// </summary>
            public IOType Type;

            /// <summary>
            /// If an undef IO type, this subtype 
            /// </summary>
            public IOType TempVar;

            /// <summary>
            /// If undefined, then return the temporarily assigned var
            /// </summary>
            public IOType CurrentType
            {
                get
                {
                    if (TempVar == IOType.UNDEF)
                        return Type;
                    else
                        return TempVar;
                }
            }

            #endregion

            #region REFERENCE

            /// <summary>
            /// ID of other nodes that this connects to
            /// </summary>
            public IO LinkedIO;

            /// <summary>
            /// ID of the node that this IO object belongs to
            /// </summary>
            public NodeData Node;

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

            #endregion

            #endregion

            /// <summary>
            /// Display the label on a graphic
            /// </summary>
            public string Label;

            /// <summary>
            /// Params may allow for a value to be entered directly into the 
            /// link
            /// </summary>
            public string Value;

            /// <summary>
            /// Returns the value of this node or the linked
            /// nodes
            /// </summary>
            public string GetValue
            {
                get
                {
                    if (LinkedIO == null)
                        return Value;
                    else
                        return LinkedIO.Value;
                }
            }

            public System.Type IOGetType
            {
                get
                {
                    switch (CurrentType)
                    {
                        case IOType.NUM:
                        case IOType.NUMARRAY:
                            return typeof(float);
                        case IOType.STRING:
                        case IOType.STRINGARRAY:
                            return typeof(string);
                        case IOType.VEC2:
                        case IOType.VEC2ARRAY:
                            return typeof(Vector2);
                        case IOType.OBJECT:
                        case IOType.OBJARRAY:
                            return typeof(IDEObjectData);
                        default:
                            return typeof(void);
                    }
                }
            }

            /// <summary>
            /// Returns a cloned IO object
            /// with the same parameters
            /// </summary>
            /// <returns></returns>
            public IO Clone(NodeData cloneNode)
            {
                IO clone = new IO();
                clone.ID = ID;
                clone.Label = Label;

                clone.Type = Type;
                clone.Type = Type;
                clone.LinkedIO = LinkedIO;
                clone.Node = cloneNode;
                clone.Value = Value;

                clone.GroupID = GroupID;
                clone.GroupInstanceID = GroupInstanceID;
                clone.GroupCreateID = GroupCreateID;
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
        public List<NodeData.IO.IOType> SupportedTypes = new List<NodeData.IO.IOType>();

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
            io.Node = this;
            io.LinkedIO = null;

            // grouping assignment and group editing
            io.GroupID = groupID;

            if (xmlIO.Attributes["createID"] != null)
                io.GroupCreateID = Convert.ToInt32(xmlIO.Attributes["createID"].Value);
            else
                io.GroupCreateID = -1;

            // Initialize data depending on IO type
            switch (xmlIO.Name)
            {
                case "link":
                    io.Type = NodeData.IO.IOType.LINK;
                    break;
                case "param":
                    switch (xmlIO.Attributes["type"].Value)
                    {
                        case "number":
                            io.Type = IO.IOType.NUM;
                            break;
                        case "string":
                            io.Type = IO.IOType.STRING;
                            break;
                        case "bool":
                            io.Type = IO.IOType.BOOL;
                            break;
                        case "vec2":
                            io.Type = IO.IOType.VEC2;
                            break;
                        case "object":
                            io.Type = IO.IOType.OBJECT;
                            break;
                        case "objectarray":
                            io.Type = IO.IOType.OBJARRAY;
                            break;
                        case "numarray":
                            io.Type = NodeData.IO.IOType.NUMARRAY;
                            break;
                        case "stringarray":
                            io.Type = NodeData.IO.IOType.STRINGARRAY;
                            break;
                        case "vec2array":
                            io.Type = IO.IOType.VEC2ARRAY;
                            break;
                        case "undef":
                            io.Type = IO.IOType.UNDEF;
                            break;
                        case "undefsingle":
                            io.Type = IO.IOType.UNDEFSINGLE;
                            break;
                    }
                    break;
            }

            if (xmlIO.Attributes["value"] != null)
                    io.Value = xmlIO.Attributes["value"].Value;

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
        public void AssignToNode(IO self, IO other)
        {
            self.LinkedIO = other;
            
            if(self.GroupCreateID != -1)
            {
                int maxGroupID = MaxGroupInstance(self.GroupCreateID);

                foreach (IO io in m_in.FindAll(x => x.GroupID == self.GroupID))
                {
                    // if this is the latest instance, create a copy
                    if (io.GroupInstanceID == maxGroupID)
                    {
                        // Add this copy to list
                        m_in.Add(CreateGroupCopy(maxGroupID + 1, io));
                    }
                }

                foreach (IO io in m_out.FindAll(x => x.GroupID == self.GroupID))
                {
                    // if this is the latest instance, create a copy
                    if (io.GroupInstanceID == maxGroupID)
                    {
                        // Add this copy to list
                        m_out.Add(CreateGroupCopy(maxGroupID + 1, io));
                    }
                }
            }
        }

        /// <summary>
        /// Remove node refernce to other, if applicable, delete
        /// group instance and update all other group instances
        /// </summary>
        /// <param name="ID"></param>
        public void DereferenceNode(IO self)
        {
            self.LinkedIO = null;

            List<IO> delete = new List<IO>();

            if (self.GroupInstanceID != MaxGroupInstance(self.GroupCreateID))
            {
                foreach (IO io in m_in.FindAll(x => x.GroupID == self.GroupID))
                {
                    // if this is the latest instance, delete
                    if (io.GroupInstanceID == self.GroupInstanceID)
                    {
                        delete.Add(io);
                    }
                }

                foreach (IO io in m_out.FindAll(x => x.GroupID == self.GroupID))
                {
                    // if this is the latest instance, create a copy
                    if (io.GroupInstanceID == self.GroupInstanceID)
                    {
                        delete.Add(io);
                    }
                }
            }

            foreach(IO io in delete)
            {
                m_in.Remove(io);
                m_out.Remove(io);
            }
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
                clone.m_in.Add(input.Clone(clone));
            }
            foreach(IO output in m_out)
            {
                clone.m_out.Add(output.Clone(clone));
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
            copy.Node = io.Node;
            copy.GroupID = io.GroupID;
            copy.GroupInstanceID = groupID;
            copy.GroupCreateID = io.GroupCreateID;
            copy.Type = io.Type;
            copy.Type = io.Type;

            return copy;
        }

        private void DecrementGroupIDs(int groupID)
        {

        }

        #endregion

        #endregion
    }
}