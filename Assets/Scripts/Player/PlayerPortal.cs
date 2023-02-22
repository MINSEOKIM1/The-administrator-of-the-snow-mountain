using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPortal : MonoBehaviour
{
    private GameObject portal;
    public PortalManager _portalManager;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (portal != null)
            {
                _portalManager.ChangeScene(portal.gameObject.name);
                GameManager.Instance.UIManager.MapUI.UpdateMapPoint();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Portal"))
        {
            portal = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Portal"))
        {
            if (collision.gameObject == portal)
            {
                portal = null;    
            }
        }
    }
}
