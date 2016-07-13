using UnityEngine;
using System.Collections;
using Data.Space;

namespace Space.Segment
{
    public class StationGenerator : MonoBehaviour
    {
        /*public GameObject GenerateStation(StationData station)
        {
            // Create game object for station
            GameObject newStation = new GameObject();
            newStation.name = station.Name;
            newStation.tag = "Station";
            newStation.transform.parent = this.transform;
            newStation.transform.position = station.Position;
            newStation.layer = 0;
            GameObject.Find("_gui").
                SendMessage("AddUIPiece", newStation.transform);
            
            // Create boxCollider trigger and station behaviour
            BoxCollider2D col = newStation.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(5f, 5f);
            col.offset = new Vector2(0f, .75f);

            StationBehaviour behaviour = newStation.AddComponent<StationBehaviour>();
            behaviour.Station = station;
            
            // Create visual element
            SpriteRenderer render = newStation.AddComponent<SpriteRenderer>();
            Sprite stationimg = Resources.Load(station.GetPath, typeof(Sprite)) as Sprite;
            render.sprite = stationimg;
            render.sortingLayerName = "BackgroundObjects";
            render.sortingOrder = -1;

            return newStation;
        }*/
    }
}

