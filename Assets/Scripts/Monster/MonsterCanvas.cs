using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCanvas : MonoBehaviour
{
    public float rate = 1;
    public Transform tf;
    void Update()
    {
        transform.localScale = tf.localScale * rate;
    }
}
