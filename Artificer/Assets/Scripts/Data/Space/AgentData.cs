using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Space
{
    public class AgentData
    {
        #region STATE DATA

        public class StateData
        {
            public int[] TransIDs;
            public int[] StateIDs;

            public string type;
        }

        #endregion

        #region ATTRIBUTES

        public string Type;

        public StateData[] States;

        #endregion
    }
}
