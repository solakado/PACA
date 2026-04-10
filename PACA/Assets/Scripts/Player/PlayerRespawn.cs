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
    private SpriteRenderer sr; // 新增：渲染器引用

    void Start()
    {
        startPos = transform.position;
        anim = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        sr = GetComponent<SpriteRenderer>(); // 新增：获取渲染器
    }

    public void Die()
    {
        if (isDead || playerController.isDashing) return;

        isDead = true;
        Debug.Log("玩家死亡");

        if (physicsCheck != null)
        {
            physicsCheck.isDead = true;
            physicsCheck.isGround = true;
        }

        if (playerController != null)
            playerController.enabled = false;

        PlayerAttack atk = GetComponent<PlayerAttack>();
        if (atk != null)
            atk.ResetAttackState();

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        if (anim != null)
            anim.SetBool("isDead", true);

        Invoke(nameof(Respawn), respawnDelay);
    }

    void Respawn()
    {
        isDead = false;

        // 1. 强制复位位置（同时设transform和rigidbody，防止卡物理）
        transform.position = startPos;
        if (rb != null)
        {
            rb.position = startPos; // 新增：强制复位物理位置
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.velocity = Vector2.zero;
        }

        // 2. 强制显示角色（防止渲染器被关）
        if (sr != null)
        {
            sr.enabled = true;
            sr.color = Color.white; // 新增：强制恢复颜色，防止透明
        }

        // 3. 强制重置动画（防止动画状态机卡住）
        if (anim != null)
        {
            anim.Rebind(); // 新增：强制重置动画状态机
            anim.SetBool("isDead", false);
        }

        if (playerController != null)
            playerController.enabled = true;

        if (physicsCheck != null)
        {
            physicsCheck.isDead = false;
            physicsCheck.isGround = false; // 新增：重置地面检测
        }
    }
}