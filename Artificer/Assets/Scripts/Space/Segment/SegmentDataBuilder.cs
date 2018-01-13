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
        #region ATTRIBUTES

        private static GameParameters m_param;

        #region ASTEROIDS

        private static List<Rect> m_astBoundaries;

        private static string[,] m_asteroidTypes
            = new string[3, 2]
                {{"Space/Destructable/Asteroid",
                  "asteroid field"},
                 {"Space/Destructable/AsteroidPlut",
                  "plut asteroid field"},
                 {"Space/Destructable/AsteroidHighVal",
                  "high val asteroid field"}};

        private static float m_astMaxSize = 700f;

        private static float m_astMinSize = 300f;

        private static int m_AstCount;

        #endregion

        #endregion

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
        public static SegmentObjectData[] BuildNewSegment(GameParameters param)
        {
            // Initialize parameters 
            m_param = param;

            List<SegmentObjectData> SyncList = new List<SegmentObjectData>();

            // GENERATE ASTEROIDS
            m_astBoundaries = new List<Rect>();

            for (int i = 0; i < 3; i++)
            {
                m_AstCount = Random.Range(4 - i, 10 - i);
                for (int count = 0; count <= m_AstCount; count++)
                {
                    SyncList.Add(BuildAsteroidField(i));
                }
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

        #region PRIVATE UTILITIES

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

        /// <summary>
        /// Builds an asteroid field, 
        /// type is based on int
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static SegmentObjectData BuildAsteroidField(int type)
        {
            // build base object
            SegmentObjectData aField = new SegmentObjectData();

            float size = 0;

            // Create boundary for the asteroid
            Rect bounds = BuildBounds(ref size, type);

            if (m_astBoundaries == null)
                m_astBoundaries = new List<Rect>();
            else
            {
                int attempts = 10;
                while(attempts >= 0)
                {
                    bool inRange = false;

                    foreach (Rect r in m_astBoundaries)
                    {
                        if (Math.InRange(bounds, r, 100f))
                        {
                            inRange = true;
                            break;
                        }
                    }

                    if (inRange)
                    {
                        bounds = BuildBounds(ref size, type);

                        attempts--;
                    }
                    else
                        break;
                }
            }

            m_astBoundaries.Add(bounds);

            // Set descriptive info
            aField._type = "_asteroids";
            aField._prefabPath = m_asteroidTypes[type,0];
            aField._name = m_asteroidTypes[type, 1];
            aField._tag = "Asteroid";
            aField._visibleDistance = size + 100f;

            // Set numeric data
            aField._position = bounds.min;
            aField._size = bounds.size;
            aField._subType = type;
            aField._border = BuildRegion(bounds, size);

            aField._count = 
                Mathf.CeilToInt(size * Random.Range(.5f, 1f));

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
            cloud._size = new Vector2(500, 500);
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

        #region ASTEROID

        /// <summary>
        /// plots points witin the asteriod 
        /// rect at 8 points from center extending
        /// to half-size
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private static Vector2[] BuildRegion(Rect bounds, float size)
        {
            List<Vector2> region = new List<Vector2>();

            // create an array of angles we 
            // place points at
            float[] angles = new float[8]
            {
                0, 45, 90, 135, 180, 225, 270, 315
            };

            // Create points from the center
            // with magnitute of random range
            for (int i = 0; i < 8; i++)
            {
                // create the first point at the top
                Vector2 point = bounds.center + new Vector2(
                    (float)Mathf.Cos(angles[i] * Mathf.Deg2Rad), 
                    (float)Mathf.Sin(angles[i] * Mathf.Deg2Rad)) *
                    (size * Random.Range(0.2f, 0.5f));

                // Add to our storage
                region.Add(point);
            }

            return region.ToArray();
        }

        private static Rect BuildBounds(ref float size, int type)
        {
            // Change the size depending on type 
            size = Random.Range(m_astMinSize, m_astMaxSize) / (type + 1);

            Vector2 position = Vector2.zero;

            switch (type)
            {
                case 0:
                    // this is a default asteroid
                    // Spawn them closer to station
                    // first 5 at team A and rest near team B
                    Vector2 focus;
                    if (m_astBoundaries.Count <= m_AstCount/2)
                        focus = m_param.TeamASpawn;
                    else
                        focus = m_param.TeamBSpawn;

                    // Spawn field within range of station
                    position = 
                        Math.RandomWithinRange(focus, size*.5f + 100f, size*.5f + 600f);

                    // Clamp within the ma bounds
                    position.x = Mathf.Clamp(position.x, size*.5f, SystemManager.Size.width - size);
                    position.y = Mathf.Clamp(position.y, size*.5f, SystemManager.Size.height - size);
                    break;

                default:
                    bool inRange = true;

                    while (inRange)
                    {
                        inRange = false;

                        // For now spawn within 2000 of map center and distance 
                        // either station
                        position =
                            Math.RandomWithinRange(SystemManager.Size.center, 0, size * .5f + 1500f);

                        if (Vector3.Distance(position, m_param.TeamASpawn) < 1000f)
                            inRange = true;

                        if (Vector3.Distance(position, m_param.TeamBSpawn) < 1000f)
                            inRange = true;
                    }
                 break;
            }

            // position now defines the center point therefore
            // adjust accordingly
            return new Rect(position - new Vector2(size*.5f, size*.5f), new Vector2(size, size));
        }

        #endregion

        #endregion
    }
}
