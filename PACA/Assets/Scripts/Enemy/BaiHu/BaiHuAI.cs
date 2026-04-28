using UnityEngine;

public class BaiHuAI : MonoBehaviour
{
    public Transform player;
    public Transform centerPoint; // 中心点（子物体）

    [Header("巡逻点")]
    public Transform pointA;
    public Transform pointB;

    [Header("检测距离")]
    public float detectDistance = 6f;
    public float attackDistance = 12f;

    [Header("移动速度")]
    public float patrolSpeed = 2f;
    public float followSpeed = 3.5f;
    public float dashSpeed = 10f;
    public float returnSpeed = 4f;

    [Header("冲刺参数")]
    public float dashDuration = 0.5f;
    public float dashCooldown = 1f;

    [Header("丢失目标")]
    public float loseTime = 2f;

    private Animator anim;
    private Transform targetPoint;
    private float dashTimer;
    private float cooldownTimer;
    private float loseTimer;
    private Vector2 dashDir;

    enum State
    {
        Patrol, Follow, PrepareDash, Dash, Return
    }

    private State currentState;

    void Start()
    {
        anim = GetComponent<Animator>();
        targetPoint = pointA;
        currentState = State.Patrol;
    }

    void Update()
    {
        if (player == null || centerPoint == null) return;

        // 全部用 centerPoint 判断距离
        float distance = Vector2.Distance(centerPoint.position, player.position);

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                if (distance <= detectDistance) StartPrepareDash();
                else if (distance <= attackDistance) currentState = State.Follow;
                break;

            case State.Follow:
                FollowPlayer();
                if (distance <= detectDistance && cooldownTimer <= 0) StartPrepareDash();
                if (distance > attackDistance)
                {
                    loseTimer += Time.deltaTime;
                    if (loseTimer >= loseTime)
                    {
                        targetPoint = GetNearestPoint();
                        currentState = State.Return;
                        loseTimer = 0;
                    }
                }
                else loseTimer = 0;
                break;

            case State.Dash:
                DashAttack();
                break;

            case State.Return:
                ReturnPatrol();
                if (distance <= detectDistance) StartDash();
                else if (distance <= attackDistance) currentState = State.Follow;
                break;
        }

        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;
    }

    // =============================
    // 巡逻（用中心点判断）
    // =============================
    void Patrol()
    {
        anim.SetBool("isRun", true);
        anim.SetBool("isDash", false);

        //用 centerPoint 判断距离
        float dis = Vector2.Distance(centerPoint.position, targetPoint.position);

        if (dis <= 0.5f)
        {
            //transform.position = targetPoint.position;
            targetPoint = targetPoint == pointA ? pointB : pointA;
            float dir = targetPoint.position.x - centerPoint.position.x;
            Flip(dir);
            return;
        }

        MoveTo(targetPoint.position, patrolSpeed);
    }

    // =============================
    // 跟随玩家
    // =============================
    void FollowPlayer()
    {
        anim.SetBool("isRun", true);
        anim.SetBool("isDash", false);
        dashDir = (player.position - centerPoint.position).normalized;
        Flip(dashDir.x);
        MoveTo(player.position, followSpeed);
    }

    // =============================
    // 冲刺
    // =============================
    //void StartDash()
    //{
    //    dashDir = (player.position - centerPoint.position).normalized;
    //    dashTimer = dashDuration;
    //    Flip(dashDir.x);
    //    currentState = State.Dash;
    //}
    void StartDash()
    {
        dashDir =
            (player.position - transform.position).normalized;

        dashTimer = dashDuration;

        Flip(dashDir.x);

        currentState = State.Dash;

        anim.SetBool("isDash", true);
    }
    void StartPrepareDash()
    {
        currentState = State.PrepareDash;

        anim.SetTrigger("prepareDash");
    }
    public void BeginDash()
    {
        if (currentState != State.PrepareDash)
            return;

        StartDash();
    }

    void DashAttack()
    {
        anim.SetBool("isRun", false);
        anim.SetBool("isDash", true);

        transform.position += (Vector3)(dashDir * dashSpeed * Time.deltaTime);
        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0)
        {
            cooldownTimer = dashCooldown;
            float distance = Vector2.Distance(centerPoint.position, player.position);

            if (distance <= attackDistance)
                currentState = State.Follow;
            else
            {
                targetPoint = GetNearestPoint();
                currentState = State.Return;
            }
        }
    }

    // =============================
    // 返回巡逻点
    // =============================
    void ReturnPatrol()
    {
        anim.SetBool("isRun", true);
        anim.SetBool("isDash", false);
        float dir =
        targetPoint.position.x - transform.position.x;

        Flip(dir);

        MoveTo(targetPoint.position, returnSpeed);

        // 用 centerPoint 判断距离
        if (Vector2.Distance(centerPoint.position, targetPoint.position) < 0.2f)
        {
            currentState = State.Patrol;
        }
    }

    // =============================
    // 移动
    // =============================
    void MoveTo(Vector3 target, float speed)
    {
        Vector2 dir = (target - centerPoint.position).normalized;
        transform.position += (Vector3)(dir * speed * Time.deltaTime);
    }

    // =============================
    // 翻转
    // =============================
    void Flip(float x)
    {
        Vector3 scale = transform.localScale;
        scale.x = x > 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    // =============================
    // 最近巡逻点（用中心点）
    // =============================
    Transform GetNearestPoint()
    {
        float disA = Vector2.Distance(centerPoint.position, pointA.position);
        float disB = Vector2.Distance(centerPoint.position, pointB.position);
        return disA < disB ? pointA : pointB;
    }

    // =============================
    // Gizmos
    // =============================
    void OnDrawGizmosSelected()
    {
        if (centerPoint == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(centerPoint.position, detectDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centerPoint.position, attackDistance);

        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}