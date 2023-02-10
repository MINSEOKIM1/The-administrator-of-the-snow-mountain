using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraOffset : MonoBehaviour
{
    public CinemachineFramingTransposer cam;
    public Vector3[] offset;

    private void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    public void Update()
    {
        if (GameManager.Instance.UIManager.ConservationUI.gameObject.activeSelf)
        {
            cam.m_TrackedObjectOffset = offset[0];
        }
        else
        {
            cam.m_TrackedObjectOffset = offset[1];
        }
    }
}
