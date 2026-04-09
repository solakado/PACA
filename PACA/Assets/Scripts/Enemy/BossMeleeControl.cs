//using UnityEngine;

//public class BossMeleeControl : MonoBehaviour
//{
//    [Header("玩家")]
//    public Transform player;

//    [Header("中心点（用于距离计算）")]
//    public Transform centerPoint;

//    [Header("距离设置")]
//    public float chaseDistance = 5f;     // 开始追击
//    public float attackDistance = 2f;    // 下砸范围

//    [Header("移动")]
//    public float moveSpeed = 2f;

//    [Header("攻击冷却")]
//    public float attackCooldown = 2f;

//    [Header("引用")]
//    public Animator anim;

//    private float attackTimer;
//    private bool isAttacking;

//    void Update()
//    {
//        if (player == null || centerPoint == null) return;

//        // 使用中心点计算距离（避免脚底问题）
//        float distance = Vector2.Distance(centerPoint.position, player.position);

//        // 面向玩家
//        //FlipToPlayer();

//        // 冷却计时
//        attackTimer -= Time.deltaTime;

//        // ================= 状态逻辑 =================

//        //  1. 进入攻击范围 → 下砸
//        if (distance <= attackDistance)
//        {
//            anim.SetBool("isRun", false);

//            if (attackTimer <= 0 && !isAttacking)
//            {
//                anim.SetTrigger("SmashTrigger");
//                attackTimer = attackCooldown;
//                isAttacking = true;
//            }

//            return;
//        }

//        // 2. 中距离 → 追击玩家
//        if (distance <= chaseDistance)
//        {
//            anim.SetBool("isRun", true);
//            isAttacking = false;

//            MoveToPlayer();
//        }
//        else
//        {
//            // 3. 超出范围 → 停止（交给远程系统）
//            anim.SetBool("isRun", false);
//            isAttacking = false;
//        }
//    }

//    // ================= 移动 =================
//    void MoveToPlayer()
//    {
//        if (player == null) return;

//        Vector2 direction = (player.position - transform.position).normalized;

//        transform.Translate(direction * moveSpeed * Time.deltaTime);
//    }

//    // ================= 面向玩家 =================
//    //void FlipToPlayer()
//    //{
//    //    if (player == null) return;

//    //    // 防止左右抖动（非常关键）
//    //    if (Mathf.Abs(player.position.x - transform.position.x) < 0.05f)
//    //        return;

//    //    Vector3 scale = transform.localScale;

//    //    // 你默认朝左 → 反逻辑
//    //    if (player.position.x > transform.position.x)
//    //        scale.x = -Mathf.Abs(scale.x);
//    //    else
//    //        scale.x = Mathf.Abs(scale.x);

//    //    transform.localScale = scale;
//    //}

//    // ================= 动画事件（攻击结束） =================
//    public void EndAttack()
//    {
//        isAttacking = false;
//    }

//    // ================= 可视化 =================
//    void OnDrawGizmosSelected()
//    {
//        if (centerPoint == null) return;

//        // 攻击范围
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(centerPoint.position, attackDistance);

//        // 追击范围
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawWireSphere(centerPoint.position, chaseDistance);

//        // 指向玩家
//        if (player != null)
//        {
//            Gizmos.color = Color.blue;
//            Gizmos.DrawLine(centerPoint.position, player.position);
//        }
//    }
//}