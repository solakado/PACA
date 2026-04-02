using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    // ====================== 翻转专用 ======================
    private float baseScaleX; // 记录原始缩放大小，永不改变
    private float baseScaleY;
    private float baseScaleZ;

    private void Awake()
    {
        inputControl = new PlayerInputControl();
        physicsCheck = GetComponent<PhysicsCheck>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();

        // 跳跃
        inputControl.Gameplay.Jump.started += Jump;

        // 记录初始缩放（关键！保证翻转不影响大小）
        baseScaleX = Mathf.Abs(transform.localScale.x);
        baseScaleY = transform.localScale.y;
        baseScaleZ = transform.localScale.z;
    }

    private void OnEnable()
    {
        inputControl.Enable();
    }

    // 【重要修复】防止输入泄漏报错
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
        Move();
    }

    public void Move()
    {
        // 移动逻辑（你原来的）
        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);

        // ====================== 安全左右翻转 ======================
        if (inputDirection.x > 0.01f)
        {
            Flip(false); // 朝右
        }
        else if (inputDirection.x < -0.01f)
        {
            Flip(true);  // 朝左
        }
    }

    // ====================== 核心翻转方法 ======================
    // 只改变方向，不改变大小，永远不受缩放影响
    void Flip(bool faceLeft)
    {
        transform.localScale = new Vector3(
            faceLeft ? -baseScaleX : baseScaleX,  // X只改正负
            baseScaleY,                           // Y永远不变
            baseScaleZ                            // Z永远不变
        );
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (physicsCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
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

//    private void Awake()
//    {
//        inputControl = new PlayerInputControl();
//        physicsCheck = GetComponent<PhysicsCheck>();
//        rb = GetComponent<Rigidbody2D>();
//        coll = GetComponent<CapsuleCollider2D>();
//        //跳跃
//        inputControl.Gameplay.Jump.started += Jump;
//    }
//    private void OnEnable()
//    {
//        inputControl.Enable();
//    }
//    private void Update()
//    {
//        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();
//        Move();



//    }
//    private void FixedUpdate()
//    {
//        Move();
//    }
//    public void Move()
//    {
//        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);
//        float faceDir = (float)transform.localScale.x;

//        if (inputDirection.x > 0)
//        {
//            faceDir = 0.5f;

//        }
//        if (inputDirection.x < 0)
//        {
//            faceDir = -0.5f;

//        }
//        transform.localScale = new Vector3(faceDir, 0.5f, 0.5f);

//    }

//    private void Jump(InputAction.CallbackContext context)
//    {
//        if (physicsCheck.isGround)
//        {
//            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
//        }

//    }

//}
