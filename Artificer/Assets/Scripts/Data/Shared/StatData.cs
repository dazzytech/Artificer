using UnityEngine;
using System.Collections;

namespace Data.Shared
{
    /// <summary>
    /// Stats and achievements.
    /// encapsulated object that stores multiplayer stats
    /// </summary>
    public class StatData
    {
        /*
         * STATS - PLANNED AND CURRENT
         * experiance points
         * total kills
         * total deaths
         * KD ratio
         * Games Won
         * Games Lost
         * Games Played
         * */
        public float xp;
        public int total_kills;
        public int total_deaths;
        public float kd_ratio;
        public int games_won;
        public int games_lost;
        public int games_played;
    }
}
