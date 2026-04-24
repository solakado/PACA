using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveProjectile : MonoBehaviour
{
    [Header("波动球参数")]
    public float speed = 5f;
    public float lifetime = 3f; // 3秒后自动销毁

    // 波动球飞行方向
    private Vector2 moveDirection;
    public SpriteRenderer sr;

    // 初始化波动球飞行方向
    public void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    public void Setup(Vector2 direction)
    {
        moveDirection = direction;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // 持续移动波动球
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 碰到玩家 或 非Boss物体时销毁波动球
        if (other.CompareTag("Boss"))
        {
            Destroy(gameObject);
        }
    }
}
