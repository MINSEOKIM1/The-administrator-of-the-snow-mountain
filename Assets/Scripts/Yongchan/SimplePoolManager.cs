using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePoolManager : MonoBehaviour
{
    public GameObject[] prefabs;
    public Transform[] roots;
    public int[] counts;
    private List<GameObject>[] pools;

    void Start()
    {
        pools = new List<GameObject>[prefabs.Length];
        MakePool();
    }

    void MakePool()
    {
        for (int index = 0; index < pools.Length; index++)
        {
            pools[index] = new List<GameObject>();
            for (int idx = 0; idx < counts[index]; idx++)
            {
                GameObject select = Instantiate(prefabs[index], roots[index]);
                select.SetActive(false);
                select.transform.position = new Vector3(Random.Range(-2f, 2f), 0, 0);
                pools[index].Add(select);
            }
        }
    }

    public GameObject Get(int index)
    {
        GameObject select = null;

        foreach (GameObject item in pools[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                break;
            }
        }

        return select;
    }
}