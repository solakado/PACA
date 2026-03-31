using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(RawImage))]
public class VideoBlurController : MonoBehaviour
{
    [Header("模糊效果设置")]
    [Tooltip("目标模糊强度，推荐2-8，数值越大越模糊")]
    public float targetBlurStrength = 5f;
    [Tooltip("延时多久后开始模糊（单位：秒）")]
    public float delayTime = 5f;
    [Tooltip("从清晰到完全模糊的过渡时长（单位：秒）")]
    public float fadeDuration = 2f;
    [Tooltip("采样次数，推荐4-8，数值越大模糊越平滑，性能消耗越高")]
    public int sampleCount = 6;

    [Header("触发设置")]
    [Tooltip("场景启动后自动触发模糊")]
    public bool autoTriggerOnStart = true;
    [Tooltip("触发后禁用脚本，避免重复执行")]
    public bool disableAfterTrigger = true;

    private RawImage _rawImage;
    private Material _blurMaterial;
    private Coroutine _blurCoroutine;

    void Awake()
    {
        // 自动获取组件和材质
        _rawImage = GetComponent<RawImage>();
        _blurMaterial = _rawImage.material;

        // 初始化：模糊强度归0，设置采样次数
        _blurMaterial.SetFloat("_BlurSize", 0);
        _blurMaterial.SetInt("_SampleCount", sampleCount);
    }

    void Start()
    {
        if (autoTriggerOnStart)
        {
            TriggerBlur();
        }
    }

    /// <summary>
    /// 对外调用的触发方法，可通过按钮、事件、其他代码手动调用
    /// </summary>
    public void TriggerBlur()
    {
        if (_blurCoroutine != null) StopCoroutine(_blurCoroutine);
        _blurCoroutine = StartCoroutine(BlurFadeCoroutine());
    }

    /// <summary>
    /// 重置模糊，回到完全清晰状态
    /// </summary>
    public void ResetBlur()
    {
        if (_blurCoroutine != null) StopCoroutine(_blurCoroutine);
        _blurMaterial.SetFloat("_BlurSize", 0);
        enabled = true;
    }

    // 模糊过渡协程
    private IEnumerator BlurFadeCoroutine()
    {
        // 等待设定的延时时间
        yield return new WaitForSeconds(delayTime);

        float timeElapsed = 0;
        float startBlur = 0;

        // 平滑过渡模糊强度
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float currentBlur = Mathf.Lerp(startBlur, targetBlurStrength, timeElapsed / fadeDuration);
            _blurMaterial.SetFloat("_BlurSize", currentBlur);
            yield return null;
        }

        // 确保最终值准确
        _blurMaterial.SetFloat("_BlurSize", targetBlurStrength);

        if (disableAfterTrigger) enabled = false;
    }
}