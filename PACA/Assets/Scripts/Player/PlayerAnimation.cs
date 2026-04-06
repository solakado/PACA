using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;
    private PlayerController playerController;
    private PlayerAttack playerAttack;
    private PlayerRespawn playerRespawn; // 新增：获取死亡状态

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        playerController = GetComponent<PlayerController>();
        playerAttack = GetComponent<PlayerAttack>();
        playerRespawn = GetComponent<PlayerRespawn>(); // 新增：初始化
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
      
    }
}