using UnityEngine;
using System.Collections;

namespace Data.Space.Library
{
    public class FactionLibrary
    {
        static FactionData[] _data;

        public void AssignData(FactionData[] data)
        {
            _data = data;
        }

        /// <summary>
        /// Returns the element.
        /// use when you know which element you want
        /// </summary>
        /// <returns>The element.</returns>
        /// <param name="element">Element.</param>
        public static FactionData ReturnFaction(int id)
        {
            if (_data.Length == 0)
                return new FactionData();

            foreach (FactionData mat in _data)
                if (mat.ID == id)
                    return mat;

            return new FactionData();
        }
    }
}
