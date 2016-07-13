using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System;

namespace Editor
{
    /// <summary>
    /// Contract object create a serializable object
    /// </summary>
    /// 
    public class ContractObject
    {
        public string ContractFile;
        public string ContractName;
        public string ContractDescription;
        public string ContractLevel;
        public string ContractType;

        // Store container for mission list
        public List<MissionItem> PrimaryMissions;
        
        // Store container for mission list
        public List<MissionItem> OptionalMissions;
        
        // Store container for all 3 types
        public List<WaveItem> EnemyWaves;
        
        public List<WaveItem> FriendlyWaves;
        
        public WaveItem MissionWave;

        public bool Save()
        {
            XmlTextWriter writer = null;
            // Begin writing document
            try
            {
                writer = new XmlTextWriter
                    ("Assets/Resources/Space/ContractLists/" + ContractFile + 
                    ".xml", System.Text.Encoding.UTF8);
            }
            catch(Exception e)
            {
                Debug.Log("Error - Contract Object: " + e.Message);
                          return false;
            }
           
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartElement("contract");
            // Create info data table
            CreateInfo(writer);
            // mission info
            CreateMissions(writer, true);
            CreateMissions(writer, false);

            writer.WriteStartElement("waves");
            CreateWave(writer, 0);
            CreateWave(writer, 1);
            CreateWave(writer, 2);
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();

            return true;
        }

