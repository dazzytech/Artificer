using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Editor
{
    public class ContractEditor : EditorWindow 
    {
        // Interaction variables
        public static Vector2 _mousePos;
        public static Vector2 _scrollPos;

        // Store container for all data
        private static ContractObject cObj;

        // Create items to contain objects pending deletion
        private static WaveItem deleteWave;
        private static MissionItem deleteMission;

        static ContractEditor()
        {
            WaveItem.DeleteWave = DeleteWave;
            MissionItem.DeleteMission = DeleteMission;
        }

        [MenuItem("Window/Artificer/Contract Editor")]
        static void ShowEditor()
        {
            ContractEditor editor = 
                EditorWindow.GetWindow<ContractEditor> ();
        }

        void Update()
        {
            // if any pending deleted item exist then remove them
            if (deleteWave != null)
            {
                if(cObj.EnemyWaves.Contains(deleteWave))
                    cObj.EnemyWaves.Remove(deleteWave);
                if(cObj.FriendlyWaves.Contains(deleteWave))
                    cObj.FriendlyWaves.Remove(deleteWave);
                if(cObj.MissionWave != null)
                    if(cObj.MissionWave.Equals(deleteWave))
                        cObj.MissionWave = null;
                deleteWave = null;
            }

            if (deleteMission != null)
            {
                if(cObj.PrimaryMissions.Contains(deleteMission))
                    cObj.PrimaryMissions.Remove(deleteMission);
                else if(cObj.OptionalMissions.Contains(deleteMission))
                    cObj.OptionalMissions.Remove(deleteMission);
                deleteMission = null;
            }
        }

        void OnGUI()
        {
            Event e = Event.current;
            _mousePos = e.mousePosition;

            // Build Window UI
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.BeginHorizontal();

            // fixed width, should fit every label
            EditorGUIUtility.labelWidth = 100f;
            // Text items should wrap
            EditorStyles.textField.wordWrap = true;

            if (cObj == null)
            {
                BuildContract();
            }

            // Create responsive headerbar
            // fluid Contract label
            EditorGUILayout.LabelField("Contract File:", cObj.ContractFile, GUILayout.MinWidth(200), GUILayout.MaxWidth(500) , GUILayout.ExpandWidth(true));

            // Respond to not being able to fit all elements in window
            if (Screen.width < 1500)
            {
                // add load button at the end of the label is correct size
                if (Screen.width > 250)
                    if(GUILayout.Button("New", GUILayout.MinWidth(50),
                                     GUILayout.MaxWidth(100), GUILayout.ExpandWidth(true)))
                    {
                        if (EditorUtility.DisplayDialog("Confirm New",
                                                        "Are you sure you want a new contract?", "Yes", "No"))
                        {
                            BuildContract();
                        }

                    }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }

            cObj.ContractName = EditorGUILayout.TextField("Contract Name:", cObj.ContractName,
                               GUILayout.MinWidth(200), GUILayout.MaxWidth(500) , GUILayout.ExpandWidth(true));

            if (Screen.width < 500)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }
            // add textasset used to load existing contract
            TextAsset newAsset = 
                (TextAsset)EditorGUILayout.ObjectField("Load Contract:", null, typeof(TextAsset),
                                                       false, GUILayout.MinWidth(200), GUILayout.MaxWidth(500) , GUILayout.ExpandWidth(true));

            if (newAsset != null)
            {
                cObj.Load(newAsset);
                newAsset = null;
            }

            // respond to not fitting object picker and button in window
            if (Screen.width < 250)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }

            // save changes to contract
            if (GUILayout.Button("Save", GUILayout.MinWidth(50), GUILayout.MaxWidth(100), GUILayout.ExpandWidth(true)))
            {
                if (EditorUtility.DisplayDialog("Confirm Save",
                                               "Are you sure you want to save?", "Yes", "No"))
                {
                    if (!SaveContract())
                        EditorUtility.DisplayDialog("Save Failed", "Save was not saved successfully", "OK");
                }
            }

            // Add New Button to the end of the save button if width is between 250 & 500
            if (Screen.width < 250 || Screen.width > 1500)
            {
                if(GUILayout.Button("New", GUILayout.MinWidth(50),
                                    GUILayout.MaxWidth(100), GUILayout.ExpandWidth(true)))
                {
                    if (EditorUtility.DisplayDialog("Confirm New",
                                                    "Are you sure you want a new contract?", "Yes", "No"))
                    {
                        BuildContract();
                    }
                    
                }
            }
            
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            // description element large text input with resizing height
            cObj.ContractDescription = EditorGUILayout.TextArea(cObj.ContractDescription ,GUILayout.MinWidth(200), GUILayout.MaxWidth(500) , GUILayout.ExpandWidth(true),
                                      GUILayout.MinHeight(50), GUILayout.MaxHeight(100) , GUILayout.ExpandHeight(true));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            // Contract level/requirement
            cObj.ContractLevel = EditorGUILayout.Popup(int.Parse(cObj.ContractLevel), 
                new string[10]{"0", "1", "2", "3", "4", "5", "6", "7", "8", "9"},
                GUILayout.MinWidth(100), GUILayout.MaxWidth(200), GUILayout.ExpandWidth(true)).ToString();

            // respond to not fitting both popup windows horizontally
            if (Screen.width < 250)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }

            // contract type
            cObj.CType = EditorGUILayout.Popup(cObj.CType, new string[4]{"Defend", "Attrition", "Escort", "Targets"},
                GUILayout.MinWidth(100), GUILayout.MaxWidth(200), GUILayout.ExpandWidth(true));

            EditorGUILayout.EndHorizontal();

            // Create Scroll view for each wave and mission item
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, 
                GUILayout.MinHeight(200), GUILayout.MaxHeight(500),GUILayout.ExpandHeight(true));

            EditorGUILayout.LabelField("Primary Mission Items", GUILayout.ExpandWidth(true));

            // for each mission - cycle through and display UI elements
            foreach (MissionItem mission in cObj.PrimaryMissions)
            {
                mission.DrawRect();
            }

            if (GUILayout.Button("Create New..", GUILayout.ExpandWidth(true)))
                CreateMission(true);

            GUILayout.Space(10);
            
            EditorGUILayout.LabelField("Optional Mission Items", GUILayout.ExpandWidth(true));

            // for each mission - cycle through and display UI elements
            foreach (MissionItem mission in cObj.OptionalMissions)
            {
                mission.DrawRect();
            }

            if (GUILayout.Button("Create New..", GUILayout.ExpandWidth(true)))
                CreateMission(false);

            GUILayout.Space(10);

            EditorGUILayout.LabelField("Enemy Wave Items", GUILayout.ExpandWidth(true));

            // for each mission - cycle through and display UI elements
            foreach (WaveItem wave in cObj.EnemyWaves)
            {
                wave.DrawRect();
            }
            
            if (GUILayout.Button("Create New..", GUILayout.ExpandWidth(true)))
                CreateWave(0);

            GUILayout.Space(10);

            EditorGUILayout.LabelField("Friendly Wave Items", GUILayout.ExpandWidth(true));
            
            // for each mission - cycle through and display UI elements
            foreach (WaveItem wave in cObj.FriendlyWaves)
            {
                wave.DrawRect();
            }
            
            if (GUILayout.Button("Create New..", GUILayout.ExpandWidth(true)))
                CreateWave(1);

            GUILayout.Space(10);

            EditorGUILayout.LabelField("Mission Wave", GUILayout.ExpandWidth(true));
            
            // for each mission - cycle through and display UI elements
            if (cObj.MissionWave != null)
            {
                cObj.MissionWave.DrawRect();
            } else
            {
                if (GUILayout.Button("Create New..", GUILayout.ExpandWidth(true)))
                    CreateWave(2);
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        private static bool SaveContract()
        {
            if (Resources.Load("Space/ContractLists" + cObj.ContractFile) != null)
            {
                if (EditorUtility.DisplayDialog("Confirm Save",
                                                "Object already exists, are you sure you wish to overwrite?", "Yes", "No"))
                {
                    if(cObj.Save())
                    {
                        AssetDatabase.SaveAssets();
                        return true;
                    }
                    else
                        return false;
                }
            }
            if(cObj.Save())
                return true;
            else
                return false;
        }
        
        /// <summary>
        /// Adds an empty mission to the Mission List.
        /// </summary>
        private static void CreateMission(bool primary)
        {
            MissionItem mission = 
                ScriptableObject.CreateInstance("MissionItem") as MissionItem;

            if (primary)
                cObj.PrimaryMissions.Add(mission);
            else
                cObj.OptionalMissions.Add(mission);
        }

        private static void DeleteMission(MissionItem item)
        {
            // Remove item that called func
            deleteMission = item;
            
            // Reset index numbers in numeric order
            int index = 0;
        }

        /// <summary>
        /// Adds an empty wave to the Wave List.
        /// </summary>
        private static void CreateWave(int type)
        {
            WaveItem wave = 
                ScriptableObject.CreateInstance("WaveItem") as WaveItem;

            switch(type)
            {
                case 0:
                    wave.SetIndex(cObj.EnemyWaves.Count, type);
                    cObj.EnemyWaves.Add(wave);
                    break;
                case 1:
                    wave.SetIndex(cObj.FriendlyWaves.Count, type);
                    cObj.FriendlyWaves.Add(wave);
                    break;
                case 2:
                    wave.SetIndex(0, type);
                    cObj.MissionWave = wave;
                    break;
            }
        }

        private static void DeleteWave(WaveItem item)
        {
            // Remove item that called func
            deleteWave = item;

            // Reset index numbers in numeric order
            int index = 0;
            switch(item.WaveType)
            {
                case 0:
                    foreach (WaveItem wave in cObj.EnemyWaves)
                    // Exclude the current item as it will be removed
                    if(wave != item)
                    {
                        wave.SetIndex(index, -1);
                        index++;
                    }
                    break;
                case 1:
                    foreach (WaveItem wave in cObj.FriendlyWaves)
                        // Exclude the current item as it will be removed
                        if(wave != item)
                    {
                        wave.SetIndex(index, -1);
                        index++;
                    }
                    break;
                case 2:
                    break;
            }
        }

        private static void BuildContract()
        {
            // Create new contract storage
            cObj = new ContractObject();
            
            //Assign filename
            // Calculate existing items
            int count = Resources.LoadAll("Space/ContractLists").Length;
            cObj.ContractFile = "contract_" + count.ToString().PadLeft(3, '0');
            
            cObj.ContractName = "Untitled";

            cObj.ContractDescription = "Enter a description here..";

            cObj.ContractLevel = "0";

            cObj.ContractType = "defend";

            cObj.PrimaryMissions 
                = new List<MissionItem>();

            cObj.OptionalMissions 
                = new List<MissionItem>();

            cObj.EnemyWaves 
                = new List<WaveItem>();

            cObj.FriendlyWaves 
                = new List<WaveItem>();

            cObj.MissionWave 
                = null;
        }
    }
}