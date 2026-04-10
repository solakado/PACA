using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("音效挂载")]
    public AudioClip walkSound;  // 走路音效
    public AudioClip jumpSound;  // 跳跃音效
    public AudioClip attackSound;// 攻击音效

    private AudioSource _audio;
    private Rigidbody2D _rb;

    void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 移动时播放走路音，停下自动停
        PlayWalkSound();
    }

    // 走路循环音效
    void PlayWalkSound()
    {
        // 检测水平移动 && 在地面
        bool isMoving = Mathf.Abs(_rb.velocity.x) > 0.1f;

        if (isMoving)
        {
            if (!_audio.isPlaying)
            {
                _audio.clip = walkSound;
                _audio.loop = true;
                _audio.Play();
            }
        }
        else
        {
            _audio.loop = false;
        }
    }

    // 给动画事件调用：播放单次音效
    public void PlaySound(AudioClip clip)
    {
        _audio.PlayOneShot(clip);
    }
}