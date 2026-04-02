using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("基础设置")]
    private Vector3 startPos; // 角色初始位置
    private Animator anim;
    private bool isDead = false;

    [Header("复活延迟（秒）")]
    public float respawnDelay = 1.5f; // 死亡动画播完再复活

    void Start()
    {
        // 记录角色一开始的位置（复活点）
        startPos = transform.position;
        anim = GetComponent<Animator>();
    }

    // 碰撞检测
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("触发了物体：" + other.gameObject.name + " 标签：" + other.tag);

        if (other.CompareTag("trap") && !isDead)
        {
            Debug.Log(" 成功碰到陷阱！");
            Die();
        }
    }

    // 死亡逻辑
    void Die()
    {
        isDead = true;
        anim.SetBool("isDead", true); // 播放死亡动画

        // 延迟执行复活
        Invoke(nameof(Respawn), respawnDelay);
    }

    // 复活逻辑
    void Respawn()
    {
        isDead = false;
        transform.position = startPos; // 回到初始点
        anim.SetBool("isDead", false); // 回到正常动画
    }
}
