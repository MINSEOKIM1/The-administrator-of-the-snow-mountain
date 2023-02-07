using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAction : MonoBehaviour
{
    public Effect Effect;
    public void Destroy()
    {
        if (transform.parent != null) Destroy(transform.parent.gameObject);
        Destroy(gameObject);
        
    }
}
