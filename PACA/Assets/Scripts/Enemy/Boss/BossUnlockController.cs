using UnityEngine;

public class BossUnlockController : MonoBehaviour
{
    [Header("需要检测死亡的 Boss")]
    public GameObject baiHuBoss;   // 白虎
    public GameObject zhuQueBoss;  // 朱雀

    [Header("最终激活的 Boss")]
    public GameObject finalBoss;   // 反派 Boss

    private bool hasActivated = false;

    void Start()
    {
        // 开始时确保反派 Boss 未激活
        if (finalBoss != null)
        {
            finalBoss.SetActive(false);
        }
    }

    void Update()
    {
        if (hasActivated) return;

        // 判断两个 Boss 是否都死亡（被销毁）
        bool baiHuDead = baiHuBoss == null;
        bool zhuQueDead = zhuQueBoss == null;

        if (baiHuDead && zhuQueDead)
        {
            ActivateFinalBoss();
        }
    }

    void ActivateFinalBoss()
    {
        hasActivated = true;

        if (finalBoss != null)
        {
            finalBoss.SetActive(true);
            Debug.Log("反派 Boss 已激活！");
        }
    }
}