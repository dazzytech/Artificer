using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Data.Space
{
    [System.Serializable]
    public struct SegmentObjectData
    {
        public NetworkInstanceId netID;
        public Vector2 _position;
        public Vector2 _size;
        public Vector2[] _border;
        public int _count;
        public float _visibleDistance;

        public string _name;
        public string _tag;
        public string _texturePath;
        public string _prefabPath;
        public string _type;
    }
}