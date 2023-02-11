using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Transform cam;

    // Update is called once per frame
    private void Update()
    {
        transform.position = cam.position;
    }
}
