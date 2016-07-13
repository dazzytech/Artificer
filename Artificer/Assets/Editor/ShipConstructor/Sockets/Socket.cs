using UnityEngine;
using System.Collections;
using System.Xml.Serialization;

public class Socket
{
    [XmlAttribute("linkID")]
    public string socketID;
    [XmlAttribute("IDfrom")]
    public string objectID;

    [XmlAttribute("IDto")]
    public string objToID;
    [XmlAttribute("linkToID")]
    public string sockToID;
}

