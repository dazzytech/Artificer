using UnityEngine;
using System;
using System.Collections;


/// <summary>
/// Randomized node generator.
/// populates the universe with randomly located nodes and 
/// todo: add conditionals for different nodes
/// </summary>
/// 
namespace Space.Segment.Generator
{
    public class NodeGenerator : MonoBehaviour
    {
        public void GenerateNodes(int numOfNodes, string tag, float x, float y, float width, float height)
        {
            // for now only create lots of raider nodes
            int index = 0;
            while (index < numOfNodes)
            {
                GameObject node = new GameObject();
                node.name = String.Format("{0}_{1}", tag, index);
                node.tag = tag;
                node.transform.parent = this.transform;

                Vector3 newPos = new Vector3
                    (UnityEngine.Random.Range(x, x + width), 
                     UnityEngine.Random.Range(y, y + height));

                node.transform.position = newPos;
                index++;
            }
        }
    }
}
