using UnityEngine;
using System.Collections;

namespace Data.Space
{
    [System.Serializable]
    public class SpawnPointData 
    {
        /// <summary>
        /// 1. Player Spawn, 2. Enemy Spawn, 3. Friendly Spawn, 4. Asteroid Spawn
        /// </summary>
        public string SpawnType;

        public SpawnPointData()
        {
            //seg.Type = "spawnpoint";
        }
    }
}
