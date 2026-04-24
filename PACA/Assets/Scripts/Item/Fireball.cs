using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("火球参数")]
    public float speed = 5f;
    public float lifetime = 3f; // 3秒后自动销毁

    // 火球飞行方向
    private Vector2 moveDirection;
    public SpriteRenderer sr;

    // 初始化火球飞行方向
    public void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    public void Setup(Vector2 direction)
    {
        moveDirection = direction;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // 持续移动火球
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 碰到玩家 或 非Boss物体时销毁火球
        if (other.CompareTag("Player") || !other.CompareTag("Boss"))
        {
            Destroy(gameObject);
        }
    }
}

//using UnityEngine;

//public class Fireball : MonoBehaviour
//{
//    [Header("火球参数")]
////    public float speed = 5f;
////    public float lifetime = 3f;

////    private Vector2 direction;
////    private Rigidbody2D rb;
////    private SpriteRenderer sr;

////    void Awake()
////    {
////        rb = GetComponent<Rigidbody2D>();
////        sr = GetComponent<SpriteRenderer>();
////    }

////    // ================= 初始化 =================
////    public void Setup(Vector2 dir)
////    {
////        if (dir == Vector2.zero)
////        {
////            Debug.LogError("Fireball方向是0！已自动修复");

////            dir = Vector2.right; // 默认向右
////        }

////        dir = dir.normalized;

////        rb.velocity = new Vector2(dir.x * speed, 0);

////        if (sr != null)
////            sr.flipX = dir.x < 0;

////        Destroy(gameObject, lifetime);
////    }

////    // ================= 碰撞 =================
////    void OnTriggerEnter2D(Collider2D other)
////    {
////        if (other.CompareTag("Player") || !other.CompareTag("Boss")) { Destroy(gameObject); }
////    }
////}
