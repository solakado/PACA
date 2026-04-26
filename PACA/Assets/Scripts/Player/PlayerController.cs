
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

    [Header("控制开关")]
    public bool canControl = true;

    private float baseScaleX;
    private float baseScaleY;
    private float baseScaleZ;
    private PlayerHealth health;

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
    private void Start()
    {
        health = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (!canControl)
        {
            inputDirection = Vector2.zero;
            return;
        }

        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (!canControl) return;

        if (!isDashing)
            Move();
    }

    public void Move()
    {
        // 修复：不要乘 deltaTime
        rb.velocity = new Vector2(inputDirection.x * speed*Time.deltaTime, rb.velocity.y);

        if (inputDirection.x > 0.01f)
            Flip(false);
        else if (inputDirection.x < -0.01f)
            Flip(true);
    }

    private void Dash(InputAction.CallbackContext context)
    {
        if (!canControl) return;

        if (isDashing || !canDash || GetComponent<PlayerRespawn>().isDead)
            return;

        StartCoroutine(DashCoroutine());
    }

    IEnumerator DashCoroutine()
    {
        health.isInvincible = true;
        isDashing = true;
        canDash = false;

        physicsCheck.isDashing = true;
        physicsCheck.isGround = true;

        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;

        float dir = transform.localScale.x > 0 ? 1 : -1;
        rb.velocity = new Vector2(dir * dashForce, 0f);

        yield return new WaitForSeconds(dashTime);
        health.isInvincible = false;
        isDashing = false;
        rb.gravityScale = 3.5f;

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
        if (!canControl) return;

        if (physicsCheck.isGround && !isDashing)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    // ================= 控制接口 =================

    public void DisableControl()
    {
        canControl = false;

        inputControl.Disable();

        // 防止滑动
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public void EnableControl()
    {
        canControl = true;

        inputControl.Enable();
    }
}