using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCanvas : MonoBehaviour
{


    public Transform tf;
    void Update()
    {
        transform.localScale = tf.localScale * 0.0168f;
    }
}
