using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Editor.Extension
{ 
    [CustomEditor(typeof(ViewerItem))]
    [CanEditMultipleObjects]
    public class ViewerItemEditor : UnityEditor.Editor
    {
        #region ATTRIBUTES

        ViewerItem m_viewerItem = null;

        SerializedProperty m_health;

        SerializedProperty m_interactive;

        #region COLOR

        SerializedProperty m_highHealth;

        SerializedProperty m_medHealth;

        SerializedProperty m_lowHealth;

        #endregion

        #endregion

        #region EDITOR

        void OnEnable()
        {
            // Assign serialized properties

            m_viewerItem = serializedObject.targetObject as ViewerItem;

            m_health = serializedObject.FindProperty("m_displayHealth");

            m_interactive = serializedObject.FindProperty("m_interactive");

            m_highHealth = serializedObject.FindProperty("HighHealth");

            m_medHealth = serializedObject.FindProperty("MedHealth");

            m_lowHealth = serializedObject.FindProperty("LowHealth");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Label("Behaviours", EditorStyles.boldLabel);

            m_health.boolValue = EditorGUILayout.Toggle("Display Health", m_health.boolValue);

            m_interactive.boolValue = EditorGUILayout.Toggle("Interactive", m_interactive.boolValue);

            if(m_health.boolValue)
            {
                GUILayout.Label("Colours", EditorStyles.boldLabel);

                m_highHealth.colorValue = EditorGUILayout.ColorField("High Colour", m_highHealth.colorValue);

                m_medHealth.colorValue = EditorGUILayout.ColorField("Medium Colour", m_medHealth.colorValue);

                m_lowHealth.colorValue = EditorGUILayout.ColorField("Low Colour", m_lowHealth.colorValue);
            }

            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}