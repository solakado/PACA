using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("基础设置")]
    private Vector3 startPos;
    private Animator anim;
    public bool isDead = false;

    [Header("复活延迟（秒）")]
    public float respawnDelay = 1.5f;

    private PlayerController playerController;
    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;


    void Start()
    {
        startPos = transform.position;
        anim = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("trap") && !isDead)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // 【关键1：告诉 PhysicsCheck 我死了，停止检测】
        if (physicsCheck != null)
        {
            physicsCheck.isDead = true;
            physicsCheck.isGround = true; // 强制设为地面
        }

        // 禁用控制
        if (playerController != null)
            playerController.enabled = false;

        // 锁死物理
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        anim.SetBool("isDead", true);

        Invoke(nameof(Respawn), respawnDelay);
    }

    void Respawn()
    {
        isDead = false;
        transform.position = startPos;

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.velocity = Vector2.zero;
        }

        if (playerController != null)
            playerController.enabled = true;

        // 【关键2：复活时告诉 PhysicsCheck 我活了，恢复检测】
        if (physicsCheck != null)
        {
            physicsCheck.isDead = false;
        }

        anim.SetBool("isDead", false);
    }
}