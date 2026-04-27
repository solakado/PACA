using UnityEngine;
using UnityEngine.Video;

public class AutoPlayNextVideo : MonoBehaviour
{
    // 第一个视频（拖拽赋值）
    public VideoClip firstVideo;
    // 第二个视频（拖拽赋值）
    public VideoClip secondVideo;
    // 关联场景中的VideoPlayer组件
    private VideoPlayer videoPlayer;

    void Start()
    {
        // 获取当前物体上的VideoPlayer组件
        videoPlayer = GetComponent<VideoPlayer>();
        // 绑定视频播放结束事件（关键：监听第一个视频播放完的瞬间）
        videoPlayer.loopPointReached += OnFirstVideoEnd;

        // 初始播放第一个视频（若已在VideoPlayer组件勾选Auto Play，可删除这行）
        videoPlayer.Play();
    }

    // 第一个视频播放结束后触发的方法
    void OnFirstVideoEnd(VideoPlayer vp)
    {
        // 取消事件绑定（避免重复触发，可选但推荐）
        videoPlayer.loopPointReached -= OnFirstVideoEnd;
        // 切换视频剪辑为第二个视频
        videoPlayer.clip = secondVideo;
        // 立即播放第二个视频
        videoPlayer.Play();
    }
}
