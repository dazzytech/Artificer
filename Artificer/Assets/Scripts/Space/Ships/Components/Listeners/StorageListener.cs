using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Space;
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

        #region ATTRIBUTES

        StorageAttributes _attr;
        float totalWeight;

        #endregion

        #region ACCESSORS

        /// <summary>
        /// Total storage space
        /// </summary>
        public float Capacity
        {
            get { return _attr.dimensions; }
        }

        /// <summary>
        /// Space used by current materials
        /// </summary>
        public float Used
        {
            get { return _attr.currentCapacity; }
        }

        public Dictionary<int, float> Materials
        {
            get
            {
                return _attr.storage;
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Discovers if this item 
        /// currently stores the material
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool ContainsItem(int item)
        {
            if (_attr.storage == null)
                return false;

            else
                return _attr.storage.ContainsKey(item);
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

            foreach (int id in import.Keys)
            {
                float totalDimension = import[id] + _attr.currentCapacity;
                float dimensionRemainder =                               // overspill of capacity
                    Mathf.Max(totalDimension - _attr.dimensions, 0);        

                float valueToAdd = import[id] - dimensionRemainder;

                // retrieve information
                ItemData item = SystemManager.Items.Item(id);

                if (item == null)
                    continue;

                _attr.currentWeight += valueToAdd * item.Density;
                _attr.currentCapacity += valueToAdd;

                if(_attr.storage.ContainsKey(id))
                    _attr.storage[id] += valueToAdd;                   // add amount to current supply
                else if(valueToAdd > 0)
                    _attr.storage.Add(id, valueToAdd);

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

        #endregion

        #region PRIVATE UTILITIES

        protected override void InitializeComponent()
        {
            base.InitializeComponent();

            ComponentType = "Storage";

            _attr = GetComponent<StorageAttributes>();
            _attr.storage = new Dictionary<int, float>();

            if (hasAuthority)
                totalWeight = 0;
        }

        private void SetShipWeight()
        {
            rb.mass -= totalWeight;
            totalWeight = _attr.currentWeight * 0.000001f;
            rb.mass += totalWeight;
        }

        #endregion
    }
}