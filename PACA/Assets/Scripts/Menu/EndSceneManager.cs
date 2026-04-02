using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneManager : MonoBehaviour
{
    // 点击【返回主菜单】按钮调用
    public void BackToMainMenu()
    {
        // 加载索引0的主菜单场景
        SceneManager.LoadScene(0);
    }

    // 点击【重新开始】按钮调用（可选）
    public void ReplayGame()
    {
        // 加载索引1的游戏主场景，重新开局
        SceneManager.LoadScene(1);
    }

    // （可选）点击【退出游戏】按钮调用
    public void QuitGame()
    {
        Application.Quit();
    }
}
