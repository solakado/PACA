using UnityEngine;
using UnityEngine.InputSystem;

public class EscMenuInOneScene : MonoBehaviour
{
    public GameObject menuPanel;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            bool show = !menuPanel.activeSelf;
            menuPanel.SetActive(show);
        }
    }
}