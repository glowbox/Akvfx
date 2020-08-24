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

    Vector3 lastMousePosition; 
    float mousePanSpeed = 50.0f;
    void Update()
    {
        cam.orthographicSize += -Input.mouseScrollDelta.y * Time.deltaTime * zoomSpeed;
        if(Input.GetMouseButton(2)){
            Vector3 delta = -(Input.mousePosition - lastMousePosition);
            delta.x /= Screen.width;
            delta.y /= Screen.height;
            cam.transform.position += transform.up * delta.y * Time.deltaTime * mousePanSpeed;
            cam.transform.position += transform.right * delta.x * Time.deltaTime * mousePanSpeed;
        } else {
            cam.transform.position += transform.up * Input.GetAxis("Vertical") * Time.deltaTime;
            cam.transform.position += transform.right * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        lastMousePosition = Input.mousePosition;
    }

    public void Reset(){
        if(cam){
            cam.orthographicSize = initialOrthographicSize;
            cam.transform.position = initialPosition;
        }
    }
}
