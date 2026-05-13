using UnityEngine;

public class BossFireControl : MonoBehaviour
{
    [Header("玩家")]
    public Transform player;

    [Header("中心点（用于距离计算）")]
    public Transform centerPoint;   // 新增（解决“从脚底算距离”问题）

    [Header("距离设置")]
    public float chaseDistance = 5f;     // 开始追击
    public float attackDistance = 2f;    // 下砸范围
    public float fireDistance = 6f;
    public float detectDistance = 10f;   // 最远攻击距离


    [Header("移动")]
    public float moveSpeed = 2f;

    [Header("冲刺")]
    public float dashSpeed = 10f;
    public float dashTime = 0.8f;

    private bool isDashing = false;
    private float dashTimer;
    private Vector2 dashDir;

    [Header("冷却时间")]
    public float fireCooldown = 2f;
    public float attackCooldown = 2f;

    [Header("引用")]
    public Animator anim;
    public GameObject fireballPrefab;
    public GameObject smashPrefab;

    public Transform firePoint;
    public Transform smashPoint;

    private float attackTimer;
    private bool isAttacking;
    private float fireTimer;

    private bool isRun;

    void Update()
    {
        if (player == null || centerPoint == null) return;
        //if (isAttacking)
        //{
        //    anim.SetBool("isRun", false);
        //    return;
        //}
        if (isAttacking)
        {
            anim.SetBool("isRun", false);

            if (isDashing)
            {
                transform.Translate(dashDir * dashSpeed * Time.deltaTime);

                dashTimer -= Time.deltaTime;

                if (dashTimer <= 0)
                {
                    EndDash();
                }
            }

            return;
        }

        // 用中心点计算距离（核心修复）
        float distance = Vector2.Distance(centerPoint.position, player.position);

        // 面向玩家
        FlipToPlayer();

        // 冷却计时
        fireTimer -= Time.deltaTime;
        attackTimer -= Time.deltaTime;

        // 超出距离 → 远程攻击
        //if (distance > fireDistance)
        //{
        //    anim.SetBool("isRun", false);
        //    isRun = false;
        //    if (fireTimer <= 0)
        //    {
        //        isAttacking = true;
        //        anim.SetTrigger("FireTrigger");
        //        fireTimer = fireCooldown;

        //    }
        //}
        if (distance > fireDistance && distance <= detectDistance)
        {
            anim.SetBool("isRun", false);

            if (fireTimer <= 0)
            {
                isAttacking = true;

                int rand = Random.Range(0, 100);

                if (rand < 50)
                {
                    anim.SetTrigger("FireTrigger");
                }
                else
                {
                    anim.SetTrigger("DashReadyTrigger");
                }

                fireTimer = fireCooldown;
            }

            return;
        }

        if (distance > detectDistance)
        {
            anim.SetBool("isRun", false);
            return;
        }

        if (distance <= attackDistance)
        {
            anim.SetBool("isRun", false);
            isRun = false;

            if (attackTimer <= 0 )
            {
                isAttacking = true;
                anim.SetTrigger("SmashTrigger");
                attackTimer = attackCooldown;
         
            }

            return;
        }

        // 2. 中距离 → 追击玩家
        if (distance <= chaseDistance)
        {
            anim.SetBool("isRun", true);
            isRun = true;

            MoveToPlayer();
        }
        else
        {
            // 3. 超出范围 → 停止（交给远程系统）
            anim.SetBool("isRun", false);
            isRun = false;
        }
    }

    // ================= 面向玩家 =================
    void FlipToPlayer()
    {
        if (player == null) return;

        Vector3 scale = transform.localScale;

        // 你是“默认朝左”，所以反转逻辑
        if (player.position.x > transform.position.x)
            scale.x = -Mathf.Abs(scale.x);   // 面向右
        else
            scale.x = Mathf.Abs(scale.x);    // 面向左

        transform.localScale = scale;
    }

    void MoveToPlayer()
    {
        if (player == null&&!isRun) return;

        Vector2 direction = (player.position - transform.position).normalized;

        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }


    // ================= 动画事件调用 =================
    public void SpawnSmashHitBox()
    {
        GameObject sp = Instantiate(smashPrefab, smashPoint.position, Quaternion.identity);
        Destroy(sp, 0.2f);

    }

    public void SpawnFireball()
    {
        if (fireballPrefab == null || firePoint == null)
        {
            Debug.LogWarning("FireballPrefab 或 FirePoint 没设置！");
            return;
        }

        Debug.Log("发射火球");

        GameObject fb = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

        // 注意：你默认朝左，这里方向要反！
        float dir = transform.localScale.x > 0 ? -1 : 1;

        fb.GetComponent<Fireball>()?.Setup(new Vector2(dir, 0));
        if(dir>0)
        {
            fb.GetComponent<Fireball>().sr.flipX = true;    
        }
    }

    public void StartDash()
    {
        isDashing = true;

        dashDir = (player.position - transform.position).normalized;
        dashDir.y = 0;
        dashDir.Normalize();

        dashTimer = dashTime;

        anim.SetBool("isDash", true);
    }
    void EndDash()
    {
        isDashing = false;
        isAttacking = false;

        anim.SetBool("isDash", false);
    }
    //public void EndAttack()
    //{
    //    isAttacking = false;
    //}
    public void EndAttack()
    {
        if (!isDashing)
            isAttacking = false;
    }

    // ================= 可视化 =================
    //void OnDrawGizmosSelected()
    //{
    //    if (centerPoint == null) return;

    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(centerPoint.position, attackDistance);
    //    // 攻击范围（从身体中心）
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(centerPoint.position, fireDistance);
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(centerPoint.position, chaseDistance);

    //    // 指向玩家
    //    if (player != null)
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawLine(centerPoint.position, player.position);
    //    }
    //}
    void OnDrawGizmosSelected()
    {
        if (centerPoint == null) return;

        // 近战范围（下砸）
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centerPoint.position, attackDistance);

        // 追击范围
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(centerPoint.position, fireDistance);

        // 随机技能范围（新增）
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(centerPoint.position, detectDistance);

        // 指向玩家
        if (player != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(centerPoint.position, player.position);
        }
    }
}