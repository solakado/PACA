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

    // 对外提供死亡接口（由 HurtBox 调用）
    public void Die()
    {
        if (isDead||playerController.isDashing) return;

        isDead = true;
        Debug.Log("玩家死亡");

        // 停止地面检测
        if (physicsCheck != null)
        {
            physicsCheck.isDead = true;
            physicsCheck.isGround = true;
        }

        // 禁用控制
        if (playerController != null)
            playerController.enabled = false;

        // 停止攻击
        PlayerAttack atk = GetComponent<PlayerAttack>();
        if (atk != null)
            atk.ResetAttackState();

        // 锁物理（防止飞走/下落）
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        // 播放死亡动画
        if (anim != null)
            anim.SetBool("isDead", true);

        Invoke(nameof(Respawn), respawnDelay);
    }

    void Respawn()
    {
        isDead = false;

        // 回到出生点
        transform.position = startPos;

        // 恢复物理
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.velocity = Vector2.zero;
        }

        // 恢复控制
        if (playerController != null)
            playerController.enabled = true;

        // 恢复地面检测
        if (physicsCheck != null)
        {
            physicsCheck.isDead = false;
        }

        // 恢复动画
        if (anim != null)
            anim.SetBool("isDead", false);
    }
}