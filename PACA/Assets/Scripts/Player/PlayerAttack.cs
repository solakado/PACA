using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerAttack : MonoBehaviour
{

    [Header("引用")]
    public GameObject waveProjectilePrefab;
    public Transform waveProjectilePoint;
    public TextMeshProUGUI waveCountText;
    [Header("波动弹次数")]
    public int maxWaveCount = 3;
    public int currentWaveCount = 0;

    [Header("基本参数")]
    private int combo = 0;
    public bool isAttacking = false;
    private bool attackQueued = false;

    public int constrain = 1;

    private Animator anim;
    private PlayerController controller;
    private PlayerRespawn playerRespawn;
    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;
    // 【新增】保存原来的重力
    private float originalGravityScale;

    public bool canControl = true;

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
        inputControl.Gameplay.Skill.started += OnSkillInput;
        inputControl.Enable();
    }

    void OnDisable()
    {
        inputControl.Gameplay.Attack.started -= OnAttackInput;
        inputControl.Gameplay.Skill.started -= OnSkillInput;
        inputControl.Disable();
    }
    void Start()
    {
        UpdateWaveUI();
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
        if (constrain>=2)
        {
            return;
        }
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
        constrain++;
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

    void OnSkillInput(InputAction.CallbackContext ctx)
    {
        if (playerRespawn.isDead) return;

        if (isAttacking) return; // 防止攻击中释放技能
        if (currentWaveCount <= 0) return;

        currentWaveCount--;
        UpdateWaveUI();


        StartWaveAttack();
    }
    void StartWaveAttack()
    {
        isAttacking = true;

        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;

        physicsCheck.isAttacking = true;
        physicsCheck.isGround = true;

        // 关键：触发技能动画
        anim.SetTrigger("Wave");

    }
    public void SpawnWaveProjectile()
    {
        if (waveProjectilePrefab == null || waveProjectilePoint == null)
        {
            return;
        }

        Debug.Log("发射波动弹");

        GameObject fb = Instantiate(waveProjectilePrefab, waveProjectilePoint.position, Quaternion.identity);

        float dir = transform.localScale.x > 0 ? 1 : -1;

        fb.GetComponent<WaveProjectile>()?.Setup(new Vector2(dir, 0));
        if (dir > 0)
        {
            fb.GetComponent<WaveProjectile>().sr.flipX = true;
        }
    }
    public void OnWaveAnimationFinished()
    {
        EndAttack();
    }

    public void AddWaveCount(int amount)
    {
        currentWaveCount += amount;
        currentWaveCount = Mathf.Clamp(currentWaveCount, 0, maxWaveCount);

        UpdateWaveUI(); // 每次变化就刷新UI
    }
    void UpdateWaveUI()
{
    if (waveCountText != null)
    {
        waveCountText.text = currentWaveCount.ToString();
    }
}


}