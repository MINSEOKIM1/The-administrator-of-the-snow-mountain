using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EffectManager : MonoBehaviour
{
    public GameObject[] effects;
    public GameObject dropItem;

    public GameObject CreateEffect(int index, Vector3 position, Quaternion rotation)
    {
        var effect = Instantiate(effects[index], position, rotation);
        return effect;
    }
    
    public GameObject CreateEffect(int index, int localScale, Vector3 position, Quaternion rotation)
    {
        var effect = Instantiate(effects[index], position, rotation);
        effect.transform.localScale = new Vector3(localScale, 1, 1);
        return effect;
    }

    public GameObject CreateItem(ItemInfo itemInfo, int count, Vector3 position, Quaternion rotation)
    {
        var item = Instantiate(dropItem, position, rotation);
        var itemDropped = item.GetComponent<ItemDropped>();
        item.GetComponent<Rigidbody2D>().AddForce(Random.Range(-3, 3) * Vector2.left + Vector2.up * 3, ForceMode2D.Impulse);
        itemDropped.SetItem(itemInfo, count);
        return item;
    }
}
