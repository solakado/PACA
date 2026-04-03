using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;
    private PlayerController playerController;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        SetAnimation();
    }

    public void SetAnimation()
    {
       
        anim.SetFloat("velocityX", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("velocityY", rb.velocity.y);
        anim.SetBool("isGround", physicsCheck.isGround);
        anim.SetBool("isDashing", playerController.isDashing);

        // ===================== 冲刺动画自动同步 =====================


        // 你原来的注释，可以自己打开
        //anim.SetBool("isDead", playerController.isDead);
        //anim.SetBool("isAttack", playerController.isAttack);
    }

    //public void PlayHurt()
    //{
    //    anim.SetTrigger("hurt");
    //}

    //public void PlayAttack()
    //{
    //    anim.SetTrigger("attack");
    //}
}
