using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinish4 : MonoBehaviour
{
    // 玩家触碰终点触发器时调用
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 只响应玩家物体（请给你的玩家物体加上Tag：Player）
        if (other.CompareTag("Player"))
        {
            // 关键：恢复游戏时间（避免之前暂停过，导致场景异常）
            Time.timeScale = 1;
            // 加载索引2的EndGame通关场景
            SceneManager.LoadScene(10);
        }
    }

    
}