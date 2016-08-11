using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Space.Segment
{
    [Serializable]
    public struct SegmentObject
    {
        public NetworkInstanceId netID;
        public Vector2 _position;
        public Vector2 _size;
        public int _count;
        public float _visibleDistance;

        public string _name;
        public string _tag;
        public string _texturePath;
        public string _prefabPath;
        public string _type;

        // material yield when destroyed
        //public string[] _symbols;
    }

    public class SyncListSO : SyncListStruct<SegmentObject>
    { }

    class SegmentAttributes: NetworkBehaviour
    {
        public Vector2 MapSize = new Vector2(5000, 5000);

        public Rect MapBounds
        {
            get
            {
                return new Rect(new Vector2(0, 0), MapSize);
            }
        }

        // nested within controller
        public SyncListSO SegObjs = 
            new SyncListSO();

        [SyncVar]
        public int playerCount;
    }
}
