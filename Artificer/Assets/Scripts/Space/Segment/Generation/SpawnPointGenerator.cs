using UnityEngine;
using System.Collections;

using Data.Shared;
using Data.Space;

namespace Space.Segment.Generator
{
    public class SpawnPointGenerator : MonoBehaviour
    {
        /*public GameObject GenerateSpawn(SpawnPointData spawn)
        {
            // Creates a game object and build attributes based on spawn type
            GameObject newSpawnPoint = new GameObject();
            newSpawnPoint.transform.parent = this.transform;
            newSpawnPoint.transform.position = spawn.Position;
            newSpawnPoint.name = spawn.SpawnType;

            switch (spawn.SpawnType)
            {
                case "asteroidSpawn":
                {
                    AsteroidSpawner AS = newSpawnPoint.AddComponent<AsteroidSpawner>();
                    AS.SpawnDelay = Random.Range(15f, 35f);
                    AS.Engage();
                    break;
                }
                case "friendlySpawn":
                {
                    newSpawnPoint.tag = "FriendlySpawn";
                    FriendlySpawner FS = newSpawnPoint.AddComponent<FriendlySpawner>();
                    FS.SpawnDelay = 0f;
                    break;
                }
                case "enemySpawn":
                {
                    newSpawnPoint.tag = "EnemySpawn";
                    EnemySpawner ES = newSpawnPoint.AddComponent<EnemySpawner>();
                    ES.SpawnDelay = 0f;
                    break;
                }
                case "asteroidSpawnFollow":
                {
                    AsteroidSpawner AS = newSpawnPoint.AddComponent<AsteroidSpawner>();
                    AS.SpawnDelay = Random.Range(15f, 35f);
                    newSpawnPoint.AddComponent<SpawnerFollowBehaviour>();
                    AS.Engage();
                    break;
                }
                case "playerSpawnFollow":
                {
                    newSpawnPoint.tag = "TeamSpawner";
                    PlayerSpawner PS = newSpawnPoint.AddComponent<PlayerSpawner>();
                    newSpawnPoint.AddComponent<SpawnerFollowBehaviour>();
                    PS.SpawnDelay = 5f;
                    break;
                }
                case "friendlySpawnFollow":
                {
                    newSpawnPoint.tag = "FriendlySpawn";
                    FriendlySpawner FS = newSpawnPoint.AddComponent<FriendlySpawner>();
                    newSpawnPoint.AddComponent<SpawnerFollowBehaviour>();
                    FS.SpawnDelay = 0f;
                    break;
                }
                case "enemySpawnFollow":
                {
                    newSpawnPoint.tag = "EnemySpawn";
                    EnemySpawner ES = newSpawnPoint.AddComponent<EnemySpawner>();
                    newSpawnPoint.AddComponent<SpawnerFollowBehaviour>();
                    ES.SpawnDelay = 0f;
                    break;
                }
            }

            return newSpawnPoint;
        }*/
    }
}

