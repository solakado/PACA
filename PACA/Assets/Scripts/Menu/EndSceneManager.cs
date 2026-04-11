using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneManager : MonoBehaviour
{
    public void LoadTargetScene()
    {
        // 加载索引1的游戏主场景，重新开局
        Debug.Log(111);
        SceneManager.LoadScene(0);
    }
}