        private void CreateInfo(XmlTextWriter writer)
        {
            // Create info block in xml
            writer.WriteStartElement("info");

            // Add name to info
            writer.WriteStartElement("name");
            writer.WriteString(ContractName);
            writer.WriteEndElement();

            writer.WriteStartElement("desc");
            writer.WriteString(ContractDescription);
            writer.WriteEndElement();

            writer.WriteStartElement("lvl");
            writer.WriteString(ContractLevel);
            writer.WriteEndElement();

            writer.WriteStartElement("ctype");
            writer.WriteString(ContractType);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        private void CreateMissions(XmlTextWriter writer, bool prim)
        {
            // Create info block in xml
            if(prim)
                writer.WriteStartElement("primary_missions");
            else
                writer.WriteStartElement("optional_missions");

            List<MissionItem> Missions = new List<MissionItem>
                (prim ? PrimaryMissions : OptionalMissions);

            foreach (MissionItem mItem in Missions)
            {
                // write mission info to xml
                writer.WriteStartElement("mission_data");
                writer.WriteAttributeString("type", mItem.type);

                // if timelimit exists add to xml
                if(mItem.timeLimit > 0)
                {
                    writer.WriteStartElement("time_limit");
                    writer.WriteString(mItem.timeLimit.ToString());
                    writer.WriteEndElement();
                }

                // if destroy count exist etc
                if(mItem.destroyGoal > 0)
                {
                    writer.WriteStartElement("goal");
                    writer.WriteString(mItem.destroyGoal.ToString());
                    writer.WriteEndElement();
                }

                if(mItem.type == "collect")
                {
                    foreach(string sym in mItem.goalMats.Keys)
                    {
                        writer.WriteStartElement("mmats");
                        writer.WriteAttributeString("sym", sym);
                        writer.WriteString(mItem.goalMats[sym].ToString());
                        writer.WriteEndElement();
                    }
                }

                // Draw reward info to xml data
                writer.WriteStartElement("reward");
                // draw material items to xml
                foreach(string sym in mItem.reward.mats.Keys)
                {
                    writer.WriteStartElement("mat");
                    writer.WriteAttributeString("sym", sym);
                    writer.WriteString(mItem.reward.mats[sym].ToString());
                    writer.WriteEndElement();
                }
                // draw components to list
                foreach(string comp in mItem.reward.comps)
                {
                    writer.WriteStartElement("com");
                    writer.WriteString(comp);
                    writer.WriteEndElement();
                }

                writer.WriteStartElement("xp");
                writer.WriteString(mItem.reward.xp.ToString());
                writer.WriteEndElement();

                // close reward xml
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private void CreateWave(XmlWriter writer, int type)
        {
            switch (type)
            {
                case 0:
                    writer.WriteStartElement("enemy");
                    break;
                case 1:
                    writer.WriteStartElement("friendly");
                    break;
                case 2:
                    if(MissionWave == null)
                    {
                        return;
                    }

                    writer.WriteStartElement("mission");

                    // Write entire save script here
                    foreach(string ship in MissionWave.ShipNameList)
                    {
                        writer.WriteStartElement("ship");
                        writer.WriteString(ship);
                        writer.WriteEndElement();
                    }
                    // then return
                    return;
            }

            List<WaveItem> Wave = new List<WaveItem>
                (type == 0 ? EnemyWaves : FriendlyWaves);

            foreach (WaveItem item in Wave)
            {
                writer.WriteStartElement("wave");
                writer.WriteAttributeString("time", item.WaveTime.ToString());
                writer.WriteAttributeString("limit", item.WaveLimit.ToString());

                if(item.Boss != null)
                {
                    writer.WriteStartElement("boss");
                    writer.WriteString(item.Boss);
                    writer.WriteEndElement();
                }

                foreach(string ship in item.ShipNameList)
                {
                    writer.WriteStartElement("ship");
                    writer.WriteString(ship);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        // popup choice converters
        public int CType
        {
            get
            {
                switch(ContractType)
                {
                    case "defend":
                        return 0;
                    case "attrition":
                        return 1;
                    case "escort":
                        return 2;
                    case "targets":
                        return 3;
                    default:
                        return -1;
                }
            }

            set
            {
                switch (value)
                {
                    case 0:
                        ContractType = "defend"; break;
                    case 1:
                        ContractType = "attrition"; break;
                    case 2:
                        ContractType = "escort"; break;
                    case 3:
                        ContractType = "targets"; break;
                    default:
                        ContractType = "error"; break;
                }
            }
        }

        // READING DATA

        public void Load(TextAsset contractText)
        {
            // Create base document to load xml data in to
            XmlDocument baseContractXml = new XmlDocument ();

            // use try?
            // Read xml data
            baseContractXml.LoadXml(contractText.text);

            // Create XML Object
            XmlNode contractInfo = 
                baseContractXml.LastChild;

            // Load Contract filename
            ContractFile = contractText.name;

            foreach (XmlNode item
                    in contractInfo.ChildNodes)
            {
                switch(item.Name)
                {
                    case "info":
                        WriteData(item);
                        break;
                    case "primary_missions":
                        PopulateMission(item, true);
                        break;
                    case "optional_missions":
                        PopulateMission(item, false);
                        break;
                    case "waves":
                        PopulateWaves(item);
                        break;
                    default:
                        Debug.Log("Contact Object Importer - Load Contract: Unknown List Type in XML");
                        break;
                }
            }

            return;
        }

        /// <summary>
        /// Writes the description data to the contract
        /// data
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="newContract">New contract.</param>
        private void WriteData(XmlNode item)
        {
            foreach (XmlNode info
                     in item.ChildNodes)
            {
                // Add to ship
                switch (info.Name)
                {
                    case "name":
                        ContractName = info.InnerText;
                        break;
                    case "desc":
                        ContractDescription = info.InnerText;
                        break;
                    case "lvl":
                        ContractLevel = info.InnerText;
                        break;
                    case "ctype":
                        ContractType = info.InnerText;
                        break;
                }
            }
        }

        /// <summary>
        /// Populates the mission list of a contract.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="primary">If set to <c>true</c> primary.</param>
        private void PopulateMission(XmlNode item, bool primary)
        {
            // each element is mission data
            foreach (XmlNode mission
                     in item.ChildNodes)
            {
                // create empty mission
                MissionItem mData = new MissionItem();

                mData.type = mission.Attributes["type"].Value;

                foreach(XmlNode var in mission.ChildNodes)
                {
                    switch(var.Name)
                    {
                        case "time_limit":
                            float.TryParse(var.InnerText, out mData.timeLimit);
                            break;
                        case "goal":
                            int.TryParse(var.InnerText, out mData.destroyGoal);
                            break;
                        case "reward":
                        {
                            RewardInfo info = new RewardInfo();
                            info.xp = 0;
                            info.mats = new Dictionary<string, float>();
                            info.comps = new List<string>();

                            foreach(XmlNode reward in var.ChildNodes)
                            {
                                switch(reward.Name)
                                {
                                    case "mat":
                                    {
                                        info.mats.Add(reward.Attributes["sym"].Value,
                                             int.Parse(reward.InnerText));
                                        break;
                                    }
                                    case "com":
                                        info.comps.Add(reward.InnerText);
                                        break;
                                    case "xp":
                                        info.xp += float.Parse(reward.InnerText);
                                        break;
                                }
                            }
                            mData.reward = info;
                            break;
                        }
                    }
                }
                
                if(primary)
                    PrimaryMissions.Add(mData);
                else
                    OptionalMissions.Add(mData);
            }
        }
        
        private void PopulateWaves(XmlNode item)
        {
            // each element is mission data
            foreach (XmlNode wType
                     in item.ChildNodes)
            {
                switch(wType.Name)
                {
                    case "enemy":
                        EnemyWaves = new List<WaveItem>();
                        foreach (XmlNode wave
                                 in wType.ChildNodes)
                        {
                            EnemyWaves.Add(BuildWave(wave, true));
                        }
                        break;
                    case "friendly":
                        FriendlyWaves = new List<WaveItem>();
                        foreach (XmlNode wave
                                 in wType.ChildNodes)
                        {
                            FriendlyWaves.Add(BuildWave(wave, false));
                        }
                        break;
                    case "mission":
                        MissionWave = new WaveItem();
                        MissionWave.SetIndex(0, 2);
                        foreach (XmlNode ship
                                 in wType.ChildNodes)
                        {
                            MissionWave.ShipNameList.Add
                                (ship.InnerText);
                        }
                        break;
                }
            }
        }
        
        private WaveItem BuildWave(XmlNode item, bool enemy)
        {
            WaveItem wave = new WaveItem();
            if (item.Attributes ["time"] != null)
                float.TryParse(item.Attributes ["time"].Value, out wave.WaveTime);
            if (item.Attributes ["limit"] != null)
                int.TryParse(item.Attributes ["limit"].Value, out wave.WaveLimit);

            if (enemy)
                wave.SetIndex(EnemyWaves.Count, 0);
            else
                wave.SetIndex(FriendlyWaves.Count, 1);

            foreach (XmlNode ship
                     in item.ChildNodes)
            {
                if(ship.Name.Equals("boss"))
                {
                    wave.Boss = ship.InnerText;
                }
                else
                {
                    wave.ShipNameList.Add(ship.InnerText);
                }
            }
            
            return wave;
        }
    }
}

