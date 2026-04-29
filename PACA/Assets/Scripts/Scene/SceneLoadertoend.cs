using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;  // 加这个

public class SceneLoadertoend : MonoBehaviour
{
    void Update()
    {
        // 新版输入系统：按 Enter（回车）触发
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(2);
        }
    }
}