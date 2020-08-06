using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraDepth : MonoBehaviour
{
    Camera cam;
    // Start is called before the first frame update
    void Awake()
    {
        cam.depthTextureMode = DepthTextureMode.Depth;
    }
}
