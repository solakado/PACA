using UnityEngine;

public class Coin : MonoBehaviour
{
    private Animator _animator;
    private AudioSource _audioSource; // 新增：音频源引用
    [Header("吃硬币音效")]
    public AudioClip collectSound; //  Inspector里拖入音效文件

    private static readonly int IsCollected = Animator.StringToHash("IsCollected");

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>(); // 新增：获取组件
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CollectCoin();
        }
    }

    void CollectCoin()
    {
        // 1. 播放吃硬币音效（核心！）
        _audioSource.PlayOneShot(collectSound);

        // 2. 切换消失动画
        _animator.SetBool(IsCollected, true);
    }

    public void OnDisappearAnimationFinish()
    {
        Destroy(gameObject);
    }
}