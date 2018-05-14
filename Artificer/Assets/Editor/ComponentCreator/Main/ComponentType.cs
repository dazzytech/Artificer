using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace ArtificerEditor.Components
{
    public class ComponentType
    {
        private string[] _componentTypes;

        private string _selectedComponent;

        public ComponentType()
        {
            // Retreive all the directories
            string[] compPaths = Directory.GetDirectories("Assets/Resources/Space/Ships");

            // Init comp list
            _componentTypes = new string[compPaths.Length];

            // Copy Component names into list
            int index = 0;
            foreach (string dir in compPaths)
            {
                _componentTypes[index] = System.IO.Path.GetFileName(dir);
                index++;
            }

            // Set selected
            _selectedComponent = _componentTypes [0];
        }

        public string[] GetList
        {
            get { return _componentTypes;}
        }

        public int Selected
        {
            get 
            {
                for(int i = 0; i < _componentTypes.Length; i++)
                {
                    if(_componentTypes[i] == _selectedComponent)
                        return i;
                }
                return -1;
            }

            set
            {
                _selectedComponent = _componentTypes[value];
            }
        }

        public string SelectedName
        {
            get 
            {
                return _selectedComponent;
            }
        }

        public void ImportType(GameObject import)
        {
            // add testing
            string dir = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(import));
            string importedType = System.IO.Path.GetFileName(dir);
            foreach (string other in _componentTypes)
            {
                if(other == importedType)
                {
                    _selectedComponent = importedType;
                    return;
                }
            }
            // component doesnt exist
            Selected = 0;
        }
    }
}

