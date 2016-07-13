using UnityEngine;
using System.Collections;
// Artificer
using Data.Shared;

namespace Data.Space.Library
{
    /// <summary>
    /// Prospect chart.
    /// Contains a list of possible
    /// material yield symbols
    /// The number of iterations within the
    /// array determines the chance of yield
    /// </summary>
    [System.Serializable]
    public class ProspectChart
    {
        public string[] symbol;
    }
    /// <summary>
    /// Element library.
    /// Stores each element 
    /// that exists in game
    /// </summary>
    public class ElementLibrary
    {
        static MaterialData[] _data;

        public void AssignData(MaterialData[] data)
        {
            _data = data;
        }

        /// <summary>
        /// Returns the element.
        /// use when you know which element you want
        /// </summary>
        /// <returns>The element.</returns>
        /// <param name="element">Element.</param>
        public static MaterialData ReturnElement(string element)
        {
            if (_data.Length == 0)
                return null;

            foreach (MaterialData mat in _data)
                if (mat.Element == element)
                    return mat;

            return null;
        }

        /// <summary>
        /// Returns an element completely at random.
        /// </summary>
        /// <returns>The random element.</returns>
        public static MaterialData ReturnRandomElement()
        {
            if (_data.Length == 0)
                return null;

            // Get element
            int index = Random.Range(0, _data.Length);

            return _data[index];
        }


        public static MaterialData 
            ReturnRandomElementWeighted
                (string[] prospect)
        {
            if (_data.Length == 0)
                return null;

            string element = prospect
                [Random.Range(0, prospect.Length)];

            foreach (MaterialData mat in _data)
                if (mat.Element == element)
                    return mat;

            return null;
        }
    }
}
