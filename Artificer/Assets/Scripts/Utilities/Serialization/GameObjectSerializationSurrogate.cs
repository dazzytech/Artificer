using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Linq;
using System;

public class GameObjectSerializationSurrogate : ISerializationSurrogate
{
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {

        }
        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            return null;
        }
}

