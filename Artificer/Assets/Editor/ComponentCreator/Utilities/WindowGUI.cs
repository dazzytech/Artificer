using UnityEngine;
using UnityEditor;
using System.Collections;

using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;

namespace ArtificerEditor.Components
{
    public class WindowGUI
    {
        // Attributes Pointer
        private static ComponentAttributes _att;

        // Fixed set of strings for determining socket type
        private static string[] options = new string[] {"Compulsary", "Optional", "Component", "Hidden"};
        private static string[] directions = new string[] {"up","down","left","right"};
        private static int dir;

        // scrolling 
        public static Vector2 _scrollPos;

        /// <summary>
        /// Sets the attributes.
        /// invoked in CC constructor
        /// </summary>
        /// <param name="att">Att.</param>
        public static void SetAttributes(ComponentAttributes att)
        {
            _att = att;
        }

        /// <summary>
        /// Draws the header.
        /// </summary>
        public static void DrawHeader()
        {
            GUILayout.BeginVertical();

            EditorGUIUtility.labelWidth = 50f;

            EditorGUILayout.BeginHorizontal();
            // Selection Boxes
            // lanels
            _att.ComponentLabel = EditorGUILayout.TextField("Label", _att.ComponentLabel, GUILayout.ExpandWidth(true));

            if (Screen.width < 500)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }

            // load object
            GameObject newComp = (GameObject)EditorGUILayout.ObjectField(
                "Existing", _att.ComponentGO, typeof(GameObject), false, 
                GUILayout.MinWidth(100), GUILayout.MaxWidth(100), GUILayout.ExpandWidth(true));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUIUtility.labelWidth = 50f;

            _att.ComponentName = EditorGUILayout.TextField("Name", _att.ComponentName,
                                                           GUILayout.MinWidth(200),  GUILayout.ExpandWidth(true));

            if (Screen.width < 500)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }

            if(_att.ComponentName != "")
                _att.Type.Selected = EditorGUILayout.Popup
                    (_att.Type.Selected, _att.Type.GetList, GUILayout.ExpandWidth(true));

