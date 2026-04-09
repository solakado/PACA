using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("血量")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("引用")]
    public Animator anim;

    private bool isInvincible = false;
    private bool isDead = false;

    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    // ================= 受伤 =================
    public void TakeDamage(int dmg)
    {
        if (isInvincible || isDead) return;

        Debug.Log("Boss受到伤害: " + dmg);

        currentHealth -= dmg;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // 无敌帧开始
            isInvincible = true;

            // 播放受伤动画
            anim.SetTrigger("HurtTrigger");
        }
    }

    // ================= 动画事件调用 =================
    //  在受伤动画最后一帧调用
    public void EndInvincible()
    {
        Debug.Log("无敌帧结束");
        isInvincible = false;
    }

    // ================= 死亡 =================
    void Die()
    {
        if (isDead) return;

        isDead = true;
        rb.velocity = Vector2.zero;

        // 关闭物理
        rb.bodyType = RigidbodyType2D.Kinematic;


        anim.SetBool("isDead", true);

        // 关闭碰撞（防止继续被打）
        GetComponent<Collider2D>().enabled = false;
        
    }

    // 在死亡动画最后一帧调用（推荐用动画事件）
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}