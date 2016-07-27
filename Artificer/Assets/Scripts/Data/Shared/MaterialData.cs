using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// Material data.
/// Stores material as 
/// Material 
/// </summary>
namespace Data.Shared
{
    public struct MaterialData
    {
        public string Element;         // e.g. "Au"
        public string Name;            // e.g. "Gold"
        public string Description;      // e.g. "Conductive unresponsive metal
        public float Density;            // 547 kg per cubic foot (ft^3)
        public float Amount;            // Amounts owned
    }
}
