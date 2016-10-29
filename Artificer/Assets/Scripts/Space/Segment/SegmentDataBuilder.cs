using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Data.Space;
using Data.Shared;
using Utilities.Parellax;

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

            // Generate a selection of asteroid fields
            int fields = Random.Range(20, 60);
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
            for (int i = 0; i <= satellites; i++)
            {
                SyncList.Add(BuildSatellite(new Vector2
                    (Random.Range(100f, 4900f),
                     Random.Range(100f, 4900f))));
            }

            
            int clouds = Random.Range(12, 24);
            for (int i = 0; i <= clouds; i++)
            {
                SyncList.Add(BuildInterstellarCloud(new Vector2
                    (Random.Range(100f, 4900f),
                     Random.Range(100f, 4900f))));
            }

            int graves = Random.Range(2, 4);
            for (int i = 0; i <= graves; i++)
            {
                SyncList.Add(BuildGraveyard(new Vector2
                    (Random.Range(500f, 4500f),
                     Random.Range(500f, 4500f))));
            }

            return SyncList.ToArray();
        }

        public static ParellaxItem[] BuildNewBackground()
        {
            List<ParellaxItem> SyncList = new List<ParellaxItem>();

            // between 7 and 14 planets
            int planets = Random.Range(4, 7);
            float planetDistance = 500; // 500 units between each planet
            Vector2[] prevPPos = new Vector2[planets];

            for (int i = 0; i <= planets; i++)
            {
                bool tooclose = true;

                Vector2 newPos = Vector2.zero;

                while (tooclose)
                {
                    tooclose = false;

                    newPos = new Vector2
                    (Random.Range(100f, 4900f),
                     Random.Range(100f, 4900f));

                    foreach (Vector2 prev in prevPPos)
                    {
                        if (Vector2.Distance(newPos, prev) < planetDistance)
                            tooclose = true;
                    }
                }

                SyncList.Add(BuildPlanet(newPos));
            }

            // between 2 and 4 galaxies
            /*int galaxies = Random.Range(1, 3);
            float galaxyDistance = 1500; 
            Vector2[] prevGPos = new Vector2[galaxies];

            for (int i = 0; i <= galaxies; i++)
            {
                bool tooclose = true;

                Vector2 newPos = Vector2.zero;

                while (tooclose)
                {
                    tooclose = false;

                    newPos = new Vector2
                    (Random.Range(100f, 4900f),
                     Random.Range(100f, 4900f));

                    foreach (Vector2 prev in prevGPos)
                    {
                        if (Vector2.Distance(newPos, prev) < galaxyDistance)
                            tooclose = true;
                    }
                }

                SyncList.Add(BuildGalaxy(newPos));
            }*/

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

        private static ParellaxItem BuildPlanet(Vector2 planetPos)
        {
            // Create the start object
            ParellaxItem planet = new ParellaxItem();

            // Pick a random texture

            // Get directory list of textures
            string path = (System.Environment.CurrentDirectory + 
                    "\\Assets\\Resources\\Textures\\PlanetTextures");

            string[] fileEntries = Directory.GetFiles(path, "*.png");

            // assign info and pick random planet texture
            planet.X = planetPos.x;
            planet.Y = planetPos.y;
            planet.Distance = Random.Range(10, 90);
            planet.Texture = "Textures/PlanetTextures/" + 
                    System.IO.Path.GetFileNameWithoutExtension
                    (fileEntries[Random.Range(0, fileEntries.Length)]);
            planet.Type = "Planet";

            return planet;
        }

        private static ParellaxItem BuildGalaxy(Vector2 galaxyPos)
        {
            // Create the start object
            ParellaxItem galaxy = new ParellaxItem();

            // Pick a random texture

            // Get directory list of textures
            string path = (System.Environment.CurrentDirectory +
                    "\\Assets\\Resources\\Textures\\GalaxyTextures");

            string[] fileEntries = Directory.GetFiles(path, "*.png");

            // assign info and pick random planet texture
            galaxy.X = galaxyPos.x;
            galaxy.Y = galaxyPos.y;
            galaxy.Distance = Random.Range(50, 100);
            galaxy.Texture = "Textures/GalaxyTextures/" +
                    System.IO.Path.GetFileNameWithoutExtension
                    (fileEntries[Random.Range(0, fileEntries.Length)]);
            galaxy.Type = "Galaxy";

            return galaxy;
        }

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
            satellite._type = "_satellites";
            satellite._texturePath = "Textures/SatelliteTextures/satellite_01";
            satellite._name = "satellite";
            satellite._tag = "Satellite";
            satellite._visibleDistance = 50;

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
            aField._type = "_asteroids";
            aField._prefabPath = "Space/asteroid";
            aField._name = "asteroid field";
            aField._tag = "Asteroid";
            aField._visibleDistance = 300;

            // Set numeric data
            aField._position = position;
            aField._size =
                new Vector2(Random.Range(100, 200),
                Random.Range(100, 200));
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

        private static SegmentObject BuildInterstellarCloud(Vector2 position)
        {
            SegmentObject cloud = new SegmentObject();

            cloud._type = "_clouds";
            cloud._prefabPath = "Space/Cloud";
            cloud._name = "interstellar cloud";
            cloud._tag = "Untagged";
            cloud._visibleDistance = 600;

            cloud._position = position;
            cloud._count = Random.Range(6, 10);

            return cloud;   
        }

        private static SegmentObject BuildGraveyard(Vector2 position)
        {
            SegmentObject grave = new SegmentObject();

            grave._type = "_debris";
            grave._prefabPath = "Space/Wreck";
            grave._name = "ship graveyard";
            grave._tag = "Untagged";
            grave._visibleDistance = 600;

            grave._position = position;

            return grave;
        }

        #endregion
    }
}
