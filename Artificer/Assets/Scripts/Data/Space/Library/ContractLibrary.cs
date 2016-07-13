using UnityEngine;
using System.Collections;
// Artificer
using Data.Shared;

namespace Data.Space.Library
{
    /// <summary>
    /// Contract library.
    /// stores a static list of contracts to
    /// be accessible anywhere.
    /// </summary>
    public class ContractLibrary 
    {
        static ContractData[] _data;
        
        public void AssignData(ContractData[] data)
        {
            _data = data;
        }

        public static bool ContractExists()
        {
            return _data != null;
        }
        
        /// <summary>
        /// Returns the element.
        /// use when you know which element you want
        /// </summary>
        /// <returns>The element.</returns>
        /// <param name="element">Element.</param>
        public static ContractData ReturnContract(int contact)
        {
            if (_data.Length == 0)
                return null;
            
            foreach (ContractData con in _data)
                if (con.ID == contact)
                    return con;
            
            return null;
        }
        
        /// <summary>
        /// Returns an element completely at random.
        /// </summary>
        /// <returns>The random element.</returns>
        public static ContractData[] ReturnContractList()
        {
            if (_data.Length == 0)
                return null;
            
            return _data;
        }
    }
}