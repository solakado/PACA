using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private void Awake()
    {
        inputControl = new PlayerInputControl();
        physicsCheck = GetComponent<PhysicsCheck>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        //跳跃
        inputControl.Gameplay.Jump.started += Jump;
    }
    private void OnEnable()
    {
        inputControl.Enable();
    }
    private void Update()
    {
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();
        Move();



    }
    private void FixedUpdate()
    {
        Move();
    }
    public void Move()
    {
        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);
        float faceDir = (float)transform.localScale.x;
        if (inputDirection.x > 0)
        {
            faceDir = 0.5f;
        }
        if (inputDirection.x < 0)
        {
            faceDir = -0.5f;
        }

        transform.localScale = new Vector3(faceDir, 0.5f, 0.5f);

    }
    private void Jump(InputAction.CallbackContext context)
    {
        if (physicsCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }
        
    }

}
