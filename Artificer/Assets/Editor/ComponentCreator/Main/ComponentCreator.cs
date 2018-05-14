using UnityEngine;
using UnityEditor;
using System.Collections;

using Data.Space.DataImporter;

/*
 * Component Creator is used to build ship components easily. 
 * The list of tasks are as follows.
 * socketcount updated when object is imported
 * */
namespace ArtificerEditor.Components
{
    public class ComponentCreator : EditorWindow
    {
        private ComponentAttributes _att;

        [MenuItem("Window/Artificer/Component Creator")]
        static void ShowEditor()
        {
            ComponentCreator editor = 
                EditorWindow.GetWindow<ComponentCreator> ();
        }

        public ComponentCreator()
        {
            _att = new ComponentAttributes();

            // Pass attribute reference
            WindowGUI.SetAttributes(_att);
            SocketController.SetAttributes(_att);
            InputController.SetAttributes(_att);
            ComponentCallback.SetAttributes(_att);
            ComponentController.SetAttributes(_att);
        }

        void Update()
        {
            InputController.Update();
        }

        void OnGUI()
        {
            InputController.MouseHandle(Event.current);

            // Use utility to draw a header
            WindowGUI.DrawHeader();

            if(_att.ComponentSprites.ContainsKey("default"))
            {
                WindowGUI.DrawTexture(position);
            }

            WindowGUI.DrawFooter(position.width, position.height);

            Repaint();
        }
    }
}
