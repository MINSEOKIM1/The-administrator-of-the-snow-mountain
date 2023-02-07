using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public GameObject[] effects;

    public GameObject CreateEffect(int index, Vector3 position, Quaternion rotation)
    {
        var effect = Instantiate(effects[index], position, rotation);
        return effect;
    }
}
