using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Data.Space;

namespace Space.Segment.Generator
{
    public class SegmentObjectGenerator : MonoBehaviour
    {
        #region ATTRIBUTES

        // Prefabs
        public GameObject SatellitePrefab;
        public GameObject AsteroidPrefab;
        public GameObject CloudPrefab;

        #endregion

        #region GENERATION

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

        public GameObject GenerateSatellite(SegmentObject segObj)
        {
            GameObject newSatellite = Instantiate(SatellitePrefab);
            newSatellite.transform.position = segObj._position;

            NetworkServer.Spawn(newSatellite);

            return newSatellite;
        }

        #region ASTEROID GENERATION

        /// <summary>
        /// Generate asteroid field and child asteroid objects
        /// </summary>
        /// <param name="segObj"></param>
        /// <returns></returns>
        public GameObject GenerateField(SegmentObject segObj)
        {
            GameObject field = Instantiate(AsteroidPrefab);
            field.transform.position = segObj._position;

            NetworkServer.Spawn(field);

            // for now only spawn one asteroid to make sure it works
            int ACount = 0;
            while (ACount < segObj._count)
            {
                GameObject asteroid = (GameObject)Instantiate(Resources.Load(segObj._prefabPath));
                asteroid.transform.parent = field.transform;
                // Give a random location and size;
                Vector2 location = new Vector2
                    (Random.Range(0f, segObj._size.x),
                     Random.Range(0f, segObj._size.y));
                asteroid.transform.localPosition = location;

                float scale = Random.Range(0f, 5f);
 
                NetworkServer.Spawn(asteroid);
                
                asteroid.GetComponent<AsteroidBehaviour>().
                    InitializeParameters(scale, field.GetComponent<NetworkIdentity>().netId);
                //behaviour.prospect = aData._symbols;
                ACount++;
            }

            return field;
        }

        public GameObject GenerateSingle(SegmentObject aData,
                                        Vector2 position, Vector2 velocity)
        {
            GameObject asteroid = (GameObject)Instantiate
                (Resources.Load(aData._texturePath));

            asteroid.transform.parent = this.transform;
            // Give a random location and size;
            Vector2 location = position;
            asteroid.transform.position = location;

            float scale = Random.Range(1f, 10f);
            asteroid.transform.localScale =
                new Vector3(scale,
                            scale, 1f);

            Rigidbody2D rb = asteroid.GetComponent<Rigidbody2D>();
            rb.mass = scale;
            rb.AddForce(velocity, ForceMode2D.Force);

            AsteroidBehaviour behaviour = asteroid.AddComponent<AsteroidBehaviour>();
            //behaviour.rockDensity = 20f * scale;
            //behaviour.prospect = aData._symbols;

            return asteroid;
        }

        #endregion

        public GameObject GenerateCloud(SegmentObject segObj)
        {
            GameObject newCloud = Instantiate(CloudPrefab);
            newCloud.transform.position = segObj._position;

            NetworkServer.Spawn(newCloud);

            // for now only spawn one asteroid to make sure it works
            int CloudCount = 0;
            while (CloudCount < segObj._count)
            {
                GameObject cloud = (GameObject)Instantiate(Resources.Load(segObj._prefabPath));
                cloud.transform.parent = newCloud.transform;

                // Give a random location and size;
                Vector2 location = new Vector2
                    (Random.Range(-500, 500),
                     Random.Range(-500, 500));

                cloud.transform.localPosition = location;

                NetworkServer.Spawn(cloud);

                cloud.GetComponent<InterstellarCloudBehaviour>().
                    InitializeParameters(newCloud.GetComponent<NetworkIdentity>().netId);
                CloudCount++;
            }

            return newCloud;
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

        #endregion
    }
}

