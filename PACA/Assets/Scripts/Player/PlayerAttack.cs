using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private int combo = 0;
    public bool isAttacking = false;
    private bool attackQueued = false;

    private Animator anim;
    private PlayerController controller;
    private PlayerRespawn playerRespawn;
    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;
    // 【新增】保存原来的重力
    private float originalGravityScale;

    private PlayerInputControl inputControl;

    void Awake()
    {
        inputControl = new PlayerInputControl();
        anim = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
        playerRespawn = GetComponent<PlayerRespawn>();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        // 【新增】记住初始重力
        originalGravityScale = rb.gravityScale;
    }

    void OnEnable()
    {
        inputControl.Gameplay.Attack.started += OnAttackInput;
        inputControl.Enable();
    }

    void OnDisable()
    {
        inputControl.Gameplay.Attack.started -= OnAttackInput;
        inputControl.Disable();
    }

    void Update()
    {
        if (playerRespawn.isDead)
            return;

        controller.enabled = !isAttacking;

        if (isAttacking)
        {
            rb.velocity = Vector2.zero;
        }
    }

    void OnAttackInput(InputAction.CallbackContext ctx)
    {
        if (playerRespawn.isDead) return;

        if (!isAttacking)
        {
            StartAttack(1);
        }
        else if (combo == 1)
        {
            attackQueued = true;
        }
    }

    void StartAttack(int num)
    {
        combo = num;
        isAttacking = true;
        attackQueued = false;

        // 1. 锁死刚体
        rb.velocity = Vector2.zero;
        // 【新增】关掉重力，彻底不下落
        rb.gravityScale = 0;

        // 2. 【核心：配合你的 PhysicsCheck】
        physicsCheck.isAttacking = true; // 告诉 PhysicsCheck 别检测了
        physicsCheck.isGround = true;     // 强制设为地面，动画机不切跳跃

        anim.SetInteger("Combo", num);
    }

    // 动画最后一帧调用
    public void OnAttackAnimationFinished()
    {
        if (combo == 1 && attackQueued)
        {
            StartAttack(2);
        }
        else
        {
            EndAttack();
        }
    }

    void EndAttack()
    {
        isAttacking = false;
        combo = 0;
        attackQueued = false;

        // 【新增】恢复原来的重力
        rb.gravityScale = originalGravityScale;

        // 【关键：恢复】
        physicsCheck.isAttacking = false; // 恢复 PhysicsCheck 检测
        // 这里不手动改 isGround，让 PhysicsCheck 下一帧自己检测真实值

        anim.SetInteger("Combo", 0);
    }

    public void ResetAttackState()
    {
        EndAttack();
    }
}