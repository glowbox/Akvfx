using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantOffsetFromParent : MonoBehaviour
{
    Vector3 startPosition;
    void Start(){
        startPosition = transform.position; 
    }
    void LateUpdate()
    {
        transform.localPosition = startPosition - transform.parent.position;
    }
}
