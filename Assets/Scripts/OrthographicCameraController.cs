using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrthographicCameraController : MonoBehaviour
{
    private Camera cam;
    private float initialOrthographicSize;
    private Vector3 initialPosition;

    public float zoomSpeed = 3.0f;

    void Start(){
        cam = GetComponent<Camera>();
        initialOrthographicSize = cam.orthographicSize;
        initialPosition = transform.position;
    }

    void Update()
    {
        cam.orthographicSize += -Input.mouseScrollDelta.y * Time.deltaTime * zoomSpeed;
        cam.transform.position += transform.up * Input.GetAxis("Vertical") * Time.deltaTime;
        cam.transform.position += transform.right * Input.GetAxis("Horizontal") * Time.deltaTime;
    }

    public void Reset(){
        if(cam){
            cam.orthographicSize = initialOrthographicSize;
            cam.transform.position = initialPosition;
        }
    }
}
