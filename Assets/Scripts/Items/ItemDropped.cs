using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemDropped : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;
    private ItemInfo _item;
    private int _count;
    private float _timeElapsed;

    private void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        _timeElapsed -= Time.deltaTime;
        if (_timeElapsed < -1)
        {
            Destroy(gameObject);
        }
    }

    public void SetItem(ItemInfo item, int num)
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        
        _rigidbody.AddForce(new Vector2(Random.Range(-4, 4), 5));
        _item = item;
        _count = (int) Mathf.Clamp(((float)num) * Random.Range(0, 1), 1, num);
        _timeElapsed = 120;
        _spriteRenderer.sprite = _item.itemIcon;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player")) 
        {
            if (GameManager.Instance.PlayerDataManager.inventory.AddItem(_item, _count))
            {
                // 무슨 아이템, 몇 개 먹었는지 메시지 보내기
                Destroy(gameObject);
            }
            else
            {
                // 가방이 가득 찼습니다 메시지 보내기
                throw new NotImplementedException();
            }
        }
    }
}
