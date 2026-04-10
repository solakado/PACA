using UnityEngine;

public class Coin : MonoBehaviour
{
    // 引用 Animator 组件
    private Animator _animator;

    // 为了性能优化，我们将参数名转换为哈希值
    private static readonly int IsCollected = Animator.StringToHash("IsCollected");

    void Awake()
    {
        // 获取当前物体上的 Animator 组件
        _animator = GetComponent<Animator>();
    }

    // 2D 触发检测（如果是3D游戏，请改为 OnTriggerEnter)
    void OnTriggerEnter2D(Collider2D other)
    {
        // 检查碰撞的物体是否是玩家（建议给玩家物体设置 Tag 为 "Player"）
        if (other.CompareTag("Player"))
        {
            CollectCoin();
        }
    }

    void CollectCoin()
    {
        // 1. 告诉动画机播放消失动画
        _animator.SetBool(IsCollected, true);

        // 注意：不要在这里直接 Destroy(gameObject)，否则动画还没播物体就没了
    }

    // 【关键】这个方法将由“动画事件”调用
    public void OnDisappearAnimationFinish()
    {
        // 销毁硬币物体
        Destroy(gameObject);
    }
}