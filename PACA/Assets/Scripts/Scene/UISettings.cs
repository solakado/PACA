using UnityEngine;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
    [Header("设置面板")]
    public GameObject settingPanel;

    [Header("全屏")]
    public Toggle fullScreenToggle;

    [Header("音量")]
    public Slider volumeSlider;

    void Start()
    {
        // 1. 先强制设置为全屏
        Screen.fullScreen = true;

        // 2. 再同步UI → 这样开关就会自动勾选
        fullScreenToggle.isOn = true;

        // 音量初始化
        volumeSlider.minValue = 0;
        volumeSlider.maxValue = 1;
        volumeSlider.value = AudioListener.volume;

        // 绑定事件
        fullScreenToggle.onValueChanged.AddListener(SetFullScreen);
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    // 打开设置
    public void OpenSetting()
    {
        settingPanel.SetActive(true);
    }

    // 关闭设置
    public void CloseSetting()
    {
        settingPanel.SetActive(false);
    }

    // 全屏切换
    void SetFullScreen(bool isFull)
    {
        Screen.fullScreen = isFull;
    }

    // 音量调节
    void SetVolume(float vol)
    {
        AudioListener.volume = vol;
    }
}