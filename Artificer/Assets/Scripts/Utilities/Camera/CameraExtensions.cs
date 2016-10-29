using UnityEngine;
using System.Collections;

public static class CameraExtensions
{
    public static Bounds OrthographicBounds(Camera camera)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        Bounds bounds = new Bounds(
            camera.transform.position + new Vector3(0f,0f,10f),
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }

    public static Bounds MaxOrthographicBounds(Camera camera)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = 31 * 2;
        Bounds bounds = new Bounds(
            camera.transform.position + new Vector3(0f, 0f, 10f),
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }
}

