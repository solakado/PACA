using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ZhuQueAI : MonoBehaviour
{
    public Transform player;

    [Header("巡逻点")]
    public Transform pointA;
    public Transform pointB;

    [Header("距离")]
    public float detectDistance = 6f;
    public float attackDistance = 20f;

    [Header("速度")]
    public float patrolSpeed = 2f;
    public float dashSpeed = 8f;
    public float returnSpeed = 5f;
    public float chaseSpeed = 4f;
    [Header("俯冲")]
    public float riseHeight = 10f;
    public float waitTime = 1f;
    private Vector2 dashDir;
    private Vector2 dashStartPos;

    private float dashDistance;
    private float dashTime;
    private float dashTimer;

    private Vector2 dashTarget;
    private Vector2 riseTarget;
    private float waitTimer;
    [Header("丢失目标")]
    public float loseTime = 2f;

    private Animator anim;

    private Transform targetPoint;

    private float loseTimer;

    enum State
    {
        Patrol,
        Dash,
        Rise,
        Wait,
        Return
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
        if (player == null) return;

        float distance =
            Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrol:

                Patrol();

                if (distance <= detectDistance)
                {
                    
                        StartDash();
                    
                }

                break;

            case State.Dash:

                //if (distance > attackDistance)
                //{
                //    loseTimer += Time.deltaTime;

                //    if (loseTimer >= loseTime)
                //    {
                //        targetPoint = GetNearestPoint();
                //        currentState = State.Return;
                //        loseTimer = 0;
                //    }
                //}
                //else
                //{
                //    DashAttack();
                //    loseTimer = 0; 
                //}

                DashAttack();
                break;

            case State.Rise:

                Rise();

                break;

            //case State.Wait:
            //    if (distance > attackDistance)
            //    {
            //        loseTimer += Time.deltaTime;

            //        if (loseTimer >= loseTime)
            //        {
            //            targetPoint = GetNearestPoint();
            //            currentState = State.Return;
            //            loseTimer = 0;
            //        }
            //    }
            //    else
            //    {
            //        WaitNext();
            //        loseTimer = 0;
            //    }



            //    break;
            case State.Wait:

                // 1. 玩家太远 -> 计时回归
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
                else
                {
                    loseTimer = 0;
                    WaitNext(); // 仍然保留等待计时（可以继续循环节奏）
                }

                break;

            case State.Return:

                ReturnPatrolPoint();

                if (distance <= detectDistance)
                {
                   
                    StartDash();
                }

                break;
        }
    }

    //void Patrol()
    //{
    //    //anim.SetBool("isFly", true);

    //    MoveTo(targetPoint.position, patrolSpeed);

    //    if (Vector2.Distance(transform.position,
    //        targetPoint.position) < 0.2f)
    //    {
    //        targetPoint =
    //            targetPoint == pointA ? pointB : pointA;
    //    }
    //}
    void Patrol()
    {
        //anim.SetBool("isFly", true);
        anim.SetBool("isDash", false);

        MoveTo(targetPoint.position, patrolSpeed);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            // 切换目标点
            targetPoint = targetPoint == pointA ? pointB : pointA;

            // 立即转向新目标点
            float dir = targetPoint.position.x - transform.position.x;
            Flip(dir);
        }
    }

    //void DashAttack()
    //{
    //    anim.SetBool("isDash", true);

    //    DashMoveTo(dashSpeed); // 只按固定方向冲

    //    if (Vector2.Distance(transform.position, dashTarget) < 0.3f)
    //    {
    //        StartRise();
    //    }
    //}
    void DashAttack()
    {
        anim.SetBool("isDash", true);

        transform.position += (Vector3)(dashDir * dashSpeed * Time.deltaTime);

        dashTimer += Time.deltaTime;

        float movedDistance =
            Vector2.Distance(dashStartPos, transform.position);

        // 双保险：距离到 OR 时间到
        //if (movedDistance >= dashDistance && movedDistance <= dashDistance + 0.1)
        //    if(dashTimer >= dashTime&& dashTimer <= dashTime+0.1)
        //    {
        //        StartRise();

        //    }
        if (movedDistance >= dashDistance || dashTimer >= dashTime)
        {
            StartRise();
            return;
        }

    }
    void Rise()
    {
        anim.SetBool("isDash", false);

        Vector3 pos = transform.position;

        pos.y += returnSpeed * Time.deltaTime;

        transform.position = pos;

        
        // 身体回正
        transform.rotation =
            Quaternion.Lerp(
                transform.rotation,
                Quaternion.identity,
        5f * Time.deltaTime
            );
        float dir = player.position.x - transform.position.x;
        Flip(dir);

        if (transform.position.y >= riseTarget.y)
        {
            waitTimer = waitTime;
            currentState = State.Wait;
        }
    }
    void StartRise()
    {
        if (currentState != State.Dash) return;
        riseTarget = new Vector2(
            transform.position.x,
            transform.position.y + riseHeight
        );

        anim.SetBool("isDash", false);

        currentState = State.Rise;
    }

    void WaitNext()
    {
        waitTimer -= Time.deltaTime;
        float dir = player.position.x - transform.position.x;
        Flip(dir);
        float distance = Vector2.Distance(transform.position, player.position);

        if (waitTimer <= 0)
        {
            if (distance <= detectDistance)
            {
                StartDash();
            }
            // 3. 中距离 -> 缓慢追踪玩家
            else 
            {
                SlowFollowPlayer();
            }
        }
    }
    void ReturnPatrolPoint()
    {

        anim.SetBool("isDash", false);
        MoveTo(targetPoint.position, returnSpeed);

        if (Vector2.Distance(transform.position,
            targetPoint.position) < 0.2f)
        {
            currentState = State.Patrol;
        }
    }
    void MoveTo(Vector3 target, float speed)
    {
        Vector2 dir =
            (target - transform.position).normalized;

        transform.Translate(dir * speed * Time.deltaTime);

        Flip(dir.x);
    }

    
    //void DashMoveTo(Vector3 target, float speed)
    //{
    //    Vector2 dir = (target - transform.position).normalized;

    //    transform.Translate(dir * speed * Time.deltaTime);

    //    Flip(dir.x);

    //    RotateBody(dir);
    //}
    //void DashMoveTo(Vector3 target, float speed)
    //{
    //    Vector2 dir = (target - transform.position).normalized;

    //    transform.Translate(dir * speed * Time.deltaTime);

    //    RotateBody(-dir);
    //}
    //void DashMoveTo(Vector3 target, float speed)
    //{
    //    Vector2 dir = (target - transform.position).normalized;
    //    transform.position += (Vector3)(dir * speed * Time.deltaTime);
    //    Flip(dir.x);
    //    RotateBody(-dashDir); // 只用初始方向
    //    //if (Vector2.Distance(transform.position, dashStartPos) >= dashDistance)
    //    //{
    //    //    currentState = State.Rise; // 或 Return / Stop
    //    //}
    //}
    void DashMoveTo(float speed)
    {
        transform.position += (Vector3)(dashDir * speed * Time.deltaTime);
    }
   
    //void RotateBody(Vector2 dir)
    //{
    //    float angle =
    //        Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180f;

    //    transform.rotation =
    //        Quaternion.Euler(0, 0, angle);
    //}
    void RotateBody(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 判断左右
        if (dir.x >= 0)
        {
            // 朝右
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // 朝左（镜像修正）
            transform.rotation = Quaternion.Euler(0, 0, angle + 180f);
        }
    }
    //void Flip(float x)
    //{
    //    Vector3 scale = transform.localScale;

    //    if (x > 0)
    //        scale.x = -Mathf.Abs(scale.x);
    //    else
    //        scale.x = Mathf.Abs(scale.x);

    //    transform.localScale = scale;
    //}
    void Flip(float x)
    {
        if (currentState == State.Dash) return;

        Vector3 scale = transform.localScale;

        if (x > 0)
            scale.x = -Mathf.Abs(scale.x);
        else
            scale.x = Mathf.Abs(scale.x);

        transform.localScale = scale;
    }

    
    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (currentState != State.Dash) return;

    //    if (collision.gameObject.CompareTag("isGround") ||
    //        collision.gameObject.CompareTag("Player"))
    //    {
    //        StopDash();
    //    }
    //}
    void StopDash()
    {
        StartRise();// 或 Return

        anim.SetBool("isDash", false);
    }
    void StartDash()
    {
        float dir = player.position.x - transform.position.x;
        Flip(dir);
        dashTarget = player.position;

        dashStartPos = transform.position;

        dashDir = (dashTarget - dashStartPos).normalized;

        dashDistance = Vector2.Distance(dashStartPos, dashTarget);

        dashTime = dashDistance / dashSpeed;

        dashTimer = 0f;

        RotateBody(dashDir);

        currentState = State.Dash;
    }

    void SlowFollowPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;

        float slowSpeed = chaseSpeed; // 比巡逻稍慢一点

        transform.position += (Vector3)(dir * slowSpeed * Time.deltaTime);

        Flip(dir.x);
    }

    Transform GetNearestPoint()
    {
        float disA =
            Vector2.Distance(transform.position, pointA.position);

        float disB =
            Vector2.Distance(transform.position, pointB.position);

        return disA < disB ? pointA : pointB;
    }
    void OnDrawGizmosSelected()
    {
        // ========= 检测范围 =========
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectDistance);

        // ========= 丢失范围 =========
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        // ========= 巡逻路线 =========
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawLine(pointA.position, pointB.position);

            Gizmos.DrawSphere(pointA.position, 0.2f);
            Gizmos.DrawSphere(pointB.position, 0.2f);
        }
    }
}