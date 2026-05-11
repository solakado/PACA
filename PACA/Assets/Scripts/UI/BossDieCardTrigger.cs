using UnityEngine;

public class BossDieCardTrigger : MonoBehaviour
{
    [Header("把场景里的Boss拖进来")]
    public GameObject bossObj;

    [Header("卡片预制体")]
    public GameObject cardPrefab;

    [Header("Canvas Transform")]
    public Transform canvasTrans;

    private bool hasPopCard = false;

    void Update()
    {
        // 已经弹过卡片 直接返回
        if (hasPopCard) return;

        // Boss被销毁 == 引用为空
        if (bossObj == null)
        {
            PopCard();
            hasPopCard = true;
        }
    }

    void PopCard()
    {
        if (cardPrefab == null || canvasTrans == null)
        {
            Debug.LogError("卡片预制体或Canvas没赋值！");
            return;
        }

        // 生成卡片居中
        GameObject card = Instantiate(cardPrefab, canvasTrans);
        RectTransform rect = card.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;

        Debug.Log("Boss已销毁，弹出奖励卡片");
    }
}