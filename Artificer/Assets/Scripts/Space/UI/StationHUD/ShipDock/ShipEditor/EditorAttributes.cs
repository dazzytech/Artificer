using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Construction.ShipEditor
{
    public class EditorAttributes : MonoBehaviour
    {
        public GameObject TabPrefab;
        public GameObject TabHeader;
        
        public GameObject ItemPrefab;
        public GameObject ItemPanel;
        public Scrollbar ItemScroll;

        public GameObject ShipItemPrefab;
        public GameObject NewItemPrefab;
        public GameObject ShipItemPanel;
        
        public List<GameObject> ComponentList;
        public List<GameObject> StarterList;
        //public ShipEditorController Controller;

        // Analytics Data
        public int ShipsCreated;
        public int ShipsDeleted;
        public int PiecesPlaced;
       
    }

}