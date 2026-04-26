using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthUI : MonoBehaviour
{
    public GameObject heartPrefab; // 一滴血预制体
    public Transform container;    // UI容器（Horizontal Layout）

    private List<GameObject> hearts = new List<GameObject>();

    public void Init(int maxHealth)
    {
        // 清空
        foreach (var h in hearts)
            Destroy(h);

        hearts.Clear();

        // 生成血滴
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, container);
            hearts.Add(heart);
        }
    }

    //public void UpdateHealth(int currentHealth, int maxHealth)
    //{
    //    for (int i = 0; i < hearts.Count; i++)
    //    {
    //        hearts[i].SetActive(i < currentHealth);
    //    }
    //}
    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        // 如果血量上限变化 → 重建UI
        if (hearts.Count != maxHealth)
        {
            Init(maxHealth);
        }

        for (int i = 0; i < hearts.Count; i++)
        {
            Image img = hearts[i].GetComponent<Image>();

            if (i < currentHealth)
                img.color = Color.white;
            else
                img.color = Color.gray;
        }
    }
}