            if (Screen.width < 250)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }

            EditorGUIUtility.labelWidth = 110f;

            _att.GOCollider = EditorGUILayout.Toggle("Has Collider?", _att.GOCollider, 
                GUILayout.MinWidth(20), GUILayout.MaxWidth(100), GUILayout.ExpandWidth(true));
            
            EditorGUILayout.EndHorizontal();

            // DRAW OBJECT PICKER FOR EACH COMPONENT PIECE
            // create string for storing name of destroyed sprite
            string delSprite = "";

            string changeString = "";

            // Begin scroll area if more than 3 comp styles to stop overflow 
            if(_att.ComponentSprites.Count > 3)
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, 
                  GUILayout.Height(61));

            foreach (string key in _att.ComponentSprites.Keys)
            {
                EditorGUILayout.BeginHorizontal();

                string name = EditorGUILayout.TextField(key);
                EditorGUILayout.LabelField(_att.ComponentSprites[key].name);

                if(name != key)
                {
                    changeString = name;
                    delSprite = key;
                }

                if(GUILayout.Button("X"))
                    delSprite = key;

                EditorGUILayout.EndHorizontal();
            }

            if (_att.ComponentSprites.Count > 3)
                EditorGUILayout.EndScrollView();

            if (changeString != "")
            if (_att.ComponentSprites.ContainsKey(delSprite))
            {   
                Sprite temp = _att.ComponentSprites[delSprite];
                _att.ComponentSprites.Remove(delSprite);
                _att.ComponentSprites.Add(changeString, temp);
                delSprite = "";
            }

            // if delSprite != null then delete sprite
            if (delSprite != "")
            {
                if (_att.ComponentSprites.ContainsKey(delSprite))
                    _att.ComponentSprites.Remove(delSprite);
                if(_att.ComponentSprite.name == delSprite)
                    _att.ComponentSprite = null;
            }

            // Create an object picker for adding new textures.
            Sprite newSprite = EditorGUILayout.ObjectField(
                "Texture Picker", null, typeof(Sprite), false) as Sprite;

            if (newSprite != null)
            {
                if(_att.ComponentSprites.Count == 0)
                    _att.ComponentSprites.Add("default", newSprite);
                else
                    _att.ComponentSprites.Add("untitled_" + 
                        (_att.ComponentSprites.Count+1).ToString(), newSprite);
            }

            // if new 
            if (newComp != _att.ComponentGO)
            {
                _att.ComponentGO = newComp;

                _att.ComponentSprites.Clear();

                _att.EP = null;

                if(_att.ComponentGO != null)
                {
                    // overwrite changes
                    _att.ComponentLabel = _att.ComponentGO.transform.name;

                    // Test if component already has a collider and if so then set colliderGO
                    // to true
                    if(newComp.GetComponent<BoxCollider2D>() != null)
                        _att.GOCollider = true;
                    else
                        _att.GOCollider = false;

                    // Add material requirements to editor attributes
                    int index = 0;
                    _att.Symbols = new string[4];
                    _att.Amounts = new float[4];

                    ComponentListener list = newComp.GetComponent<ComponentListener>();
                    Space.Ship.Components.Attributes.ComponentAttributes att = list.GetAttributes();

                    _att.Weight = list.Weight;
                    _att.HP = att.Integrity;
                    _att.ComponentName = att.Name;
                    _att.ComponentSprite = null;

                    foreach(ConstructInfo info in att.RequiredMats)
                    {
                        if(info.material != null)
                        {
                            _att.Symbols[index] = info.material;
                            _att.Amounts[index] = info.amount;
                        }
                        index++;
                    }

                    // Load all the saved textures into attributes
                    if(att.componentStyles != null)
                    {
                        foreach(StyleInfo info in att.componentStyles)
                        {
                            _att.ComponentSprites.Add(info.name, info.sprite);
                        }

                        // use the default texture for sizing purposes
                        if(_att.ComponentSprites.ContainsKey("default"))
                            SocketController.PopulateSocketList(newComp, _att.ComponentSprites["default"].texture);
                    }else{
                        // use the texture currently in the GO renderer as default
                        // retreive texture from the first child object
                        foreach(Transform child in newComp.transform)
                        {
                            // if component has a renderer then set the sprite as comp texture
                            SpriteRenderer newTex =
                                child.GetComponent<SpriteRenderer>();
                            if(newTex != null)
                            {
                                if(newTex.sprite != null)
                                {
                                    _att.ComponentSprites.Add("default", newTex.sprite);
                                    // use default texture as reference for sockets
                                    SocketController.PopulateSocketList
                                        (newComp, _att.ComponentSprites["default"].texture);
                                    break;
                                }
                            }
                        }
                    }

                    _att.Type.ImportType(newComp);
                    Selection.activeGameObject=newComp.gameObject;
                }
            }

            // currently a socket is connected
            if (_att.SelectedSocket != null)
            {
                if(_att.SelectedSocket is ComponentSocket)
                {
                    EditorGUILayout.BeginHorizontal();
                    // output label ID
                    EditorGUILayout.LabelField(string.Format
                        ("Socket - {0} info: ", ((ComponentSocket)_att.SelectedSocket).ID));

                    if(Screen.width > 250)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.EndHorizontal();
                    }

                    // change label ID
                    _att.SelectedSocket.socketType = EditorGUILayout.Popup
                        (_att.SelectedSocket.socketType, options);
                    dir = EditorGUILayout.Popup
                        (((ComponentSocket)_att.SelectedSocket).dir, directions);
                    _att.SelectedSocket.socketDirection = directions[dir];
                    _att.Sockets[_att.Sockets.IndexOf(_att.SelectedSocket)].socketDirection = directions[dir];
                    EditorGUILayout.EndHorizontal();
                }

                _att.SelectedSocket.Position = EditorGUILayout.Vector2Field
                    ("Selected Socket Position:", _att.SelectedSocket.Position);
            }

            GUILayout.EndVertical();
        }

        /// <summary>
        /// Draws the texture.
        /// to the middle of the window
        /// </summary>
        /// <param name="center">Center.</param>
        public static void DrawTexture(Rect position)
        {
            // assign texture variable using component sprite
            if(_att.ComponentSprites.ContainsKey("default"))
            {
                if(_att.ComponentSprite == null)               
                    _att.ComponentSprite = _att.ComponentSprites["default"];

                float yAxis = (position.height / 2) - (_att.ComponentSprite.rect.height / 2);

                yAxis += Screen.width < 250? 75: Screen.width < 500? 60: 30;

                yAxis += _att.ComponentSprites.Count >= 3? 10:0;

                
                // Calculate center point for drawing the sprite
                _att.CenterPoint = new Vector2(
                    (position.width / 2) - (_att.ComponentSprite.rect.width / 2), yAxis);

                // draw texture to screen
                GUI.DrawTexture(
                new Rect(_att.CenterPoint, new Vector2
                         (_att.ComponentSprite.texture.width, 
                            _att.ComponentSprite.texture.height)),
                         _att.ComponentSprite.texture);

                // draw the sockets
                SocketController.DrawSockets();
            }
        }

        public static void DrawFooter(float width, float height)
        {
            GUILayout.BeginArea(new Rect(0, height - 90, width, height));

            EditorGUIUtility.labelWidth = 50f;
            
            for (int i = 0; i < 4; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _att.Symbols[i] = EditorGUILayout.TextField("Symbol",_att.Symbols[i], GUILayout.ExpandWidth(true));
                _att.Amounts[i] = EditorGUILayout.FloatField("Amount",_att.Amounts[i], GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear"))
            {
                ComponentController.Clear();
            }
            if (GUILayout.Button("Save"))
            {
                if (EditorUtility.DisplayDialog("Confirm Save",
                                                "Are you sure you want to save?", "Yes", "No"))
                {
                    ComponentController.SaveComponent();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}

