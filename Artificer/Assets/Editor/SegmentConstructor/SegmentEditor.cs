using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Editor
{

	public class SegmentEditor : EditorWindow {

		private List<BaseComponent> _components =
			new List<BaseComponent>();

		private ComponentContainer _cont =
			new ComponentContainer ();

		private Vector2 _mousePos;
		
		private BaseComponent _selectedNode;

		private string mapName = "";

		[MenuItem("Window/Artificer/Space Segment Editor")]
		static void ShowEditor()
		{
			SegmentEditor editor = 
				EditorWindow.GetWindow<SegmentEditor> ();
		}

		void OnGUI()
		{
			Event e = Event.current;
			_mousePos = e.mousePosition;

			if (e.button == 1) 
			{
				if (e.type == EventType.MouseDown) 
				{
					bool clickedOnWindow = false;
					int selectedIndex = -1;
					
					for (int i = 0; i < _components.Count; i++) {
						if (_components [i].windowRect.Contains (_mousePos)) {
							selectedIndex = i;
							clickedOnWindow = true;
							break;
						}
					}
					
					if (!clickedOnWindow) 
					{
						GenericMenu menu = new GenericMenu ();
						
						menu.AddItem (new GUIContent ("Add Station"), false, ContextCallback, "stationComponent");
						menu.AddItem (new GUIContent ("Add Asteroid Field"), false, ContextCallback, "asteroidComponent");
						menu.AddItem (new GUIContent ("Add Satellite"), false, ContextCallback, "satelliteComponent");
                        
                        menu.ShowAsContext ();
                        e.Use ();
                    } else	{
                        GenericMenu menu = new GenericMenu ();

                        menu.AddItem (new GUIContent ("Delete Component"), false, ContextCallback, "deleteComponent");
                        
						menu.ShowAsContext ();
						e.Use ();
					}
				}
			}

			GUILayout.BeginVertical ();
			GUILayout.BeginHorizontal ();
			mapName = EditorGUILayout.TextField ("Enter map name.", mapName);

			if (GUILayout.Button ("Save Segment")) {
                if(EditorUtility.DisplayDialog("Confirm Save",
                   "Are you sure you want to save?" , "Yes", "No"))
                    _cont.Save(mapName, _components);
			}

			if (GUILayout.Button ("Load Segment")) {
				_cont = _cont.Load(mapName);
				_cont.PopComps(out _components);
            }

			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();

            _cont.threat = EditorGUILayout.TextField ("Threat Level.", _cont.threat);

			_cont.x = EditorGUILayout.TextField ("Map X.", _cont.x);

			_cont.y = EditorGUILayout.TextField ("Map Y.", _cont.y);

			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			
			_cont.pX = EditorGUILayout.TextField ("Physical X.", _cont.pX);
			
			_cont.pY = EditorGUILayout.TextField ("Physical Y.", _cont.pY);
			
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
            
			_cont.sizeX = EditorGUILayout.TextField ("Map Width.", _cont.sizeX); 
            
			_cont.sizeY = EditorGUILayout.TextField ("Map Height.", _cont.sizeY );
            
            GUILayout.EndHorizontal ();
			
			GUILayout.EndVertical ();
            

			BeginWindows ();

			for (int i = 0; i < _components.Count; i++) {
				_components[i].windowRect = 
					GUI.Window(i, _components[i].windowRect, 
							    DrawComponentWindow, 
					           _components[i].windowTitle);
            }
            
            EndWindows ();
		}
	

		void DrawComponentWindow(int id)
		{
			_components [id].DrawWindow ();
			GUI.DragWindow ();
		}
		
		void ContextCallback(object obj)
	    {
			string clb = obj.ToString ();

			if (clb.Equals ("stationComponent")) {
				StationComponent comp = new StationComponent ();
				comp.windowRect = new Rect (_mousePos.x, 
				                                 _mousePos.y, 200, 180);
				_components.Add (comp);
			} else 
			if (clb.Equals ("asteroidComponent")) {
				AsteroidComponent comp = new AsteroidComponent ();
				comp.windowRect = new Rect (_mousePos.x, 
				                            _mousePos.y, 200, 150);
				_components.Add (comp);
			} else
			if (clb.Equals ("satelliteComponent")) {
				SatelliteComponent comp = new SatelliteComponent ();
				comp.windowRect = new Rect (_mousePos.x, 
				                            _mousePos.y, 200, 120);
                _components.Add (comp);
            } else 
                if (clb.Equals ("deleteComponent"))
			{
				bool clickedOnWindow = false;
				int selectedIndex = -1;
				
				for (int i = 0; i < _components.Count; i++) {
					if (_components [i].windowRect.Contains (_mousePos)) {
						selectedIndex = i;
						clickedOnWindow = true;
						break;
					}
				}
				
				if (clickedOnWindow) {
					BaseComponent selNode =_components [selectedIndex];
					_components.RemoveAt (selectedIndex);
				}
			}
		}
	}
}