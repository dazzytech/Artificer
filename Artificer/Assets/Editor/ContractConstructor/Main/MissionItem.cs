using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using ShipComponents;

namespace Editor
{
    /// <summary>
    /// Reward info
    /// Contains reward yield as mats or exp or components
    /// </summary>
    public class RewardInfo
    {
        public float xp;
        public List<string> comps;
        public Dictionary<string, float> mats;
    }

    public class MissionItem : ScriptableObject
    {
        public string type;
        public float timeLimit;
        public int destroyGoal;
        public RewardInfo reward;
        public Dictionary<string, float> goalMats;

        // symbol item for adding mats
        string newSymReward = null;
        string newSymMission = null;

        public delegate void Delete(MissionItem item);
        
        public static Delete DeleteMission;

        public MissionItem()
        {
            type = "defend";
            timeLimit = 0;
            destroyGoal = 0;
            goalMats = new Dictionary<string, float>();
            CreateReward();
        }

        public void DrawRect()
        {
            // Draw Wave UI items
            // Build WindowGUI
            GUILayout.Space(10);
            
            EditorGUILayout.BeginHorizontal();

            // Create a mission type
            mType = EditorGUILayout.Popup(mType, new string[6]{"Defend", "Destroy", "Attrition", "Escort", "Targets", "Collect"},
            GUILayout.MinWidth(30), GUILayout.MaxWidth(50), GUILayout.ExpandWidth(true));

            // Create the delete button
            if (GUILayout.Button("Delete", GUILayout.MinWidth(30), GUILayout.MaxWidth(50), GUILayout.ExpandWidth(true)))
            if (EditorUtility.DisplayDialog("Confirm Deletion",
                                                "Are you sure you want to delete this mission?", "Yes", "No"))
            {
                DeleteMission(this);
                return;
            }

            EditorGUILayout.EndHorizontal();

            // Add number fields

            EditorGUILayout.BeginHorizontal();

            timeLimit = EditorGUILayout.FloatField("Time Limit:", timeLimit, 
                                                   GUILayout.MinWidth(30), GUILayout.MaxWidth(50), GUILayout.ExpandWidth(true));

            if (Screen.width < 500)
            {
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
            }

            // Create button 
            destroyGoal = EditorGUILayout.IntField("Destroy Goal:", destroyGoal, 
                                                   GUILayout.MinWidth(30), GUILayout.MaxWidth(50), GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();

            if (mType == 5)
                DrawGoalMats();

            // draw the mission reward
            DrawReward(reward);

            GUILayout.Space(10);
        }

        private void CreateReward()
        {
            reward = new RewardInfo();
            reward.xp = 0;
            reward.comps = new List<string>();
            reward.mats = new Dictionary<string, float>();
        }

        /// <summary>
        /// Builds the reward info item to the
        /// UI
        /// </summary>
        /// <param name="info">Info.</param>
        private void DrawReward(RewardInfo info)
        {
            // Create division
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

            // Label the reward
            EditorGUILayout.LabelField("New Reward Item", GUILayout.MinWidth(30), GUILayout.MaxWidth(50), GUILayout.ExpandWidth(true));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            info.xp = EditorGUILayout.FloatField("Exp Reward:", info.xp, 
                GUILayout.MinWidth(30), GUILayout.MaxWidth(50), GUILayout.ExpandWidth(true));

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.LabelField("Component Rewards:", GUILayout.MinWidth(30), 
                                       GUILayout.MaxWidth(50), GUILayout.ExpandWidth(true));

            DrawComponents(info);

            GUILayout.Space(10);

            EditorGUILayout.LabelField("Material Rewards:", GUILayout.MinWidth(30), 
                                       GUILayout.MaxWidth(50), GUILayout.ExpandWidth(true));

            DrawMaterials(info);
        }

        /// <summary>
        /// Draws the component list
        /// of the reward to the window
        /// </summary>
        /// <param name="info">Info.</param>
        private void DrawComponents(RewardInfo info)
        {
            // create string item for deletion
            string pendingDelete = "";

            foreach (string item in info.comps)
            {
                EditorGUILayout.BeginHorizontal();

                string[] comSplit = item.Split('/');

                EditorGUILayout.LabelField(string.Format(" - {0}: {1}\n", comSplit[0],  comSplit[1]), 
                                           GUILayout.MinWidth(30), GUILayout.MaxWidth(50), GUILayout.ExpandWidth(true));

                // Add remove button
                if(GUILayout.Button("X", GUILayout.Width(20)))
                    pendingDelete = item;

                EditorGUILayout.EndHorizontal();
            }

            // if pending is not empty then delete pending ship
            if (pendingDelete.Length != 0)
                info.comps.Remove(pendingDelete);

            EditorGUILayout.BeginHorizontal();

            // add textasset used to load existing contract
            GameObject newAsset = 
                (GameObject)EditorGUILayout.ObjectField("Create New:", null, typeof(GameObject),
                    false, GUILayout.MinWidth(20), GUILayout.MaxWidth(50) , GUILayout.ExpandWidth(true));

            EditorGUILayout.EndHorizontal();

            // if any character has been added then create a new string for the name
            if (newAsset != null)
            {
                // Create function to retreive ship name
                string item = BuildNewComponent(newAsset);
                if(item.Length != 0)
                    info.comps.Add(item);
            }
        }

        /// <summary>
        /// Builds the collect mission info items to the
        /// UI
        /// </summary>
        /// <param name="info">Info.</param>
        private void DrawGoalMats()
        {
            GUILayout.Space(10);
            
            EditorGUILayout.LabelField("Mission Materials:", GUILayout.MinWidth(30), 
                                       GUILayout.MaxWidth(50), GUILayout.ExpandWidth(true));
            // create string item for deletion
            string pendingDelete = "";
            
            List<string> keys = new List<string>(goalMats.Keys);
            foreach (string item in keys)
            {
                EditorGUILayout.BeginHorizontal();
                
                // Draw element symbol
                EditorGUILayout.LabelField(item, 
                                           GUILayout.MinWidth(5), GUILayout.MaxWidth(10), GUILayout.ExpandWidth(true));
                
                // editable amount
                goalMats[item] = EditorGUILayout.FloatField("Amount:", goalMats[item], 
                                                             GUILayout.MinWidth(30), GUILayout.MaxWidth(100), GUILayout.ExpandWidth(true));
                
                // Add remove button
                if(GUILayout.Button("X", GUILayout.Width(20)))
                    pendingDelete = item;
                
                EditorGUILayout.EndHorizontal();
            }
            
            // create ui to input element symbol and button to add
            EditorGUILayout.BeginHorizontal();
            
            newSymMission = EditorGUILayout.TextField("Enter Symbol:", newSymMission,
                                               GUILayout.MinWidth(30), GUILayout.MaxWidth(50), GUILayout.ExpandWidth(true));
            
            if (GUILayout.Button("Save Sym", GUILayout.MinWidth(30), GUILayout.MaxWidth(50), GUILayout.ExpandWidth(true)))
            {
                if(newSymMission != null)
                {
                    if (!goalMats.ContainsKey(newSymMission))
                    {
                        goalMats.Add(newSymMission, 0f);
                        newSymMission = null;
                    }
                }
            }

            if (pendingDelete != "")
            if (goalMats.ContainsKey(pendingDelete))
                goalMats.Remove(pendingDelete);
            
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Builds the new component
        /// using the game object with error checking
        /// </summary>
        /// <returns>The new component.</returns>
        /// <param name="GO">G.</param>
        private string BuildNewComponent(GameObject GO)
        {
            ComponentListener Con = GO.GetComponent<ComponentListener>();
            if (Con != null)
                return string.Format("{0}/{1}", Con.ComponentType, GO.name);
            else
                return "";
        }

        private void DrawMaterials(RewardInfo info)
        {
            // create string item for deletion
            string pendingDelete = "";

            List<string> keys = new List<string>(info.mats.Keys);
            foreach (string item in keys)
            {
                EditorGUILayout.BeginHorizontal();

                // Draw element symbol
                EditorGUILayout.LabelField(item, 
                   GUILayout.MinWidth(5), GUILayout.MaxWidth(10), GUILayout.ExpandWidth(true));

                // editable amount
                info.mats[item] = EditorGUILayout.FloatField("Amount:", info.mats[item], 
                    GUILayout.MinWidth(30), GUILayout.MaxWidth(100), GUILayout.ExpandWidth(true));
                
                // Add remove button
                if(GUILayout.Button("X", GUILayout.Width(20)))
                    pendingDelete = item;
                
                EditorGUILayout.EndHorizontal();
            }

            // create ui to input element symbol and button to add
            EditorGUILayout.BeginHorizontal();

            newSymReward = EditorGUILayout.TextField("Enter Symbol:", newSymReward,
                GUILayout.MinWidth(30), GUILayout.MaxWidth(50), GUILayout.ExpandWidth(true));

            if (GUILayout.Button("Save Sym", GUILayout.MinWidth(30), GUILayout.MaxWidth(50), GUILayout.ExpandWidth(true)))
            {
                if(newSymReward != null)
                {
                    if (!info.mats.ContainsKey(newSymReward))
                    {
                        info.mats.Add(newSymReward, 0f);
                        newSymReward = null;
                    }
                }
            }

            if (pendingDelete != "")
                if (info.mats.ContainsKey(pendingDelete))
                    info.mats.Remove(pendingDelete);

            EditorGUILayout.EndHorizontal();
        }

        // popup choice converters
        public int mType
        {
            get
            {
                switch(type)
                {
                    case "defend":
                        return 0;
                    case "destroy":
                        return 1;
                    case "attrition":
                        return 2;
                    case "escort":
                        return 3;
                    case "targets":
                        return 4;
                    case "collect":
                        return 5;
                    default:
                        return -1;
                }
            }
            
            set
            {
                switch (value)
                {
                    case 0:
                        type = "defend"; break;
                    case 1:
                        type = "destroy"; break;
                    case 2:
                        type = "attrition"; break;
                    case 3:
                        type = "escort"; break;
                    case 4:
                        type = "targets"; break;
                    case 5:
                        type = "collect"; break;
                    default:
                        type = "error"; break;
                }
            }
        }
    }
}

