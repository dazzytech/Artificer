using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ArtificerEditor
{

	public class ShipEditor : EditorWindow {

        /*
         * components 
         */
        private ShipContainer _ship =
            new ShipContainer();

		public static Vector2 _mousePos;
        private BaseShipComponent _selectedNode;
        GameObject selObj;

        public BaseShipComponent selNode
        {
            set
            {
                _selectedNode = value;
                selObj = value.SetGO;
                optionsIndex = value.GetDirection();
            }
        }

        private bool draggingMode;

        private string shipName = "";

        /*
         * GUI ATTRIBUTES
         * */
        private string[] options = new string[] {"Up", "Down", "Left", "Right"};
        private int optionsIndex = 0;

		[MenuItem("Window/Artificer/Ship Editor")]
		static void ShowEditor()
		{
            ShipEditor editor = 
                EditorWindow.GetWindow<ShipEditor> ();
		}

        void Awake()
        {
            _ship.Pieces.Clear();
        }

        void Update()
        {
            if (draggingMode)
            {
                _selectedNode.MovePiece(_mousePos);
                foreach (BaseShipComponent s in _ship.Pieces)
                {
                    if (_selectedNode.Bounds.Overlaps(s.Bounds)
                        && !_selectedNode.Equals(s))
                        _selectedNode.AddCollision(s);
                }
            } 

            if(_ship.Pieces.Count != 0)
                if (_ship.Pieces [0] == null)
                    _ship.Pieces.Clear();

            foreach (BaseShipComponent s in _ship.Pieces)
            {
                s.Tick();

                if(!draggingMode)
                    s.Connect();
            }
        }

		void OnGUI()
		{
            Event e = Event.current;
            _mousePos = e.mousePosition;

            EditorGUIUtility.labelWidth = 100f;
            EditorStyles.textField.wordWrap = true;

            if (e.button == 1)
            {
                if (e.type == EventType.MouseDown)
                {
                    bool clickedOnWindow = false;
                    int selectedIndex = -1;
					
                    for (int i = 0; i < _ship.Pieces.Count; i++)
                    {
                        if (_ship.Pieces [i].Bounds.Contains(_mousePos))
                        {
                            selectedIndex = i;
                            clickedOnWindow = true;
                            break;
                        }
                    }
					
                    if (!clickedOnWindow)
                    {
                        GenericMenu menu = new GenericMenu();
						
                        menu.AddItem(new GUIContent("Add Piece"), false, ContextCallback, "createComponent");

                        menu.ShowAsContext();
                        e.Use();
                    } else
                    {
                        GenericMenu menu = new GenericMenu();

                        menu.AddItem(new GUIContent("Delete Component"), false, ContextCallback, "deleteComponent");
                        menu.AddItem(new GUIContent("Duplicate Component"), false, ContextCallback, "duplicateComponent");
                        menu.AddItem(new GUIContent("Bring to front"), false, ContextCallback, "moveFront");
                        menu.AddItem(new GUIContent("Push to Back"), false, ContextCallback, "moveBack");
                      
                        menu.ShowAsContext();
                        e.Use();
                    }
                }
            } else if (e.button == 0 && e.type == EventType.MouseDown)
            {
                foreach(BaseShipComponent s in _ship.Pieces)
                {
                    if(s.Bounds.Contains(_mousePos))
                    {
                        selNode = s;
                        draggingMode = true;
                    }
                }
            }else if(e.button == 0 && e.type == EventType.MouseUp)
            {
                draggingMode = false;
            }

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            _ship.Name = EditorGUILayout.TextField("Ship Label", _ship.Name, 
                   GUILayout.MinWidth(100f), GUILayout.MinWidth(200f),GUILayout.ExpandWidth(true));

            if (Screen.width < 350)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }

            _ship.Player = EditorGUILayout.Toggle("Player Created", _ship.Player,
                   GUILayout.MinWidth(30f), GUILayout.MinWidth(50f),GUILayout.ExpandWidth(true));

            if (Screen.width < 700)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }

            _ship.RotorFollow = EditorGUILayout.Toggle("Follow Mouse", _ship.RotorFollow,
                                                  GUILayout.MinWidth(30f), GUILayout.MinWidth(50f),GUILayout.ExpandWidth(true));

            if (Screen.width < 300)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }

            // add textasset
            TextAsset newAsset = 
                (TextAsset)EditorGUILayout.ObjectField("Ship Asset", _ship.Asset, typeof(TextAsset),
                                                       false,GUILayout.MinWidth(30f), GUILayout.MinWidth(100f),
                                                       GUILayout.ExpandWidth(true));

            if (_ship.Asset != newAsset)
            {
                if(newAsset != null)
                {
                    _ship.Asset = newAsset;
                    _ship.Load();
                }
            }

            EditorGUILayout.EndHorizontal();

            if (_selectedNode != null)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(_selectedNode.GetName(),
                          GUILayout.MinWidth(100f), GUILayout.MinWidth(150f),GUILayout.ExpandWidth(true));

                if (Screen.width < 255)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }

                _selectedNode.IsHead =
                    EditorGUILayout.Toggle("Head Piece", _selectedNode.IsHead,
                                           GUILayout.MinWidth(30f), GUILayout.MinWidth(30f),GUILayout.ExpandWidth(true));

                if (Screen.width < 500)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }

                selObj = (GameObject)
                    EditorGUILayout.ObjectField(selObj,
                        typeof(GameObject), true,
                        GUILayout.MinWidth(50f), GUILayout.MinWidth(100f),GUILayout.ExpandWidth(true));

                if(selObj != null)
                    _selectedNode.SetGO = selObj;

                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();

                if(_selectedNode != null)
                {
                    _selectedNode.triggerKey = 
                    EditorGUILayout.TextField("Trigger Key",  _selectedNode.triggerKey);

                    if (Screen.width < 300)
                    {
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                    }

                    _selectedNode.combatKey = 
                        EditorGUILayout.TextField("Combat Key",  _selectedNode.combatKey);

                    if (Screen.width < 500)
                    {
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                    }

                    optionsIndex = EditorGUILayout.Popup(optionsIndex, options);
                    _selectedNode.SetDirection(optionsIndex);

                    string[] toList = _selectedNode.StyleList();
                    _selectedNode.StyleSelect = EditorGUILayout.Popup
                        (_selectedNode.StyleSelect, toList);
                }

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            if(Screen.width > 300)
                GUILayout.BeginArea(new Rect(0, position.height - 90, position.width, position.height));
            else
                GUILayout.BeginArea(new Rect(0, position.height - 107, position.width, position.height));

            GUILayout.BeginHorizontal();

            if (_ship.info == null)
                _ship.info = new Info();

            _ship.info.name = EditorGUILayout.TextField("Ship Name", _ship.info.name, 
                                                   GUILayout.MinWidth(100f), GUILayout.MinWidth(200f),GUILayout.ExpandWidth(true));

            if (Screen.width < 300)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }

            _ship.info.category = EditorGUILayout.TextField("Ship Category", _ship.info.category, 
                                                   GUILayout.MinWidth(100f), GUILayout.MinWidth(200f),GUILayout.ExpandWidth(true));

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            _ship.info.description = EditorGUILayout.TextField("Ship Description", _ship.info.description, 
                                      GUILayout.MinWidth(100f), GUILayout.MinWidth(200f), GUILayout.ExpandWidth(true),
                                      GUILayout.Height(50f));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear"))
            {
                _ship.Clear();
            }
            if (GUILayout.Button("Save Ship"))
            {
                if (EditorUtility.DisplayDialog("Confirm Save",
                                                "Are you sure you want to save?", "Yes", "No"))
                {
                    _ship.Save();
                    _ship.Asset = newAsset = // reassign asset to new piece
                        (TextAsset)Resources.Load("Space/Pre-built_Ships/" + _ship.Name);
                    AssetDatabase.SaveAssets();
                }
            }
            if (GUILayout.Button("Revert to Save"))
            {
                if (EditorUtility.DisplayDialog("Confirm Revert?",
                                                "You will lose all your progress from the last save.", "Yes", "No"))
                {
                    _ship.Load();
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            if (draggingMode && _selectedNode != null)
            {
                _selectedNode.MovePiece(_mousePos);
            }


            foreach (BaseShipComponent b in _ship.Pieces)
            {
                if(b.GetTex() != null)
                {
                    Matrix4x4 matrixBackup = GUI.matrix;
                    GUIUtility.RotateAroundPivot(b.Rotation,b.Bounds.center);
                    GUI.DrawTexture(b.Bounds, b.GetTex());
                    GUI.matrix = matrixBackup;

                    b.OnDrawGizmos();
                }
            }

            Repaint();
        }
		
		void ContextCallback(object obj)
	    {
			string clb = obj.ToString ();

            if (clb.Equals ("createComponent")) 
            {
                ShipComponent comp = 
                    ScriptableObject.CreateInstance("ShipComponent")
                        as ShipComponent;

                // var initialization
                comp.instanceID = _ship.Pieces.Count.ToString();
                comp.Bounds.position = _mousePos;

                // Selection
                selNode = comp;
                _ship.Pieces.Add (comp);
			} 
            else
			{
				bool clickedOnWindow = false;
				int selectedIndex = -1;
				
                for (int i = 0; i < _ship.Pieces.Count; i++) {
                    if (_ship.Pieces [i].Bounds.Contains (_mousePos)) {
						selectedIndex = i;
						clickedOnWindow = true;
						break;
					}
				}
				
                if (clb.Equals ("deleteComponent"))
    				if (clickedOnWindow) 
                    {
                        BaseShipComponent selNode =_ship.Pieces [selectedIndex];
                        _ship.Pieces.RemoveAt (selectedIndex);
                        if(_selectedNode.Equals(selNode))
                           selNode = null;

                        int i = 0;
                        foreach(BaseShipComponent b in _ship.Pieces)
                        {
                            b.instanceID = i.ToString();
                            i++;
                        }
    				}
                if(clb.Equals("duplicateComponent"))
                    if(clickedOnWindow)
                {
                    // get the clicked object
                    BaseShipComponent oldNode =_ship.Pieces [selectedIndex];

                    // create component and clone
                    ShipComponent comp = 
                        ScriptableObject.CreateInstance("ShipComponent")
                         as ShipComponent;
                    // Clone vars
                    comp.instanceID = _ship.Pieces.Count.ToString();
                    comp.Bounds.position = oldNode.Bounds.position
                        + oldNode.Bounds.size + (Random.insideUnitCircle * 10f);
                    comp.SetGO = oldNode.SetGO;
                    comp.IsHead = oldNode.IsHead;
                    comp.SetDirection(oldNode.GetDirection());
                    comp.triggerKey = oldNode.triggerKey;
                    comp.combatKey = oldNode.combatKey;
                    comp.StyleSelect = oldNode.StyleSelect;
                        
                    // Selection
                    selNode = comp;
                    _ship.Pieces.Add (comp);                        // add it to list

                }
                if (clb.Equals ("moveBack"))
                    if (clickedOnWindow) {
                        BaseShipComponent selNode = _ship.Pieces[selectedIndex];
                        _ship.Pieces.Remove(selNode);
                        _ship.Pieces.Insert(0, selNode);
                    }

                if (clb.Equals ("moveFront"))
                if (clickedOnWindow) {
                    BaseShipComponent selNode =_ship.Pieces[selectedIndex];
                    _ship.Pieces.Remove(selNode);
                    _ship.Pieces.Add(selNode);
                }
			}
		}
	}
}