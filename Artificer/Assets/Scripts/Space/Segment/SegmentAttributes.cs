using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Space.CameraUtils;
using Data.Space;

namespace Space.Segment
{

    public class SyncListSO : SyncListStruct<SegmentObjectData>
    { }

    public class SyncListPI : SyncListStruct<SyncPI>
    { }

    class SegmentAttributes: NetworkBehaviour
    {
        // nested within controller
        public SyncListSO SegObjs = 
            new SyncListSO();

        public SyncListPI BGItem =
            new SyncListPI();

        [SyncVar]
        public int playerCount;
    }
}
