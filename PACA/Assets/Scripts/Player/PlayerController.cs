using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private PhysicsCheck physicsCheck;
    public PlayerInputControl inputControl;
    private CapsuleCollider2D coll;
    public Rigidbody2D rb;
    public Vector2 inputDirection;

    [Header("基本参数")]
    public float speed;
    public float jumpForce;

    [Header("冲刺设置")]
    public float dashForce = 15f;
    public float dashTime = 0.15f;
    public float dashCooldown = 1f;
    public bool isDashing = false;
    private bool canDash = true;

    private float baseScaleX;
    private float baseScaleY;
    private float baseScaleZ;

    private void Awake()
    {
        inputControl = new PlayerInputControl();
        physicsCheck = GetComponent<PhysicsCheck>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();

        inputControl.Gameplay.Jump.started += Jump;
        inputControl.Gameplay.Dash.started += Dash;

        baseScaleX = Mathf.Abs(transform.localScale.x);
        baseScaleY = transform.localScale.y;
        baseScaleZ = transform.localScale.z;
    }

    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inputControl.Disable();
    }

    private void Update()
    {
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (!isDashing)
            Move();
    }

    public void Move()
    {
        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);

        if (inputDirection.x > 0.01f)
            Flip(false);
        else if (inputDirection.x < -0.01f)
            Flip(true);
    }

    private void Dash(InputAction.CallbackContext context)
    {
        if (isDashing || !canDash || GetComponent<PlayerRespawn>().isDead)
            return;

        StartCoroutine(DashCoroutine());
    }

    IEnumerator DashCoroutine()
    {
        isDashing = true;
        canDash = false;

        // 【锁定：冲刺期间停止地面检测】
        physicsCheck.isDashing = true;
        physicsCheck.isGround = true; // 强制地面，动画不跳变

        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        float dir = transform.localScale.x > 0 ? 1 : -1;
        rb.velocity = new Vector2(dir * dashForce, 0f);

        yield return new WaitForSeconds(dashTime);

        // 恢复
        isDashing = false;
        rb.gravityScale = 3.5f;

        // 【解锁：恢复地面检测】
        physicsCheck.isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void Flip(bool faceLeft)
    {
        transform.localScale = new Vector3(
            faceLeft ? -baseScaleX : baseScaleX,
            baseScaleY,
            baseScaleZ);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (physicsCheck.isGround && !isDashing)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class PlayerController : MonoBehaviour
//{
//    private PhysicsCheck physicsCheck;
//    public PlayerInputControl inputControl;
//    private CapsuleCollider2D coll;
//    public Rigidbody2D rb;
//    public Vector2 inputDirection;

//    [Header("基本参数")]
//    public float speed;
//    public float jumpForce;

//    // ====================== 翻转专用 ======================
//    private float baseScaleX; // 记录原始缩放大小，永不改变
//    private float baseScaleY;
//    private float baseScaleZ;

//    private void Awake()
//    {
//        inputControl = new PlayerInputControl();
//        physicsCheck = GetComponent<PhysicsCheck>();
//        rb = GetComponent<Rigidbody2D>();
//        coll = GetComponent<CapsuleCollider2D>();

//        // 跳跃
//        inputControl.Gameplay.Jump.started += Jump;

//        // 记录初始缩放（关键！保证翻转不影响大小）
//        baseScaleX = Mathf.Abs(transform.localScale.x);
//        baseScaleY = transform.localScale.y;
//        baseScaleZ = transform.localScale.z;
//    }

//    private void OnEnable()
//    {
//        inputControl.Enable();
//    }

//    // 【重要修复】防止输入泄漏报错
//    private void OnDisable()
//    {
//        inputControl.Disable();
//    }

//    private void Update()
//    {
//        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();
//    }

//    private void FixedUpdate()
//    {
//        Move();
//    }

//    public void Move()
//    {
//        // 移动逻辑（你原来的）
//        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);

//        // ====================== 安全左右翻转 ======================
//        if (inputDirection.x > 0.01f)
//        {
//            Flip(false); // 朝右
//        }
//        else if (inputDirection.x < -0.01f)
//        {
//            Flip(true);  // 朝左
//        }
//    }

//    // ====================== 核心翻转方法 ======================
//    // 只改变方向，不改变大小，永远不受缩放影响
//    void Flip(bool faceLeft)
//    {
//        transform.localScale = new Vector3(
//            faceLeft ? -baseScaleX : baseScaleX,  // X只改正负
//            baseScaleY,                           // Y永远不变
//            baseScaleZ                            // Z永远不变
//        );
//    }

//    private void Jump(InputAction.CallbackContext context)
//    {
//        if (physicsCheck.isGround)
//        {
//            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
//        }
//    }
//}

