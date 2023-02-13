using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMoving : MonoBehaviour
{
    public Transform cam;
    public Vector3 camBeginPos, beginPos;
    public float movingSpeed;
    void Start()
    {
        camBeginPos = cam.position;
        beginPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = beginPos + (cam.position - camBeginPos) * movingSpeed;
    }
}
