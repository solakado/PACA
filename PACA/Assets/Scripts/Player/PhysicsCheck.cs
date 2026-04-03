using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    private CapsuleCollider2D coll;
    [Header("检测参数")]
    public bool manual;
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;
    public LayerMask groundLayer;
    public float checkRaduis;

    [Header("状态")]
    public bool isGround;
    public bool touchLeftWall;
    public bool touchRightWall;

    // 【新增】死亡开关，死亡时停止检测
    public bool isDead;

    private void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();

        if (!manual)
        {
            rightOffset = new Vector2((coll.bounds.size.x + coll.offset.x) / 2, coll.offset.y / 2);
            leftOffset = new Vector2(-rightOffset.x, rightOffset.y);
        }
    }

    private void Update()
    {
        // 【关键】如果死了，直接跳过检测，isGround 保持不变
        if (isDead) return;

        Check();
    }

    public void Check()
    {
        //检测地面
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRaduis, groundLayer);

        //墙体判断
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRaduis, groundLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRaduis, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRaduis);
    }
}