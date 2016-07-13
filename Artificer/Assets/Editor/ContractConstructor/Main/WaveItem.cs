using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using System.Xml;

namespace Editor
{
    public class WaveItem : ScriptableObject
    {
        private int WaveNumber;
        public float WaveTime;
        public int WaveLimit;
        public int WaveType;

        public delegate void Delete(WaveItem item);

        public static Delete DeleteWave;

        public List<string> ShipNameList;

        public string Boss;
        public TextAsset BossAst;


        public WaveItem()
        {
            // Init wave data
            WaveTime = 0;
            WaveLimit = 0;

            ShipNameList = new List<string>();
        }

        public void SetIndex(int index, int type)
        {
            WaveNumber = index;

            if(type != -1)
                WaveType = type;
        }
        
        public void DrawRect()
        {
            // Draw Wave UI items
            // Build WindowGUI
            GUILayout.Space(10);
        
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Wave: " + WaveNumber.ToString(), GUILayout.MinWidth(20), GUILayout.MaxWidth(30) , GUILayout.ExpandWidth(true));

            // Create the delete button
            if (GUILayout.Button("Delete"))
            if (EditorUtility.DisplayDialog("Confirm Deletion",
                                                "Are you sure you want to delete this wave?", "Yes", "No"))
            {
                DeleteWave(this);
                return;
            }

            // fixed width, should fit every label
            EditorGUIUtility.labelWidth = 80f;

            // Label and items cant fit on same line
            if (Screen.width < 500)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }
           
            // Create responsive headerbar
            // fluid Wave time
            WaveTime = EditorGUILayout.FloatField("Wave Time:", WaveTime);

            // Cant fit both items on one line
            if (Screen.width < 250)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }

            WaveLimit = EditorGUILayout.IntField("Wave Limit:", WaveLimit);

            // create string item for deletion
            string pendingDelete = "";

            // Build string list in form of text boxes
            foreach (string ship in ShipNameList)
            {
                // Horizontal list item
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Ship Name:", ship);

                // Add remove button
                if(GUILayout.Button("X", GUILayout.Width(20)))
                    pendingDelete = ship;
            }

            // if pending is not empty then delete pending ship
            if (pendingDelete.Length != 0)
                ShipNameList.Remove(pendingDelete);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            // add textasset used to load existing contract
            TextAsset newAsset = 
                (TextAsset)EditorGUILayout.ObjectField("Create New:", null, typeof(TextAsset),
                    false, GUILayout.MinWidth(200), GUILayout.MaxWidth(500) , GUILayout.ExpandWidth(true));

            // if any character has been added then create a new string for the name
            if (newAsset != null)
                // Create function to retreive ship name
                ShipNameList.Add(RetreiveName(newAsset));

            EditorGUILayout.EndHorizontal();

            // Draw boss for none mission items
            if (WaveType != 2)
            {
                EditorGUILayout.BeginHorizontal();
                // add textasset used to load existing contract
                BossAst = 
                (TextAsset)EditorGUILayout.ObjectField("Create New Boss:", BossAst, typeof(TextAsset),
                    false, GUILayout.MinWidth(200), GUILayout.MaxWidth(500), GUILayout.ExpandWidth(true));

                if (BossAst != null)
                {
                    Boss = RetreiveName(BossAst);
                }

                if (Screen.width < 1000)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }

                EditorGUILayout.LabelField("Boss Name:", Boss);

                EditorGUILayout.EndHorizontal();
            }
        }

        private string RetreiveName(TextAsset asset)
        {
            // Create XML item
            XmlDocument baseShipXml = new XmlDocument ();
            baseShipXml.LoadXml(asset.text);

            // search for ship name within xml
            foreach (XmlNode componentList
                     in baseShipXml.LastChild.ChildNodes)
            {
                if (componentList.Name == "info")
                {
                    foreach (XmlNode componentInfo
                             in componentList.ChildNodes)
                    {
                        // Add to ship
                        if(componentInfo.Name == "name")
                            return componentInfo.InnerText;
                    }
                }
            }
            return asset.name;
        }
    }
}

