using UnityEngine;
using System.Collections;
// Artificer
using Data.Shared;

/// <summary>
/// Raider generator.
/// Raiders generate random on segment depending on 
/// Threat level on segment
/// </summary>
public class RaiderGenerator : MonoBehaviour
{
    private float timeTillNextTick;

    private string threat;
    private static int raidCount = 0;

    private float width;
    private float height;

    private ShipGenerator shipGen;
    private ShipData raiderShip;

    private bool running;

    public float spawnRadius = 20f;
    public float spawnDistance = 3f;

    public float BaseSpawnMaxDistance = 300f;
    public float BaseSpawnMinDistance = 200f;

    public float ShipSpawnMaxDistance = 100f;
    public float ShipSpawnMinDistance = 50f;

    public float DistanceFromStation = 300f;

    public void Initialize(GameBaseAttributes data)
    {
        // dimensions of spawn area
        width = data.Segment.Size.x;
        height = data.Segment.Size.y;

        // Ship generation data
        shipGen = GetComponent<ShipGenerator>();
        raiderShip = data.PrebuiltShips.GetShip("ship_neutral_combat_small_01");

        // Raider threat
        threat = data.Segment.ThreatLevel;

        running = false;
    }

    void Update()
    {
        if (!running)
        {
            StartCoroutine("SearchPossibleRaidTarget");
        }
    }

    /// <summary>
    /// Generates the base raid.
    /// Picks a random station
    /// and generates a group of raider within a certain distance of the
    /// station and passes it as a target to the raider AI Controller
    /// </summary>
    /// <returns>The base raid.</returns>
    IEnumerator SearchPossibleRaidTarget()
    {
        running = true;

        // Create a spawn position away from a station
        Transform[] stationList = GameObject.Find("_stations")
            .GetComponentsInChildren<Transform>();

        // Determine which we are looking for
        if (Random.Range(-1, 1) == 0)
        {
            // test for ship
            ShipAttributes[] shipAttList = GameObject.Find("_ships")
                .GetComponentsInChildren<ShipAttributes>();

            Transform[] shipList = new Transform[shipAttList.Length];

            for(int i = 0; i < shipAttList.Length; i++)
            {
                shipList[i] = shipAttList[i].transform;
            }

            if (shipList.Length <= 0)
                yield return null;

            foreach (Transform ship in shipList)
            {
                if(ship.name == "Raider")
                    continue;
                
                bool stray = true;
                foreach(Transform station in stationList)
                {
                    if(Vector3.Distance(ship.position, station.position) 
                       < DistanceFromStation)
                    {
                        // possible target
                        stray = false;
                    }
                }
                
                if(stray)
                {
                    GenerateRaid(ship, "station");
                }
            }
        } 
        else
        {
            if (stationList.Length == 1)
                yield return null; 

            Transform station = stationList [Random.Range(1, stationList.Length)];

            GenerateRaid(station, "station");
        }

        switch (threat)
        {
            case "low":
                timeTillNextTick = Random.Range(140f, 180f);
                break;
            case "medium":
                timeTillNextTick = Random.Range(100f, 140f);
                break;
            case "high":
                timeTillNextTick = Random.Range(90f, 120f);
                break;
            default:
                timeTillNextTick = Random.Range(140f, 180f);
                break;
        }

        yield return new WaitForSeconds(timeTillNextTick);

        running = false;
    }

    /// <summary>
    /// Generates a group of raiders with a target
    /// </summary>
    /// <param name="station">Station.</param>
    private void GenerateRaid
        (Transform target, string targetType)
    {     
        // Generate Raiders corresponding to threat level
        int numOfRaiders = Random.Range(1,4);
        
        int index = 0;
        
        ArrayList raidGroup = new ArrayList();
        
        float spawnX = 0;
        float spawnY = 0;

        while (true)
        {
            switch(targetType)
            {
                // station positioning
                case "station":

                    spawnX = Random.Range(target.position.x - BaseSpawnMaxDistance,
                                          target.position.x + BaseSpawnMaxDistance);
                    
                    spawnY = Random.Range(target.position.y - BaseSpawnMaxDistance,
                                          target.position.y + BaseSpawnMaxDistance);
                    
                    if(Vector3.Distance(new Vector3(spawnX, spawnY),
                                    target.position) < BaseSpawnMinDistance)
                    {
                        // too close to a station
                        continue;
                    }
                    break;

                case "ship":
                    
                    spawnX = Random.Range(target.position.x - ShipSpawnMaxDistance,
                                          target.position.x + ShipSpawnMaxDistance);
                    
                    spawnY = Random.Range(target.position.y - ShipSpawnMaxDistance,
                                          target.position.y + ShipSpawnMaxDistance);
                    
                    if(Vector3.Distance(new Vector3(spawnX, spawnY),
                                    target.position) < ShipSpawnMinDistance)
                    {
                        // too close to a station
                        continue;
                    }
                    break;
            }
            break;
        }

        while (index < numOfRaiders)
        {
            if(raidCount > 20)
                break;
            
            Vector3 position = new Vector3
                (Random.Range(spawnX - spawnRadius
                              ,spawnX + spawnRadius),
                 Random.Range(spawnY - spawnRadius
                             ,spawnY + spawnRadius));
            
            while(true)
            {
                foreach(SimpleRaiderController
                        others in raidGroup)
                {
                    if(Vector3.Distance(new Vector3(spawnX, spawnY),
                                        others.transform.position)
                       < spawnDistance)
                    {
                        // too close to a station
                        continue;
                    }
                }
                break;
            }
            
            Transform newShip = shipGen.GenerateShip
                (raiderShip, position,
                 Vector3.up);
            
            newShip.name = "Raider";
            SimpleRaiderController raider =
                newShip.gameObject.AddComponent<SimpleRaiderController>();
            raider.SetController
                (newShip.gameObject.GetComponent<ShipMessegeController>());
            raider.target = target;

            if(GameObject.Find("_gui") != null)
                GameObject.Find("_gui").
                    SendMessage("AddUIPiece", newShip, 
                            SendMessageOptions.DontRequireReceiver);
            
            raidGroup.Add(raider);
            
            index++;
            raidCount++;
        }
        
        foreach (SimpleRaiderController raider in raidGroup)
        {
            raider.SetFaction("Raider", new string[1]{"RaiderNode"});
            raider.SetGroup(raidGroup);
        }
    }

    public static void KillRaider()
    {
        raidCount--;
    }
}

