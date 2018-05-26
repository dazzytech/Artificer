using UnityEngine;
using System.Collections;
using Data.UI;
using System.Collections.Generic;

using System.Linq;

namespace Data.Space.Library
{
    /// <summary>
    /// stores the prefab node that can be used 
    /// to create a code snippet
    /// </summary>
    public class NodeLibrary : IndexedList<NodeData>
    {
        /// <summary>
        /// The list of categories make up the tags in ui
        /// </summary>
        public List<string> Categories = new List<string>();

        #region ACCESSORS

        /// <summary>
        /// Retrieves id for an node Prefab
        /// if label doesn't match then -1 is returned;
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetID(string label)
        {
            // Find our ID within list and remove it
            for (int i = 0; i < base.Count; i++)
            {
                if (label == (base[i]).Label)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// returns all nodes of the category type
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<NodeData> GetCategory(string category)
        {
            return base.FindAll(x => x.Category == category);
        }

        #endregion
    }
}