using UnityEngine;
using System;
using System.Collections;

public class ZhuQueController : MonoBehaviour
{

    [Header("血量")]
    public int maxHealth = 100;
    public int currentHealth;
    [Header("无敌帧时间")]
    public bool isInvincible = false;
    public float invincibleTime = 0.5f;

    [Header("引用")]
    private Animator anim;
    private bool isDead = false;
    //private XuanWuAttack water;
    private FlashController flash;
    private AudioSource audioSource;
    public AudioClip hurtClip;

    //[Header("游戏结束")]
    //public GameObject endGameObj; // 结束标志物预制体
    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
       
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        flash = GetComponent<FlashController>();
        audioSource = GetComponent<AudioSource>();
    }

    // ================= 受伤 =================
    public void TakeDamage(int dmg)
    {
        if (isInvincible || isDead) return;

        //Debug.Log("ZhuQue受到伤害: " + dmg);

        currentHealth -= dmg;
        if (hurtClip != null)
            audioSource.PlayOneShot(hurtClip);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // 无敌帧开始
            StartCoroutine(InvincibleCoroutine());

        }
    }

    // ================= 动画事件调用 =================
    //  在受伤动画最后一帧调用
    //public void EndInvincible()
    //{
    //    Debug.Log("无敌帧结束");
    //    isInvincible = false;
    //}

    
    void Die()
    {
        if (isDead) return;

        isDead = true;
        MonoBehaviour[] allScripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in allScripts)
        {
            // 只保留我们当前这个 BossController 脚本，其他全关
            if (script != this)
            {
                script.enabled = false;
            }
        }

        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        anim.SetBool("isDead", true);
        GetComponent<Collider2D>().enabled = false;
    }

    // 这个方法会在死亡动画最后一帧调用
    //public void OnBossDeathAnimationFinish()
    //{

    //    // 生成结束标志物
    //    if (endGameObj != null)
    //    {

    //        Instantiate(endGameObj, water.centerPoint.position, Quaternion.identity);
    //    }


    //}
    private IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;

        float timer = 0f;

        while (timer < invincibleTime)
        {
            if (flash != null)
            {
                flash.Flash(0.5f, 0.1f);
            }

            yield return new WaitForSeconds(0.2f);

            timer += 0.2f;
        }

        isInvincible = false;
    }

    // 在死亡动画最后一帧调用（推荐用动画事件）
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}