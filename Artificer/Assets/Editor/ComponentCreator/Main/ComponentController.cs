using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
// Artificer defined
using Data.UI;
using Data.Space.Library;
using Space.Ship.Components;
using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;

namespace Editor.Components
{
    public class ComponentController
    {
        // Attributes Pointer
        private static ComponentAttributes _att;
        
        /// <summary>
        /// Sets the attributes.
        /// invoked in CC constructor
        /// </summary>
        /// <param name="att">Att.</param>
        public static void SetAttributes(ComponentAttributes att)
        {
            _att = att;
        }

        public static void SaveComponent()
        {
            // test to see if object already exists
            if (CheckComponentExists())
            {
                if (!EditorUtility.DisplayDialog("Overwrite Object",
                                                "Object already exists: are you sure you want to save?", "Yes", "No"))
                {
                    return;
                }
            }

            // Proceeding with save 

            // If object does not have name dont save
            if (_att.ComponentLabel == "")
            {
                // show warning and Quit
                if (EditorUtility.DisplayDialog("Name Needed",
                                                "Please enter name for component", "Ok"))
                {
                    return;
                }
            }

            // if a default sprite does not exist then cancel
            // If object does not have name dont save
            if (!_att.ComponentSprites.ContainsKey("default"))
            {
                // show warning and Quit
                if (EditorUtility.DisplayDialog("No default sprite",
                                                "There must be one sprite with the name 'default'", "Ok"))
                {
                    return;
                }
            }
            
            // create base objects to instantiate prefabs
            GameObject newGO = new GameObject();

            // create sprite object
            GameObject texGO = new GameObject();
            Sprite DefaultSprite = _att.ComponentSprites ["default"];

            // Initialize texture GO transform
            texGO.AddComponent<SpriteRenderer>();
            texGO.name = DefaultSprite.name;
            texGO.transform.parent = newGO.transform;
            
            // Create empty game objects for fireport and sockets
            foreach (ComponentObject obj in _att.Sockets)
            {
                GameObject ObjectGO = new GameObject();

                // convert pixel to units and position object
                Vector2 pos = new Vector2((obj.Position.x - DefaultSprite.rect.width/2)/100 ,
                                          (-(obj.Position.y - DefaultSprite.rect.height/2)/100));

                // define transform
                ObjectGO.transform.position = pos;
                ObjectGO.transform.parent = newGO.transform;
                
                // determine the name based on the object
                if(obj is ComponentSocket)
                {
                    ObjectGO.transform.name = string.Format("socket_{0}", 
                                                            ((ComponentSocket)obj).ID);
                    ObjectGO.AddComponent<SocketData>();
                }
                else if(obj is FirePoint)
                {
                    ObjectGO.transform.name = "firePort";
                }
                else if(obj is EnginePoint)
                {
                    ObjectGO.name = "Engine";
                    ObjectGO.transform.parent = newGO.transform;
                    
                    // add  particle emitter components
                    ObjectGO.AddComponent<EllipsoidParticleEmitter>();
                    ObjectGO.AddComponent<ParticleAnimator>();
                    ObjectGO.AddComponent<ParticleRenderer>();
                    ObjectGO.AddComponent<EngineSortingLayer>();
                    
                    // Set rotation depending on if engine or rotor
                    if(_att.Type.SelectedName == "Engines")
                        ObjectGO.transform.Rotate(90f, 0f, 0f);
                    else if(_att.Type.SelectedName == "Rotors")
                        ObjectGO.transform.Rotate(270f, 0f, 0f);
                }
            }

            GameObject old = null;
            // Before we override any pieces we must clone the original
            if (_att.ComponentGO != null)
                old = Object.Instantiate(_att.ComponentGO);
            
            // Create Prefab within the assets
            GameObject prefab = PrefabUtility.CreatePrefab(
                string.Format("Assets/Resources/Space/Ships/{0}/{1}", _att.Type.SelectedName, 
                          (_att.ComponentLabel +".prefab")), newGO,
                ReplacePrefabOptions.ConnectToPrefab);

            // Set variables to the created prefab and not the created instance
            SpriteRenderer rend = prefab.transform.Find(DefaultSprite.name).transform.GetComponent<SpriteRenderer>();
            
            rend.sprite = DefaultSprite;
            
            // Add socket behaviours to socket objects within the prefab
            foreach (Transform obj in prefab.transform)
            {
                if(obj.name.Contains("socket_"))
                {
                    // Get correct socket data
                    int SockID = 0;
                    ComponentSocket sock = null;
                    
                    if(int.TryParse(Regex.Match(obj.name, @"\d+").Value, out SockID))
                        sock = SocketController.GetSocket
                            (SockID);
                    else
                    {
                        Debug.Log("ComponentController: Save - Socket Not Found");
                        continue;
                    }
                    
                    if(sock != null)
                    {
                        SocketData behaviour = obj.GetComponent<SocketData>();
                        behaviour.Direction = sock.socketDirection;
                        behaviour.Type = sock.socketType;
                    }
                }
                if(obj.name.Contains("firePort"))
                {
                    obj.transform.tag = "Fire";
                }
                if(obj.name.Contains("Engine"))
                {
                    // define emitter variables
                    
                    // Set the particle as the default particle
                    obj.GetComponent<ParticleRenderer>().material = (Material)Resources.Load("Space/Materials/EngineMaterial");
                    
                    //set size grow?
                    obj.GetComponent<ParticleAnimator>().sizeGrow = -0.5699999f;
                    
                    // set particle emitter default
                    EllipsoidParticleEmitter part =
                        obj.GetComponent<EllipsoidParticleEmitter>();
                    part.emit = false;
                    part.localVelocity = new Vector3(0f,0f,3.823578f);
                    part.minSize = 0;
                    part.maxSize = .5f;
                    part.minEnergy = 0.2f;
                    part.maxEnergy = 0.3f;
                    part.minEmission = 1000f;
                    part.maxEmission = 1000f;
                }
            }
            
            // Add a boxcollider if the object is component
            if (_att.GOCollider)
            {
                BoxCollider2D collider = (BoxCollider2D)prefab.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(DefaultSprite.rect.width, 
                                            DefaultSprite.rect.height)*0.01f;
            }

            // change tag to body
            prefab.transform.tag = "Body";

            //prefab.transform.name = _att.ComponentLabel;
            
            // Add component scripts and set object as selected in editor
            CompTypeScriptUtil.AddBehavioursToObject(prefab, _att.Type.SelectedName, old);

            // Add material requirements to prefab object
            ComponentListener list = prefab.GetComponent<ComponentListener>();
            Space.Ship.Components.Attributes.ComponentAttributes att = list.GetAttributes();
            att.RequiredMats = new ConstructInfo[_att.Symbols.Length];
            for (int i = 0; i < _att.Symbols.Length; i++)
            {
                if(_att.Symbols[i] != null)
                {
                    ConstructInfo item = new ConstructInfo();
                    item.material = _att.Symbols[i];
                    item.amount = _att.Amounts[i];
                    att.RequiredMats[i] = item;
                }
            }

            list.Weight = _att.Weight;
            att.Integrity = _att.HP;
            att.Name = _att.ComponentName;

            // add the skin styles to the comp attributes
            att.componentStyles = new StyleInfo
                [_att.ComponentSprites.Count]; int index = 0;

            foreach (string name in _att.ComponentSprites.Keys)
            {
                StyleInfo info = new StyleInfo();
                info.name = name;
                info.sprite = _att.ComponentSprites [name];
                att.componentStyles[index++] = info;
            }

            // default by default
            att.currentStyle = "default";

            // assign components
            list.ComponentType = _att.Type.SelectedName;

            // Set component GO and active selection
            _att.ComponentGO = prefab.gameObject;
            Selection.activeGameObject=_att.ComponentGO;

            // delete the resulting gameobject created in scene
            GameObject.DestroyImmediate(newGO);
            GameObject.DestroyImmediate(old);
        }

        /// <summary>
        /// Checks the component exists.
        /// Invoked by the save function
        /// </summary>
        /// <returns><c>true</c>, if component exists was checked, <c>false</c> otherwise.</returns>
        private static bool CheckComponentExists()
        {
            if (Resources.Load(string.Format("Space/Ships/{0}/{1}", _att.Type.SelectedName, _att.ComponentLabel)) != null)
                return true;
            else
                return false;
        }

        // Clears 
        public static void Clear()
        {
            _att.ComponentGO = null;
            _att.ComponentName = "";
            _att.ComponentSprites.Clear();
            _att.GOCollider = false;
            _att.SocketCount = 0;
            _att.Type.Selected = 0;
            _att.EP = null;
            _att.Weight = 0;
            _att.HP = 0;
            _att.SelectedSocket = null;
            _att.Sockets.Clear();
            _att.ComponentSprite = null;
        }
    }
}

