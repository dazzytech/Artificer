using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Data.Space;
using Data.Shared;
using Space.UI;
using Space.Segment;

/// <summary>
/// Enemy spawn manager.
/// keeps track enemy count etc 
/// </summary>
namespace Space.Contract
{
    public class EnemySpawnManager
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
        private float SpawnDelay = 15f;

        // Enemy Vars
        public static Transform Target;

        // Store list of segments object currently active
        public static List<Transform> Ships;

        public List<SpawnerBehaviour> Points;

        public EnemySpawnManager(GameParameters param)
        {
            ShipExternalController.ShipDestroyed += ShipDestroyed;

            //Waves = param.Contract.EnemyWaves;

            Points = new List<SpawnerBehaviour>();

            Ships = new List<Transform>();

            WaveNumber = -1;
            WaveTimer = 0;
            Wave = null;

             (param.Contract.CType == ContractType.Targets)
            {
                BuildTargetShips(param.Contract);
            }

            if (param.Contract.CType != ContractType.Escort)
            {
                BulkSpawn();
            }
        }

        ~EnemySpawnManager()
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

        private void BuildTargetShips(ContractData contract)
        {
            GetSpawnList();
            
            foreach(ShipData Ship in (contract.MissionSpecific))
            {
                EnemySpawner spawn = Points [Random.Range(0, Points.Count)] as EnemySpawner;
                spawn.ShipPending = Ship;
                spawn.Trigger();

                // In theory, by this point the latest transform should be our new target
                foreach (MissionData m in contract.PrimaryMissions)
                    if(m is TargetMission)
                    {
                        TargetMission t = m as TargetMission;
                        t.Targets.Add(Ships[Ships.Count-1]);
                    }
                foreach (MissionData m in contract.OptionalMissions)
                    if(m is TargetMission)
                {
                    TargetMission t = m as TargetMission;
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

            // Set to popup gui
            MessageHUD.DisplayMessege(new MsgParam("bold", "Starting Wave: " + (WaveNumber+1).ToString()));
        }

        public void CycleEnemySpawn()
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
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("EnemySpawn"))
            {
                Points.Add(go.GetComponent<EnemySpawner>());
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
            
            EnemySpawner spawn = Points [Random.Range(0, Points.Count)] as EnemySpawner;
            spawn.ShipPending = Ship;
            spawn.Target = Target;
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
        }

        public static void AddShip(Transform newShip)
        {
            Ships.Add(newShip);
        }*/
    }
}

