using UnityEngine;
using System.Runtime.Serialization;

public class RectSerializationSurrogate : ISerializationSurrogate {
    
    // Method called to serialize a Vector3 object
    public void GetObjectData(System.Object obj,
                              SerializationInfo info, StreamingContext context) {
        
        Rect r = (Rect) obj;
        info.AddValue("x", r.x);
        info.AddValue("y", r.y);
        info.AddValue("width", r.width);
        info.AddValue("height", r.height);
    }
    
    // Method called to deserialize a Rect object
    public System.Object SetObjectData(System.Object obj,
                                       SerializationInfo info, StreamingContext context,
                                       ISurrogateSelector selector) {
        
        Rect r = (Rect) obj;
        r.x = (float)info.GetValue("x", typeof(float));
        r.y = (float)info.GetValue("y", typeof(float));
        r.width = (float)info.GetValue("width", typeof(float));
        r.height = (float)info.GetValue("height", typeof(float));
        obj = r;
        return obj;   // Formatters ignore this return value //Seems to have been fixed!
    }
}
