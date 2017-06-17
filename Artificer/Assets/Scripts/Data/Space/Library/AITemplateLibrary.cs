using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Data.Space.Library
{
    public class AITemplateLibrary : IndexedList<AITemplateData>
    {
        #region ACCESSORS

        /// <summary>
        /// Retrieves itemID for an item
        /// if name doesn't match ant then -1 is returned;
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetID(string type)
        {
            // Find our ID within list and remove it
            for (int i = 0; i < base.Count; i++)
            {
                if (type == (base[i]).Type)
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion
    }
}