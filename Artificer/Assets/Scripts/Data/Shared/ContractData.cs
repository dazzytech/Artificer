using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Contract type.
/// used by generator to determine segment structure
/// </summary>
public enum ContractType {BaseDefense, Escort, Targets, Attrition}

namespace Data.Shared
{
    /// <summary>
    /// Wave data.
    /// Stores ship data for wave
    /// 
    /// </summary>
    public class WaveData
    {
        public ShipData[] Ships;
        public float waveTime;
        public int waveLimit;
        public ShipData Boss;
    }

    /// <summary>
    /// Contract data.
    /// Acts as a game mode for Artificer 
    /// contains data about 
    /// </summary>
    public class ContractData
    {
        public ContractData()
        {
            //PrimaryMissions = new List<MissionData>();
            //OptionalMissions = new List<MissionData>();
            Locked = false;
        }

        // Store Primary Missions
        //public List<MissionData> PrimaryMissions;
        // Store Optional Missions
        //public List<MissionData> OptionalMissions;

        // Type
        public ContractType CType;

        // Accessible
        public bool Locked;

        // Unique ID
        public int ID;

        public int Requirement;

        // Will store other attributes here
        public string Name;
        public string Description;

        // Ship waves
        public List<WaveData> FriendlyWaves;
        public List<WaveData> EnemyWaves;
        public List<ShipData> MissionSpecific;
    }
}