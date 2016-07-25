using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;
using Data.Space;

public class SpaceGenerator : MonoBehaviour {

	// GENERATORS
	ShipGenerator _shipGen;
	SegmentGenerator _segGen;
    RandomizedNodeGenerator _nodeGen;
    RaiderGenerator _raidGen;

	void Awake()
	{
		//Retreive all other generator components
		_shipGen = GetComponent<ShipGenerator> ();
		_segGen = GetComponent<SegmentGenerator> ();
        _nodeGen = GetComponent<RandomizedNodeGenerator>();
        _raidGen = GetComponent<RaiderGenerator>();
	}

	public void GenerateSpaceSegment(GameBaseAttributes data)
	{
		if (data.Segment == null)
			return;

        _shipGen.GenerateBase();
        _segGen.GenerateBase();

		/*
		 * Generate the player's ship
		 * */
		Vector2 playerPos = new Vector2 ();
        Vector2 up = Vector2.up;

		if (data.UserData.PreviousLocation != Vector2.zero)
        {
            playerPos = data.UserData.PreviousLocation;
            if(data.UserData.NewLocation.x != 0f)
                playerPos.x = data.UserData.NewLocation.x;
            if(data.UserData.NewLocation.y != 0f)
                playerPos.y = data.UserData.NewLocation.y;

            up = data.UserData.PreviousUp;
        }
		else
			playerPos = data.Segment.Size / 2;

        // caused issue when leaving station in new segment
        data.UserData.NewLocation = Vector2.zero;

		_shipGen.GeneratePlayerShip (data.UserData, playerPos, up);

		// create objects that follow player
		GameObject starField = (GameObject)Instantiate (Resources.Load ("Space/starfield")
			                                                , Vector3.zero, Quaternion.identity);
		starField.transform.parent = transform.Find ("PlayerCamera");
			
		GameObject starDrop = (GameObject)Instantiate (Resources.Load ("Space/Backdrops/stardrop_0")
			                                               , Vector3.zero, Quaternion.identity);
		starDrop.transform.localScale = new Vector3 (35f, 30f);
		starDrop.transform.parent = transform.Find ("PlayerCamera");
		starDrop.transform.Translate (new Vector3 (0f, 0f, 1f));

		/*
		 * ----------------------------------------------------------/
		 * */

		// Generate the segment's stations
		foreach (StationData station
		        in data.Segment.GetObjsOfType("station")) 
		{
			_segGen.GenerateStation
				(station);
            _nodeGen.GenerateNodes(5, System.String.Format("{0}Node", station.FactionName),
                                  station.Position.x-25f, station.Position.y-25f, 
                                 50f, 50f);
		}

		// Generate segments asteroids
		foreach (AsteroidData asteroid
		         in data.Segment.GetObjsOfType("asteroid")) 
		{
			_segGen.GenerateField
				(asteroid);
		}

		foreach (SatelliteData satellite
		         in data.Segment.GetObjsOfType("satellite")) 
		{
			_segGen.GenerateSatellite
				(satellite);
		}

        _nodeGen.GenerateNodes(100, "RaiderNode", 0, 0,
                               data.Segment.Size.x,
                               data.Segment.Size.y);

        // Create and init raider gen
        _raidGen = gameObject.AddComponent<RaiderGenerator>();
        _raidGen.BaseSpawnMaxDistance = 300f;
        _raidGen.BaseSpawnMinDistance = 200f;
        _raidGen.DistanceFromStation = 300f;
        _raidGen.ShipSpawnMaxDistance = 100f;
        _raidGen.ShipSpawnMinDistance = 50f;
        _raidGen.spawnDistance = 3f;
        _raidGen.spawnRadius = 20f;
        _raidGen.Initialize(data);
    }
}
