using UnityEngine;
using System;
using System.Reflection; 
using System.Collections;
using System.Collections.Generic;
using Space.Ship.Components;
using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;

namespace Editor.Components
{
    public class CompTypeScriptUtil
    {
        private static Dictionary<string, System.Type > 
            _ComponentListenerReference = new Dictionary<string, System.Type>()
        {{"Components", typeof(ComponentListener)},
            {"Engines", typeof(EngineListener)},
            {"Maneuvers", typeof(ManeuverListener)},
            {"Rotors", typeof(RotorListener)},
            {"Launchers", typeof(LauncherListener)},
            {"Warps", typeof(WarpListener)},
            {"Weapons", typeof(WeaponListener)},
            {"Wells", typeof(WellListener)},
            {"Targeter", typeof(TargeterListener)},
            {"Shields", typeof(ShieldListener)},
            {"Construct", typeof(BuildListener)}};
        
        private static Dictionary<string, System.Type> 
            _ComponentAttributesReference = new Dictionary<string, System.Type>()
        {{"Components", typeof(Space.Ship.Components.Attributes.ComponentAttributes)},
            {"Engines", typeof(EngineAttributes)},
            {"Maneuvers", typeof(ManeuverAttributes)},
            {"Rotors", typeof(RotorAttributes)},
            {"Launchers", typeof(LauncherAttributes)},
            {"Warps", typeof(WarpAttributes)},
            {"Weapons", typeof(WeaponAttributes)},
            {"Wells", typeof(WellAttributes)},
            {"Targeter", typeof(TargeterAttributes)},
            {"Shields", typeof(ShieldAttributes)},
            {"Construct", typeof(BuildAttributes)}};
        
        // For now just create an instance of every 
        public static void AddBehavioursToObject(GameObject go, string type, GameObject old)
        {
            System.Type listenerName;
            System.Type attributeName;
            
            if (_ComponentListenerReference.ContainsKey(type))
            {
                listenerName = _ComponentListenerReference [type];
                
            } else
            {
                listenerName = typeof(ComponentListener);
            }
            
            if (_ComponentAttributesReference.ContainsKey(type))
            {
                attributeName = _ComponentAttributesReference [type];
            } else
            {
                attributeName = typeof(Space.Ship.Components.Attributes.ComponentAttributes);
            }
            
            if (old != null)
            {
                UnityEngine.Component oldListener = old.GetComponent(listenerName);
                if (oldListener != null)
                {
                    UnityEngine.Component new_component = go.AddComponent(listenerName);
                    foreach (FieldInfo f in listenerName.GetFields())
                    {
                        f.SetValue(new_component, f.GetValue(oldListener));
                    }
                }
                else
                {
                    go.AddComponent(listenerName);
                }
                
                UnityEngine.Component oldAttributes = old.GetComponent(attributeName);
                if (oldAttributes != null)
                {
                    UnityEngine.Component new_component = go.AddComponent(attributeName);
                    foreach (FieldInfo f in attributeName.GetFields())
                    {
                        f.SetValue(new_component, f.GetValue(oldAttributes));
                    }
                }
                else
                {
                    go.AddComponent(attributeName);
                }
            } 
            else 
            {
                go.AddComponent(listenerName);
                go.AddComponent(attributeName);
            }

            go.AddComponent<ComponentStyleUtility>();
        }
    }
}

