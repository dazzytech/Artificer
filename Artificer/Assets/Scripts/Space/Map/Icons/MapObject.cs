﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.Map
{
    /// <summary>
    /// Storage class
    /// for map items with
    /// info about that item
    /// </summary>
    [System.Serializable]
    public class MapObject
    {
        /// <summary>
        /// GUI icon that shows
        /// </summary>
        public Transform Icon;
        /// <summary>
        /// position where the icon is displayed
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// For areas, width and height of the icon
        /// </summary>
        public Vector2 Size;
        /// <summary>
        /// colour of the map icon
        /// </summary>
        public Color Color;
        /// <summary>
        /// Original points that define the borders of a segment
        /// </summary>
        public Vector2[] Points;
        /// <summary>
        /// Points that are manipulated within the map controller
        /// and used by GL to draw bounds
        /// </summary>
        public Vector2[] RenderPoints;
        /// <summary>
        /// What type of object we are displaying
        /// </summary>
        public MapObjectType Type;
        /// <summary>
        /// Reference to the object to check for updates
        /// </summary>
        public Transform Ref;
        /// <summary>
        /// What team, if any, this object belongs to
        /// </summary>
        public int TeamID;
        /// <summary>
        /// If true, object is not displayed
        /// </summary>
        public bool Hidden;
        /// <summary>
        /// Returns if the object currently 
        /// exists in scene
        /// </summary>
        public bool Exists
        {
            get
            {
                return Ref != null || Type == MapObjectType.TEAM;
            }
        }

        public int Relation
        {
            get
            {
                if (TeamID == -1|| SystemManager.Space.Team == null)
                    return -1;

                if (SystemManager.Space.Team.EnemyTeam.Contains(TeamID))
                {
                    return 0;
                }
                else if (SystemManager.Space.TeamID == TeamID)
                    return 1;
                else
                    return -1;
            }
        }
    }
}
