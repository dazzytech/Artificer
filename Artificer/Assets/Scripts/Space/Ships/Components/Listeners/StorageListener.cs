using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;
using Space.UI;
using Space.Ship.Components.Attributes;
using Data.Space.Collectable;

namespace Space.Ship.Components.Listener
{
    /// <summary>
    /// Storage listener.
    /// additional listener
    /// </summary>
    public class StorageListener : ComponentListener
    {
        StorageAttributes _attr;
        float totalWeight;

        void Awake()
        {
            ComponentType = "Storage";
            _attr = GetComponent<StorageAttributes>();
            _attr.storage = new Dictionary<int, float>();
            totalWeight = 0;
        }

        // Use this for initialization
        void Start()
        {
            SetRB();
        }

        /// <summary>
        /// Adds the item to the storage attributes.
        /// </summary>
        /// <returns>The material remainder.</returns>
        /// <param name="import">Import.</param>
        public Dictionary<int, float> AddMaterial(Dictionary<int, float> import)
        {
            if (_attr.storage == null)
                _attr.storage = new Dictionary<int, float>();

            Dictionary<int, float> remainder
                = new Dictionary<int, float>();

            // Create new Dictionary for mission update
            Dictionary<int, float> track = new Dictionary<int, float>();

            foreach (int id in import.Keys)
            {
                float totalDimension = import[id] + _attr.currentCapacity;
                float dimensionRemainder =                               // overspill of capacity
                    Mathf.Max(totalDimension - _attr.dimensions, 0);        

                float valueToAdd = import[id] - dimensionRemainder;

                // retrieve information
                ItemData item = SystemManager.Items.Item(id);

                _attr.currentWeight += valueToAdd * item.Density;
                _attr.currentCapacity += valueToAdd;

                if(_attr.storage.ContainsKey(id))
                    _attr.storage[id] += valueToAdd;                   // add amount to current supply
                else if(valueToAdd > 0)
                    _attr.storage.Add(id, valueToAdd);

                // update tracker resource
                track.Add(id, import[id] - (dimensionRemainder));

                if( dimensionRemainder != 0)
                {
                    remainder.Add(id, dimensionRemainder);
                }
            }

            SetShipWeight();

            return remainder;
        }

        /// <summary>
        /// Ejects the material.
        /// Removes amount specified and then
        /// returns that exact amount for creating 
        /// a lootable object
        /// </summary>
        /// <returns>The material.</returns>
        /// <param name="import">Import.</param>
        public Dictionary<int, float> EjectMaterial(Dictionary<int, float> import)
        {
            if (_attr.storage == null)
                return null;

            Dictionary<int, float> ejectedMat = new Dictionary<int, float>();

            foreach (int id in import.Keys)
            {
                if (_attr.storage.ContainsKey(id))
                {
                    float amountToRemove = import[id];
                    float spillOver = Mathf.Max(amountToRemove - _attr.storage[id], 0);
                    amountToRemove -= spillOver;

                    _attr.storage[id] -= amountToRemove;

                    if(_attr.storage[id] <= 0.001f)
                        _attr.storage.Remove(id);

                    // retrieve information
                    ItemData item = SystemManager.Items.Item(id);


                    // remove weight
                    _attr.currentWeight -= amountToRemove * item.Density;
                    _attr.currentCapacity -= amountToRemove;

                    if(spillOver >= 0)
                        ejectedMat.Add(id, spillOver);
                }
            }
            return ejectedMat;
        }

        private void SetShipWeight()
        {
            if(rb == null)
                SetRB();

            rb.mass -= totalWeight;
            totalWeight = _attr.currentWeight * 0.00001f;
            rb.mass += totalWeight;
        }

        public new ComponentAttributes GetAttributes()
        {
            return _attr;
        }
    }
}