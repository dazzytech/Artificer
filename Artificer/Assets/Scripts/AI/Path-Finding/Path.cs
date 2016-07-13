using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path
{
    private List<Transform> _nodes;

    // Use this for initialization
    public Path()
    {
        _nodes = new List<Transform>();
    }

    public void GetNodesOfTag(string tag)
    {
        GameObject[] nodeObjs = 
            GameObject.FindGameObjectsWithTag(tag);

        foreach(GameObject obj in nodeObjs)
            _nodes.Add(obj.transform);
    }

    /// <summary>
    /// Find the next semi-random patrol point
    /// We only want to get points of a certain distance
    /// But also allow exceptions to avoid endless loops
    /// </summary>
    public Vector3 FindNextPoint(Transform npc)
    {
        Vector3 position = Vector3.zero;
        int iterCount = 0;
        while (true)
        {
            int rndIndex = Random.Range(0, _nodes.Count);
            Vector3 rndPosition = new Vector3
                (Random.Range(0, 30),Random.Range(0, 30), 0f);
            position = (_nodes [rndIndex].position + rndPosition);

            float dist = Vector3.Distance(position, npc.transform.position);

            if(DestUtil.IsInCurrentRange(npc, position, 1, 100) || iterCount > 5)
                break;

            iterCount++;
        }

        return position;
    }

    /// <summary>
    /// Large numbers of nodes will cause slow down
    /// </summary>
    /// <returns>The closest to.</returns>
    /// <param name="enemy">Enemy.</param>
    public Vector3 FindClosestTo(Vector3 enemy)
    {
        Vector3 position = Vector3.zero;
        float distance = float.MaxValue;
        foreach(Transform node in _nodes)
        {
            float newDist = Vector3.Distance(node.position, enemy);
            if(newDist < distance)
            {
                position = node.position;
                distance = newDist;
            }
        }
        return position;
    }
}

