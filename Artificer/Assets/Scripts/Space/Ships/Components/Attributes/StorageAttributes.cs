using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;

namespace Space.Ship.Components.Attributes
{
    /// <summary>
    /// storage component can store items 
    /// e.g. elements
    /// </summary>
    public class StorageAttributes: ComponentAttributes
    {
        public Dictionary<int, float> storage;                // <itemID, amount (kg)>

        public float dimensions;                                       // ft^3 (cubic foot)

        public float currentCapacity;
        public float currentWeight;
    }
}
