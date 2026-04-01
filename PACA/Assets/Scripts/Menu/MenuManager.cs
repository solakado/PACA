using UnityEngine;
using UnityEngine.SceneManagement; // 引用场景管理命名空间

public class MenuManager : MonoBehaviour
{
    // 点击“开始游戏”按钮时调用此方法
    public void StartGame()
    {
        // 加载索引为 1 的场景（即 GameScene）
        SceneManager.LoadScene(1);
    }

    // （可选）点击“退出游戏”按钮时调用
    public void QuitGame()
    {
        Application.Quit();
    }
}
