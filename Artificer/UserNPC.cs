// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.1433
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace Space.AI
{
    using System;
    using UnityEngine;
    using System.Collections.Generic;
    using Data.UI;
    
    
    public class CustomState : ICustomState
    {
        
        // The main execution loop of the NPC script
        public override void PerformLoop()
        {
            System.Collections.Generic.List<float> array_3 = new List<float>();
            int index_1 = 0;
            float item_1;
            for (index_1 = 0; (index_1 < array_3.Count()); index_1 = (index_1 + 1))
            {
                item_1 = array_3[index_1];
Debug.Log(item_1.ToString());
            }
        }
    }
}
