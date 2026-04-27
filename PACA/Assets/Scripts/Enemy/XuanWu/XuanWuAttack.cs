using UnityEngine;

public class XuanWuAttack : MonoBehaviour
{
    [Header("玩家")]
    public Transform player;

    [Header("中心点")]
    public Transform centerPoint;

    [Header("距离设置")]
    public float chaseDistance = 6f;
    public float attackDistance = 4f;
    public float defendDistance = 1.5f; // 贴脸缩壳

    [Header("移动")]
    public float moveSpeed = 1.5f;

    [Header("冷却")]
    public float waterCooldown = 2f;

    [Header("引用")]
    private Animator anim;
    public GameObject waterballPrefab;
    public Transform waterPoint;
    private XuanWuController xuanWuController;
    private float waterTimer;

    private bool isDefending; // 核心状态

    void Start()
    {
        anim= GetComponent<Animator>();
        xuanWuController= GetComponent<XuanWuController>();
    }
    void Update()
    {
        if (player == null || centerPoint == null) return;

        float distance = Vector2.Distance(centerPoint.position, player.position);

        FlipToPlayer();

        waterTimer -= Time.deltaTime;

        // ================== 1. 缩壳（最高优先级） ==================
        if (distance <= defendDistance)
        {
            EnterDefend();
            return; // 直接锁死，不执行下面任何逻辑
        }
        else
        {
            ExitDefend();
        }

        // ================== 2. 远程攻击 ==================
        if (distance <= attackDistance)
        {
            anim.SetBool("isRun", false);

            if (waterTimer <= 0)
            {
                anim.SetTrigger("WaterTrigger");
                waterTimer = waterCooldown;
            }

            return;
        }


        // ================== 3. 追击 ==================
        if (distance <= chaseDistance)
        {
            anim.SetBool("isRun", true);
            MoveToPlayer();
        }
        if (distance >= chaseDistance)
        {
            anim.SetBool("isRun", false);
        }
        
    }

    // ================== 缩壳 ==================
    void EnterDefend()
    {
        if (isDefending) return;

        isDefending = true;
        anim.SetBool("isDefending", true);

        // 这里可以加无敌
        xuanWuController.isInvincible = true;
        //GetComponent<Collider2D>().enabled = false;
    }

    void ExitDefend()
    {
        if (!isDefending) return;

        isDefending = false;
        anim.SetBool("isDefending", false);

        xuanWuController.isInvincible = false;
        //GetComponent<Collider2D>().enabled = true;
    }

    // ================== 面向 ==================
    void FlipToPlayer()
    {
        Vector3 scale = transform.localScale;

        if (player.position.x > transform.position.x)
            scale.x = -Mathf.Abs(scale.x);
        else
            scale.x = Mathf.Abs(scale.x);

        transform.localScale = scale;
    }

    // ================== 移动 ==================
    //void MoveToPlayer()
    //{
    //    if (player == null && !isRun) return;

    //    Vector2 direction = (player.position - transform.position).normalized;

    //    transform.Translate(direction * moveSpeed * Time.deltaTime);
    //}
    void MoveToPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.Translate(dir * moveSpeed * Time.deltaTime);
    }

    // ================== 动画事件 ==================
    public void SpawnWaterball()
    {
        GameObject wb = Instantiate(waterballPrefab, waterPoint.position, Quaternion.identity);

        float dir = transform.localScale.x > 0 ? -1 : 1;

        wb.GetComponent<WaterBall>()?.Setup(new Vector2(dir, 0));
        if (dir > 0)
        {
            wb.GetComponent<WaterBall>().sr.flipX = true;
        }
    }

    // ================== Gizmos ==================
    void OnDrawGizmosSelected()
    {
        if (centerPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centerPoint.position, defendDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(centerPoint.position, attackDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(centerPoint.position, chaseDistance);
    }
}