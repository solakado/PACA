using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class FisherTalk : MonoBehaviour
{
    [Header("UI 绑定")]
    public GameObject imagePopup;    // 图文弹窗面板
    public Image imgPage1;           // 第一张介绍图
    public Image imgPage2;           // 第二张介绍图
    public TextMeshProUGUI tipPressE;           // 【按E交流】提示文字

    private bool isInRange = false;  // 是否在触发范围
    private int currentPage = 0;     // 当前页码 0=关闭 1=图1 2=图2
    private Rigidbody2D playerRb;    // 玩家刚体

    void Update()
    {
        // 1. 控制提示文字的显示/隐藏
        if (isInRange && currentPage == 0)
        {
            tipPressE.gameObject.SetActive(true);
        }
        else
        {
            tipPressE.gameObject.SetActive(false);
        }

        // 2. 按E打开第一张图
        if (isInRange && Keyboard.current.eKey.wasPressedThisFrame && currentPage == 0)
        {
            ShowPage1();
        }

        // 3. 鼠标点击切换图片
        if (currentPage != 0 && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (currentPage == 1)
                ShowPage2();
            else if (currentPage == 2)
                ClosePopup();
        }
    }

    void ShowPage1()
    {
        currentPage = 1;
        imagePopup.SetActive(true);
        imgPage1.gameObject.SetActive(true);
        imgPage2.gameObject.SetActive(false);
        FreezePlayer(true);
    }

    void ShowPage2()
    {
        currentPage = 2;
        imgPage1.gameObject.SetActive(false);
        imgPage2.gameObject.SetActive(true);
    }

    void ClosePopup()
    {
        currentPage = 0;
        imagePopup.SetActive(false);
        imgPage1.gameObject.SetActive(false);
        imgPage2.gameObject.SetActive(false);
        FreezePlayer(false);
    }

    void FreezePlayer(bool isFreeze)
    {
        if (playerRb == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                playerRb = player.GetComponent<Rigidbody2D>();
        }

        if (playerRb != null)
        {
            if (isFreeze)
            {
                playerRb.velocity = Vector2.zero;
                playerRb.simulated = false;
            }
            else
            {
                playerRb.simulated = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
        }
    }
}