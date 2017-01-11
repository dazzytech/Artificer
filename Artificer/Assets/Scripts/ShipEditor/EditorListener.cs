using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// Artificer
using Data.Shared;
using ShipComponents;
using UnityEngine.Cloud.Analytics;

namespace Construction.ShipEditor
{
    public class EditorListener : MonoBehaviour
    {
        private EditorAttributes _att;
        
        void Awake()
        {
            _att = GetComponent<EditorAttributes>();
        }

        public void SelectTab(string tab)
        {
            _att.ComponentList.Clear();
            
            _att.ComponentList = Resources.LoadAll("Space/Ships/"+tab, typeof(GameObject))
                .Cast<GameObject>()
                    .ToList(); 
            
            SendMessage("PopulateComponentList");
        }

        public void CreateItem(GameObject GO)
        {
            ComponentListener Con = GO.GetComponent<ComponentListener>();
            Data.Shared.Component component = new Data.Shared.Component();

            component.Folder = Con.ComponentType; 
            component.Name = GO.name;
            component.Direction = "up";
            component.Style = "default";
            // We have a match
            _att.Controller.BuildComponent(component);

            // Analytics
            _att.PiecesPlaced++;
        }

        public void SelectShip(ShipData ship)
        {
            _att.Controller.LoadExistingShip(ship);
        }

        public void CreateNewShip()
        {
            _att.Controller.CreateNewShip();
            _att.ShipsCreated++;
        }

        public void SaveShip()
        {
            _att.Controller.SaveShipData();
            SendMessage("UpdateShipList");
        }

        public void DeleteShip()
        {
            _att.Controller.ClearShip();
            SendMessage("UpdateShipList");
            _att.ShipsDeleted++;
        }

        public void GoBack()
        {
            // Save? Prompt?

            UnityAnalytics.CustomEvent("ExitShipEditor", new Dictionary<string, object>
                                               {{"elapsed time", Time.timeSinceLevelLoad},
                {"pieces placed", _att.PiecesPlaced},
                {"ships created", _att.ShipsCreated},
                {"ships deleted", _att.ShipsDeleted}});
        }
    }
}

