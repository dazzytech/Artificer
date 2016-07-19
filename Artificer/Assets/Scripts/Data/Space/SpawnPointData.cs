using UnityEngine;
using System.Collections;

namespace Data.Space
{
    /// <summary>
    /// Base class for all spawn types
    /// </summary>
    public struct SpawnPointData 
    {
        /// <summary>
        /// What type of objects the spawner will spawn
        /// 1. Player Spawn, 2. Enemy Spawn, 3. Friendly Spawn, 4. Asteroid Spawn
        /// </summary>
        public string SpawnType;

        // Where the spawner is in space
        public Vector2 Position;
    }
}
