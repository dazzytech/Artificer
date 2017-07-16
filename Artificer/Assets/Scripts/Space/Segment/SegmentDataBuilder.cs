using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Data.Space;
using Space.CameraUtils;

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
        public static SegmentObjectData[] BuildNewSegment()
        {
            List<SegmentObjectData> SyncList = new List<SegmentObjectData>();
            
            // GENERATE ASTEROIDS

            // standard asteroid fields
            int fields = Random.Range(10, 20);
            for (int i = 0; i <= fields; i++)
            {
                int value = Random.Range(0, 3);
                SyncList.Add(BuildAsteroidField
                    (new Vector2
                    (Random.Range(100f, 4900f),
                     Random.Range(100f, 4900f)),
                     "Space/Destructable/Asteroid",
                     "asteroid field"));
            }

            // high value fields
            fields = Random.Range(2, 5);
            for (int i = 0; i <= fields; i++)
            {
                int value = Random.Range(0, 3);
                SyncList.Add(BuildAsteroidField
                    (new Vector2
                    (Random.Range(100f, 4900f),
                     Random.Range(100f, 4900f)),
                     "Space/Destructable/AsteroidHighVal",
                     "high val asteroid field"));
            }

            // plutonium fields
            fields = Random.Range(3, 5);
            for (int i = 0; i <= fields; i++)
            {
                int value = Random.Range(0, 3);
                SyncList.Add(BuildAsteroidField
                    (new Vector2
                    (Random.Range(100f, 4900f),
                     Random.Range(100f, 4900f)),
                     "Space/Destructable/AsteroidPlut",
                     "plut asteroid field"));
            }

            // Generate a small amount of satellites (temp)
            int satellites = Random.Range(2, 10);
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

        public static SyncPI[] BuildNewBackground()
        {
            List<SyncPI> SyncList = new List<SyncPI>();

            // between 7 and 14 planets
            int planets = Random.Range(1, 4);
            float planetDistance = 900; // 500 units between each planet
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

            return SyncList.ToArray();
        }

        #endregion

        #region DATA BUILDERS

        private static SyncPI BuildPlanet(Vector2 planetPos)
        {
            // Create the start object
            SyncPI planet = new SyncPI();

            // Pick a random texture

            // Get directory list of textures
            string path = (System.Environment.CurrentDirectory + 
                    "\\Assets\\Resources\\Textures\\PlanetTextures");

            string[] fileEntries = Directory.GetFiles(path, "*.png");

            // assign info and pick random planet texture
            planet.X = planetPos.x;
            planet.Y = planetPos.y;
            planet.Distance = Random.Range(90, 200);
            planet.Texture = "Textures/PlanetTextures/" + 
                    System.IO.Path.GetFileNameWithoutExtension
                    (fileEntries[Random.Range(0, fileEntries.Length)]);
            planet.Type = "Planet";

            return planet;
        }

        private static SyncPI BuildGalaxy(Vector2 galaxyPos)
        {
            // Create the start object
            SyncPI galaxy = new SyncPI();

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
        private static SegmentObjectData BuildSatellite(Vector2 planet)
        {
            // build base object
            SegmentObjectData satellite = new SegmentObjectData();

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

        private static SegmentObjectData BuildAsteroidField(Vector2 position, 
            string value, string name)
        {
            // build base object
            SegmentObjectData aField = new SegmentObjectData();

            // Set descriptive info
            aField._type = "_asteroids";
            aField._prefabPath = value;
            aField._name = name;
            aField._tag = "Asteroid";
            aField._visibleDistance = 1010;

            // Set numeric data
            aField._position = position;
            aField._size =
                new Vector2(Random.Range(300, 1000),
                Random.Range(300, 1000));
            aField._count = Random.Range(50, 100);

            return aField;
        }

        private static SegmentObjectData BuildInterstellarCloud(Vector2 position)
        {
            SegmentObjectData cloud = new SegmentObjectData();

            cloud._type = "_clouds";
            cloud._prefabPath = "Space/Cloud";
            cloud._name = "interstellar cloud";
            cloud._tag = "Untagged";
            cloud._visibleDistance = 600;

            cloud._position = position;
            cloud._count = Random.Range(6, 10);

            return cloud;   
        }

        private static SegmentObjectData BuildGraveyard(Vector2 position)
        {
            SegmentObjectData grave = new SegmentObjectData();

            grave._type = "_debris";
            grave._prefabPath = "Space/Wreck";
            grave._name = "ship graveyard";
            grave._tag = "Untagged";
            grave._visibleDistance = 110;

            grave._position = position;
            grave._size =
                 new Vector2(Random.Range(20, 100),
                 Random.Range(20, 100));
            grave._count = Random.Range(5, 20);

            return grave;
        }

        #endregion
    }
}
