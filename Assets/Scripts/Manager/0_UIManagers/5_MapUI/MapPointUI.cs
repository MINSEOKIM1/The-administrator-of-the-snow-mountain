using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapPointUI : MonoBehaviour
{
    public string mapName;

    private Image _image;
    private Color _c;

    private void Start()
    {
        _image = GetComponent<Image>();
        mapName = gameObject.name;
    }

    private void Update()
    {
        if (GameManager.Instance.UIManager.MapUI.selectedMapName.Equals(mapName))
        {
            _image.color = Color.gray;
        }
        else
        {
            var k = ((float) GameManager.Instance.MapManager.GetMapWithString(mapName).curMob) /
                    GameManager.Instance.MapManager.GetMapWithString(mapName).maxMob;
            _c.g = 1-k;
            _c.b = 1-k;
            _c.r = 1;
            _c.a = 1;
            _image.color = _c;
        }
    }
}
