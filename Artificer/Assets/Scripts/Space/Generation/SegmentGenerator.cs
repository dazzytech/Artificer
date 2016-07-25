using UnityEngine;
using System.Collections;
// Artificer
using Data.Shared;
using Data.Space;

public class SegmentGenerator: MonoBehaviour
{
    GameObject home;

    public void GenerateBase()
    {
        home = new GameObject();
        home.name = "_stations";
        home.transform.parent = this.transform;
    }

	public void GenerateStation(StationData station)
	{
		// Create game object for station
		GameObject newStation = new GameObject();
		newStation.name = station.Name;
		newStation.tag = "Station";
		newStation.transform.parent = home.transform;
		newStation.transform.position = station.Position;
        newStation.layer = 2;
        GameObject.Find("_gui").
            SendMessage("AddUIPiece", newStation.transform);

        // Create boxCollider trigger and station behaviour
        BoxCollider2D col = newStation.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(20f, 20f);
        col.offset = new Vector2(0f, .75f);
        StationBehaviour behaviour =  newStation.AddComponent<StationBehaviour>();
        behaviour.Station = station;

		// Create visual element
		SpriteRenderer render = newStation.AddComponent<SpriteRenderer>();
		Sprite stationimg = Resources.Load(station.GetPath, typeof(Sprite)) as Sprite;
		render.sprite = stationimg;
		render.sortingLayerName = "BackgroundObjects";

		// Add attribute scripts
	}

	public void GenerateField(AsteroidData aData)
	{
		GameObject field = new GameObject ();
		field.name = "asteroidBelt";
		field.tag = "Asteroid";
		field.transform.parent = this.transform;
		field.transform.position = aData.Position;
		
		// for now only spawn one asteroid to make sure it works
		int ACount = 0;
		while (ACount < aData.Count) {
			GameObject asteroid = (GameObject)Instantiate (Resources.Load (aData.GetPath));
			asteroid.transform.parent = field.transform;
			// Give a random location and size;
			Vector2 location = new Vector2
				(Random.Range (0f, aData.Width),
				 Random.Range (0f, aData.Height));
			asteroid.transform.localPosition = location;
			
			float scale = Random.Range (0f, 5f);
			asteroid.transform.localScale =
				new Vector3(scale,
				            scale, 1f);
			asteroid.GetComponent<Rigidbody2D>().mass = scale;
			
            AsteroidBehaviour behaviour = asteroid.AddComponent<AsteroidBehaviour>();
            behaviour.rockDensity = 20f * scale;
            behaviour.prospect = aData.Prospect;
			ACount++;
		}
	}

	public void GenerateSatellite(SatelliteData satellite)
	{
		// Create game object for station
		GameObject newSatellite = new GameObject();
		newSatellite.name = satellite.Name;
		newSatellite.tag = "Satellite";
		newSatellite.transform.parent = this.transform;
		newSatellite.transform.position = satellite.Position;
		
		// Create visual element
		SpriteRenderer render = newSatellite.AddComponent<SpriteRenderer>();
		Sprite satelliteimg = Resources.Load(satellite.GetPath, typeof(Sprite)) as Sprite;
		render.sprite = satelliteimg;
		render.sortingLayerName = "BackgroundObjects";

		// colliders
		newSatellite.AddComponent<BoxCollider2D> ();
		Rigidbody2D r = newSatellite.AddComponent<Rigidbody2D> ();
		r.gravityScale = 0;
		
		// Add attribute scripts
	}
}

