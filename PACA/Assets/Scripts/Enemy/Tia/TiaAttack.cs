using UnityEngine;

public class TiaAttack : MonoBehaviour
{
    [Header("玩家与判定点")]
    public Transform player;
    public Transform centerPoint;

    [Header("距离设置")]
    public float detectDistance = 10f;   // 发现玩家并开始奔跑的距离
    public float rangedDistance = 6f;    // 远程攻击判定距离 (原 fireDistance)
    public float meleeDistance = 2f;     // 近战攻击判定距离 (原 attackDistance)

    [Header("移动与冲刺")]
    public float moveSpeed = 4f;
    public float dashSpeed = 12f;
    public float dashTime = 0.5f;

    private bool isDashing = false;
    private float dashTimer;
    private Vector2 dashDir;

    [Header("冷却时间")]
    public float skillCooldown = 2.5f;   // 统一技能冷却
    private float skillTimer;

    [Header("引用")]
    public GameObject swordAuraPrefab;
    public Transform swordAuraPoint;
    public GameObject swordArrayPrefab;
    public Transform swordArrayPoint1;
    public Transform swordArrayPoint2;
    private Animator anim;
    private bool isAttacking;
    private bool isRun;
    void Start()
    {
        anim=GetComponent<Animator>();
    }
    void Update()
    {
        if (player == null || centerPoint == null) return;

        // 如果正在攻击或冲刺，锁定状态
        if (isAttacking)
        {
            anim.SetBool("isRun", false);
            if (isDashing)
            {
                transform.Translate(dashDir * dashSpeed * Time.deltaTime);
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0) EndDash();
            }
            return;
        }

        // 1. 计算距离与朝向
        float distance = Vector2.Distance(centerPoint.position, player.position);
        FlipToPlayer();

        // 冷却计时
        skillTimer -= Time.deltaTime;

        // 2. 超出检测距离 -> 待机
        if (distance > detectDistance)
        {
            SetRunState(false);
            return;
        }

        // 3. 近战圈判断 (<= 2f)
        if (distance <= meleeDistance)
        {
            SetRunState(false);
            if (skillTimer <= 0)
            {
                isAttacking = true;
                int rand = Random.Range(0, 100);
                if (rand < 50)
                    anim.SetTrigger("ComboSlashTrigger"); // 连斩
                else
                    anim.SetTrigger("SwordArrayTrigger"); // 剑阵

                skillTimer = skillCooldown;
            }
            return;
        }

        // 4. 远程圈判断 (<= 6f 且 > 2f)
        if (distance <= rangedDistance)
        {
            SetRunState(false);
            if (skillTimer <= 0)
            {
                isAttacking = true;
                int rand = Random.Range(0, 100);
                if (rand < 50)
                    anim.SetTrigger("SwordAuraTrigger"); // 剑气
                else
                    StartDashPreparation();              // 冲刺

                skillTimer = skillCooldown;
            }
            return;
        }

        // 5. 索敌圈判断 (<= 10f 且 > 6f) -> 跑向玩家
        SetRunState(true);
        MoveToPlayer();
    }

    void SetRunState(bool state)
    {
        if (isRun != state)
        {
            isRun = state;
            anim.SetBool("isRun", state);
        }
    }

    //void FlipToPlayer()
    //{
    //    if (player == null) return;
    //    Vector3 scale = transform.localScale;
    //    if (player.position.x > transform.position.x)
    //        scale.x = -Mathf.Abs(scale.x);
    //    else
    //        scale.x = Mathf.Abs(scale.x);
    //    transform.localScale = scale;
    //}
    void FlipToPlayer()
    {
        if (player == null) return;
        Vector3 scale = transform.localScale;

        // 恢复为标准逻辑：假设 Tia 的初始美术素材是【默认朝右】的
        if (player.position.x > transform.position.x)
        {
            // 玩家在右侧，保持正向缩放
            scale.x = Mathf.Abs(scale.x);
        }
        else
        {
            // 玩家在左侧，X轴反转
            scale.x = -Mathf.Abs(scale.x);
        }

        transform.localScale = scale;
    }

    void MoveToPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    // ================= 冲刺与动画事件 =================

    public void SpawnSwordAura()
    {
        if (swordAuraPrefab == null || swordAuraPoint == null)
        {
            Debug.LogWarning("SwordAuraPrefab 或SwordAuraPoint 没设置！");
            return;
        }

        //Debug.Log("发射剑气");

        GameObject fb = Instantiate(swordAuraPrefab, swordAuraPoint.position, Quaternion.identity);

        // 注意：你默认朝左，这里方向要反！
        float dir = transform.localScale.x > 0 ? 1 : -1;

        fb.GetComponent<SwordAura>()?.Setup(new Vector2(dir, 0));
        if (dir > 0)
        {
            fb.GetComponent<SwordAura>().sr.flipX = true;
        }
    }
    public void SpawnSwordArray()
    {
        if (swordAuraPrefab == null || swordAuraPoint == null)
        {
            Debug.LogWarning("swordArrayPrefab 或swordArrayPoint 没设置！");
            return;
        }

  

        GameObject fb1 = Instantiate(swordArrayPrefab, swordArrayPoint1.position, Quaternion.identity);
        GameObject fb2 = Instantiate(swordArrayPrefab, swordArrayPoint2.position, Quaternion.identity);

        //// 注意：你默认朝左，这里方向要反！
        //float dir = transform.localScale.x > 0 ? 1 : -1;

        //fb.GetComponent<SwordAura>()?.Setup(new Vector2(dir, 0));
        //if (dir > 0)
        //{
        //    fb.GetComponent<SwordAura>().sr.flipX = true;
        //}
    }
    

    public void StartDashPreparation()
    {
        anim.SetTrigger("DashReadyTrigger");
        dashDir = (player.position - transform.position).normalized;
        dashDir.y = 0;
        dashDir.Normalize();
    }

    // 由 DashReady 动画帧事件调用
    public void StartDash()
    {
        isDashing = true;
        
        dashTimer = dashTime;
        anim.SetBool("isDash", true);
    }

    public void EndDash()
    {
        isDashing = false;
        isAttacking = false;
        anim.SetBool("isDash", false);
    }

    // 动画结束帧调用
    public void EndAttack()
    {
        if (!isDashing) isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        if (centerPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centerPoint.position, meleeDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(centerPoint.position, rangedDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(centerPoint.position, detectDistance);
        if (player != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(centerPoint.position, player.position);
        }
    }
}
