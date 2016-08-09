using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Data.Space;
using Data.Shared;

namespace Space.Segment
{
    /// <summary>
    /// Segment data builder.
    /// Responsible for generating segment objects 
    /// of all types and returning to caller
    /// </summary>
    public class SegmentDataBuilder 
    {
        #region PUBLIC INTERACTION

        /// <summary>
        /// Builds the new segment.
        /// randomly generated objects
        /// GENERATED OBJECTS:
        /// asteroid feilds
        /// satellites
        /// pending : debris
        /// planet 
        /// satellites orbit the planet
        /// 
        /// </summary>
        /// <returns>The new segment.</returns>
        public static SegmentObject[] BuildNewSegment()
        {
            List<SegmentObject> SyncList = new List<SegmentObject>();

            // between 7 and 14 planets
            //int planets = Random.Range(7, 14);
            //float planetDistance = 300; // 300 units between each planet
            //Vector2[] prevPPos = new Vector2[planets];

            /*for(int i = 0; i <= planets; i++)
            {
                bool tooclose = true;

                Vector2 newPos = Vector2.zero;

                while (tooclose)
                {
                    tooclose = false;

                    newPos = new Vector2
                    (Random.Range(100f, 4900f),
                     Random.Range(100f, 4900f));

                    foreach(Vector2 prev in prevPPos)
                    {
                        if (Vector2.Distance(newPos, prev) < planetDistance)
                            tooclose = true;
                    }
                }

                //SyncList.Add(BuildPlanet(newPos));
            }*/
             
            // Generate a selection of asteroid fields
            int fields = Random.Range(6, 20);
            for (int i = 0; i <= fields; i++)
            {
                int value = Random.Range(0, 3);
                SyncList.Add(BuildAsteroidField
                    (new Vector2
                    (Random.Range(100f, 4900f),
                     Random.Range(100f, 4900f)),
                     value == 0? "low": value == 1? "medium": "high"));
            }

            // Generate a small amount of satellites (temp)
            int satellites = Random.Range(12, 24);
            {
                SyncList.Add(BuildSatellite(new Vector2
                    (Random.Range(100f, 4900f),
                     Random.Range(100f, 4900f))));
            }

            return SyncList.ToArray();
        }

        #endregion

        #region DATA BUILDERS

        /*
        /// <summary>
        /// Builds the station.
        /// in future this will be 
        /// done in another class with defined vars
        /// </summary>
        /// <returns>The station.</returns>
        private static StationData BuildStation(Vector2 planet)
        {
            StationData station = new StationData();
            station.Position = new Vector2
                (Random.Range(planet.x - 200f, planet.x + 200f),
                 Random.Range(planet.y - 200f, planet.y + 200f));

            station.SetAttributes("New Station", "Station_External_Small_01");

            return station;
        }*/

        /*private static SegmentObject BuildPlanet(Vector2 planetPos)
        {
            // Create the start object
            SegmentObject planet = new SegmentObject();

            // Pick a random texture

            // Get directory list of textures
            string path = (System.Environment.CurrentDirectory + 
                "\\Assets\\Resources\\Textures\\PlanetTextures");

            string[] fileEntries = Directory.GetFiles(path);

            // assign info and pick random planet texture
            planet._type = "planet";
            planet._name = "planet";
            planet._texturePath = "Textures/PlanetTextures/" + 
                System.IO.Path.GetFileNameWithoutExtension
                (fileEntries[Random.Range(0, fileEntries.Length)]);

            planet._position = planetPos;

            return planet;
        }*/

        /// <summary>
        /// Builds a satellite object and returns it
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        private static SegmentObject BuildSatellite(Vector2 planet)
        {
            // build base object
            SegmentObject satellite = new SegmentObject();

            // Set descriptive info
            satellite._type = "satellite";
            satellite._texturePath = "Textures/SatelliteTextures/satellite_01";
            satellite._name = "satellite";

            // Set numeric data
            satellite._position = new Vector2
                (Random.Range(planet.x - 100f, planet.x + 100f),
                 Random.Range(planet.y - 100f, planet.y + 100f));

            return satellite;
        }

        private static SegmentObject BuildAsteroidField(Vector2 position, string value)
        {
            // build base object
            SegmentObject aField = new SegmentObject();

            // Set descriptive info
            aField._type = "asteroid";
            aField._texturePath = "Space/asteroid";
            aField._name = "asteroid field";

            // Set numeric data
            aField._position = position;
            aField._size =
                new Vector2(Random.Range(100, 500),
                Random.Range(100, 500));
            aField._count = Random.Range(50, 100);

            /*switch (value)
            {
                case "low":
                    {
                        // only tin and copper 50/50
                        aField._symbols = new string[100];
                        int i = 0;
                        for (; i < 10; i++)
                            aField._symbols[i] = "Sn";
                        for (; i < 20; i++)
                            aField._symbols[i] = "Cu";
                        for (; i < 40; i++)
                            aField._symbols[i] = "Fe";
                        for (; i < 50; i++)
                            aField._symbols[i] = "Ni";
                        for (; i < 60; i++)
                            aField._symbols[i] = "Co";
                        for (; i < 70; i++)
                            aField._symbols[i] = "Zn";
                        for (; i < 80; i++)
                            aField._symbols[i] = "Pb";
                        for (; i < 100; i++)
                            aField._symbols[i] = "Al";
                        break;
                    }
                case "medium":
                    {
                        // only tin and copper 50/50
                        aField._symbols = new string[100];
                        int i = 0;
                        for (; i < 7; i++)
                            aField._symbols[i] = "Sn";
                        for (; i < 14; i++)
                            aField._symbols[i] = "Cu";
                        for (; i < 30; i++)
                            aField._symbols[i] = "Fe";
                        for (; i < 35; i++)
                            aField._symbols[i] = "Ni";
                        for (; i < 45; i++)
                            aField._symbols[i] = "Co";
                        for (; i < 50; i++)
                            aField._symbols[i] = "Zn";
                        for (; i < 70; i++)
                            aField._symbols[i] = "Pb";
                        for (; i < 80; i++)
                            aField._symbols[i] = "Al";
                        for (; i < 90; i++)
                            aField._symbols[i] = "Ag";
                        for (; i < 100; i++)
                            aField._symbols[i] = "Au";
                        break;
                    }
                case "high":
                    {
                        // only tin and copper 50/50
                        aField._symbols = new string[100];
                        int i = 0;
                        for (; i < 7; i++)
                            aField._symbols[i] = "Sn";
                        for (; i < 14; i++)
                            aField._symbols[i] = "Cu";
                        for (; i < 30; i++)
                            aField._symbols[i] = "Fe";
                        for (; i < 35; i++)
                            aField._symbols[i] = "Ni";
                        for (; i < 45; i++)
                            aField._symbols[i] = "Co";
                        for (; i < 50; i++)
                            aField._symbols[i] = "Zn";
                        for (; i < 70; i++)
                            aField._symbols[i] = "Pb";
                        for (; i < 80; i++)
                            aField._symbols[i] = "Al";
                        for (; i < 87; i++)
                            aField._symbols[i] = "Ag";
                        for (; i < 95; i++)
                            aField._symbols[i] = "Au";
                        for (; i < 100; i++)
                            aField._symbols[i] = "Pt";
                        break;
                    }
            }
            */
            return aField;
        }

        #endregion
    }
}
