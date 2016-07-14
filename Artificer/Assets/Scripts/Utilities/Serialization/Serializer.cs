using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Serializer
{
    public class Serializer
    {
        public static T Load<T>(string filename) where T : class
        {
            if (File.Exists(filename))
            {
                try
                {
                    using (Stream stream = File.OpenRead(filename))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        SurrogateSelector surrogate = new SurrogateSelector();

                        // Add vector2 as a surrogate
                        Vector2SerializationSurrogate vector2Surrogate = new Vector2SerializationSurrogate();
                        surrogate.AddSurrogate(typeof(UnityEngine.Vector2),
                                               new StreamingContext(StreamingContextStates.All),
                                               vector2Surrogate);
                        // RECT NOT WORKING
                        // Add Rect as a surrogate
                        RectSerializationSurrogate rectSurrogate = new RectSerializationSurrogate();
                        surrogate.AddSurrogate(typeof(UnityEngine.Rect),
                                               new StreamingContext(StreamingContextStates.All),
                                               rectSurrogate);

                        //Have the formatter use our surrogate selector
                        formatter.SurrogateSelector = surrogate;
                        return formatter.Deserialize(stream) as T;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
            return default(T);
        }

        public static void Save<T>(string filename, T data) where T : class
        {
            using (Stream stream = File.OpenWrite(filename))
            {
                // create formatter and surrogate
                BinaryFormatter formatter = new BinaryFormatter();
                SurrogateSelector surrogate = new SurrogateSelector();

                // Add vector2 as a surrogate
                Vector2SerializationSurrogate vector2Surrogate = new Vector2SerializationSurrogate();
                surrogate.AddSurrogate(typeof(UnityEngine.Vector2),
                                new StreamingContext(StreamingContextStates.All),
                                       vector2Surrogate);

                //Have the formatter use our surrogate selector
                formatter.SurrogateSelector = surrogate;

                formatter.Serialize(stream, data);
            }
        }
    }
}

