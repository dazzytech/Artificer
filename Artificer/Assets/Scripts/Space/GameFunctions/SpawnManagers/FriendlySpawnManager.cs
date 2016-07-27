using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Data.Space;
using Data.Shared;
using Space.UI;
using Space.Generator;

namespace Space.Contract
{
    public class FriendlySpawnManager 
    {
        /*public ShipGenerator ShipGen;
        
        // wave vars
        public List<WaveData> Waves;
        private int WaveNumber;
        private WaveData Wave;
        public float WaveTimer;
        
        // Spawn vars
        public int Max;
        private float SpawnTimer = 0;
        private float SpawnDelay = 30f;
        
        // Friendly Vars
        public ContractType CType;
        
        // Store list of segments object currently active
        public static List<Transform> Ships;
        
        public List<SpawnerBehaviour> Points;

        // mission specific
        public static Transform Travel;
        public List<Transform> Follow;
        
        public FriendlySpawnManager(GameParameters param)
        {
            ShipExternalController.ShipDestroyed += ShipDestroyed;
            
            /*Waves = param.Contract.FriendlyWaves;
            CType = param.Contract.CType;
            
            Points = new List<SpawnerBehaviour>();
            Ships = new List<Transform>();
            Follow = new List<Transform>();
            
            WaveNumber = -1;
            WaveTimer = 0;
            Wave = null;

            if(CType == ContractType.Escort)
                BuildEscortShips(param.Contract);
            
            BulkSpawn();
        }
        
        ~FriendlySpawnManager()
        {
            ShipExternalController.ShipDestroyed -= ShipDestroyed;
        }
        
        private void BulkSpawn()
        {
            BuildWave(WaveNumber + 1);
            
            GetSpawnList();
            
            while (Ships.Count < Max)
            {
                SpawnShip(true);
            }
        }

        private void BuildEscortShips(ContractData contract)
        {
            GetSpawnList();
            
            foreach(ShipData Ship in (contract.MissionSpecific))
            {
                FriendlySpawner spawn = Points [Random.Range(0, Points.Count)] as FriendlySpawner;
                spawn.ShipPending = Ship;
                spawn.SpawnType = "cargo";
                spawn.Target = Travel;
                spawn.Trigger();

                // Set first cargo to be followed ship
                Follow.Add(Ships[Ships.Count-1]);
                if(Follow.Count == 1)
                {
                    foreach (GameObject go in GameObject.FindGameObjectsWithTag("FriendlySpawn"))
                    {
                        go.GetComponent<SpawnerFollowBehaviour>().SetFollow(Follow[Random.Range(0, Follow.Count)], 10, 20);
                    }
                    foreach (GameObject go in GameObject.FindGameObjectsWithTag("EnemySpawn"))
                    {
                        go.GetComponent<SpawnerFollowBehaviour>().SetFollow(Follow[Random.Range(0, Follow.Count)], 20, 50);
                    }
                    foreach (GameObject go in GameObject.FindGameObjectsWithTag("TeamSpawner"))
                    {
                        go.GetComponent<SpawnerFollowBehaviour>().SetFollow(Follow[Random.Range(0, Follow.Count)], 10, 20);
                    }
                }

                // In theory, by this point the latest transform should be our new target
                foreach (MissionData m in contract.PrimaryMissions)
                    if(m is EscortMission)
                {
                    EscortMission t = m as EscortMission;
                    t.Targets.Add(Ships[Ships.Count-1]);
                }
                foreach (MissionData m in contract.OptionalMissions)
                    if(m is EscortMission)
                {
                    EscortMission t = m as EscortMission;
                    t.Targets.Add(Ships[Ships.Count-1]);
                }
            }   
        }
        
        private void BuildWave(int wave)
        {
            if (Waves == null)
                return;
            if (wave >= Waves.Count)
                return;
            
            WaveNumber = wave;
            Wave = Waves [WaveNumber];
            WaveTimer = Waves [WaveNumber].waveTime;
            Max = Waves [WaveNumber].waveLimit;
        }
        
        public void CycleFriendlySpawn()
        {
            GetSpawnList();
            
            // cycle the wave
            WaveTimer -= Time.deltaTime;
            if (WaveTimer <= 0)
            {
                BuildWave(WaveNumber + 1);
            }
            
            // cycle enemy spawns
            if (Ships.Count < Max)
            {
                SpawnTimer -= Time.deltaTime;
                if (SpawnTimer <= 0f)
                {
                    SpawnShip(false);
                }
            }
        }
        
        private void GetSpawnList()
        {
            // Refresh list of enemyspawns
            Points.Clear();
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("FriendlySpawn"))
            {
                Points.Add(go.GetComponent<FriendlySpawner>());
            }
        }
        
        private void SpawnShip(bool bulk)
        {
            // Pick out a ship to spawn
            ShipData Ship = null;
            
            if (Wave.Boss == null)
                Ship = Wave.Ships [Random.Range(0, Wave.Ships.Length)];
            else
            {
                Ship = Wave.Boss;
                Wave.Boss = null;
            }

            FriendlySpawner spawn = Points [Random.Range(0, Points.Count)] as FriendlySpawner;
            spawn.ShipPending = Ship;

            switch (CType)
            {
                case ContractType.BaseDefense:
                    spawn.SpawnType = "guard";
                    break;
                case ContractType.Attrition:
                    spawn.SpawnType = "";
                    break;
                case ContractType.Escort:
                    spawn.SpawnType = "";
                    spawn.Target = Follow[Random.Range(0, Follow.Count)];
                    break;
                default:
                    spawn.SpawnType = "";
                    break;
            }

            if (bulk)
                spawn.Trigger();
            else
                spawn.Engage();
            SpawnTimer = SpawnDelay;
        }
        
        // EVENTS
        public void ShipDestroyed(DestroyDespatch param)
        {
            if (Ships.Contains(param.Self))
            {
                Ships.Remove(param.Self);
            }
            if (Follow.Contains(param.Self))
            {
                Follow.Remove(param.Self);
                if(Follow.Count == 0)
                {
                    foreach (GameObject go in GameObject.FindGameObjectsWithTag("FriendlySpawn"))
                    {
                        GameObject.Destroy(go);
                    }
                    foreach (GameObject go in GameObject.FindGameObjectsWithTag("EnemySpawn"))
                    {
                        GameObject.Destroy(go);
                    }
                    foreach (GameObject go in GameObject.FindGameObjectsWithTag("TeamSpawner"))
                    {
                        GameObject.Destroy(go);
                    }

                    return;
                }


                foreach (GameObject go in GameObject.FindGameObjectsWithTag("FriendlySpawn"))
                {
                    go.GetComponent<SpawnerFollowBehaviour>().SetFollow(Follow[Random.Range(0, Follow.Count)], 10, 50);
                }
                foreach (GameObject go in GameObject.FindGameObjectsWithTag("EnemySpawn"))
                {
                    go.GetComponent<SpawnerFollowBehaviour>().SetFollow(Follow[Random.Range(0, Follow.Count)], 100, 200);
                }
                foreach (GameObject go in GameObject.FindGameObjectsWithTag("TeamSpawner"))
                {
                    go.GetComponent<SpawnerFollowBehaviour>().SetFollow(Follow[Random.Range(0, Follow.Count)], 10, 20);
                }
            }
        }
        
        public static void AddShip(Transform newShip)
        {
            Ships.Add(newShip);
        }*/
    }
}

