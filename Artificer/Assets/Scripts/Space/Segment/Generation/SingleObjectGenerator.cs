using UnityEngine;
using System.Collections;
using Data.Space;

namespace Space.Segment.Generator
{
    public class SingleObjectGenerator : MonoBehaviour
    {
        public GameObject GenerateSatellite(SegmentObject satellite, GameObject Prefab)
        {
            // Create game object for station
            GameObject newSatellite = Instantiate(Prefab);
            newSatellite.name = satellite._name;
            newSatellite.transform.position = satellite._position;

            // Assign texture to segment object
            newSatellite.GetComponent<SegmentObjectBehaviour>().
                AssignTexture(satellite._texturePath);
            
            return newSatellite;
        }

        public GameObject GeneratePlanet(SegmentObject planet)
        {
            // Create game object for station
            GameObject newPlanet = new GameObject();
            newPlanet.name = planet._name;
            newPlanet.transform.parent = this.transform;
            newPlanet.transform.position = planet._position;

            // Create visual element
            SpriteRenderer render = newPlanet.AddComponent<SpriteRenderer>();
            Sprite satelliteimg = Resources.Load(planet._texturePath, typeof(Sprite)) as Sprite;
            render.sprite = satelliteimg;
            render.sortingLayerName = "Background";

            return newPlanet;
        }

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

