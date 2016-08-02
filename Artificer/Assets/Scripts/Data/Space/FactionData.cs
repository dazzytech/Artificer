using UnityEngine;
using System.Collections;

namespace Data.Space
{
    public struct FactionData
    {
        // Unique faction identifier
        public int ID;

        // Faction name
        public string Name;

        // Brief regarding the faction
        public string Description;

        // Visual component styles available to the faction
        public string[] Styles;

        //Faction logo
        public string Icon;
    }
}