using UnityEngine;
using System.Collections;
// Artificer
using Data.Space.Collectable;

namespace Data.Space.Library
{
    /// <summary>
    /// Element library.
    /// Stores each element 
    /// that exists in game
    /// </summary>
    public class ItemLibrary:IndexedList<ItemData>
    {
        #region ACCESSORS

        /// <summary>
        /// Retrieves itemID for an item
        /// if name doesn't match ant then -1 is returned;
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetID(string name)
        {
            // Find our ID within list and remove it
            for (int i = 0; i < base.Count; i++)
            {
                if (name == (base[i]).Name)
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion
    }
}
