using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Space.Segment
{
    public struct SegmentObject
    {
        //public int _id;
        //public Vector2 _position;
        //public Vector2 _size;
        //public int _count;

        //public string _name;
        //public string _texturePath;
        //public string _type;

        // material yield when destroyed
        //public string[] _symbols;
    }

    public class SyncListSO : SyncListStruct<SegmentObject>
    {
        /// <summary>
    	/// Gets the type of the object 
    	/// called in parameters.
    	/// </summary>
    	/// <returns>The objects of type.</returns>
    	/// <param name="type">Type.</param>
    	/*public List<SegmentObject> GetObjsOfType(string type)
        {
            List<SegmentObject> objs = new List<SegmentObject>();
            foreach (SegmentObject obj in this)
                if (obj._type == type)
                    objs.Add(obj);
            return objs;
        }*/
    }

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

        //public SyncListSO SegObjs = new SyncListSO();

        [SyncVar]
        public int playerCount;
    }
}
