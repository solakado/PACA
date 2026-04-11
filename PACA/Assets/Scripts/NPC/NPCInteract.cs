using UnityEngine;
using UnityEngine.InputSystem;

public class NPCInteract : MonoBehaviour
{
    [Header("UI")]
    public GameObject interactHintUI;
    public GameObject chatUI;

    [Header("Input")]
    public InputActionReference interactAction;

    [Header("Player")]
    public PlayerController playerController;
    public PlayerAttack playerAttack;

    private bool playerInRange = false;

    void OnEnable()
    {
        interactAction.action.Enable();
        interactAction.action.performed += OnInteract;
    }

    void OnDisable()
    {
        interactAction.action.performed -= OnInteract;
        interactAction.action.Disable();
    }

    void Start()
    {
        if (chatUI != null)
            chatUI.SetActive(false);

        if (interactHintUI != null)
            interactHintUI.SetActive(false);
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!playerInRange) return;

        if (!chatUI.activeSelf)
        {
            OpenChat();
        }
    }

    void OpenChat()
    {
        chatUI.SetActive(true);
        interactHintUI.SetActive(false);

        if (playerController != null)
            playerController.DisableControl();
        if (playerAttack != null)
            playerAttack.DisableControl();
    }

    public void CloseChat()
    {
        chatUI.SetActive(false);

        if (playerController != null)
            playerController.EnableControl();

        if (playerInRange)
            interactHintUI.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            interactHintUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactHintUI != null)
                interactHintUI.SetActive(false);

            CloseChatSafe();
        }
    }
    void CloseChatSafe()
    {
        if (chatUI != null)
        {
            chatUI.SetActive(false);
        }

        if (playerController != null)
        {
            playerController.EnableControl();
        }
        if (playerAttack != null)
        {
            playerAttack.EnableControl();
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return;

        if (col is BoxCollider2D box)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(box.offset, box.size);
        }
        else if (col is CircleCollider2D circle)
        {
            Gizmos.DrawWireSphere(
                (Vector2)transform.position + circle.offset,
                circle.radius
            );
        }
    }
}
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class NPCInteract : MonoBehaviour
//{
//    [Header("UI")]
//    public GameObject interactHintUI;
//    public GameObject chatUI;

//    [Header("Input")]
//    public InputActionReference interactAction;
//    public PlayerInput playerInput;

//    [Header("Player")]
//    public MonoBehaviour playerController; // 拖你的玩家控制脚本

//    private bool playerInRange = false;

//    void OnEnable()
//    {
//        interactAction.action.Enable();
//        interactAction.action.performed += OnInteract;
//    }

//    void OnDisable()
//    {
//        interactAction.action.performed -= OnInteract;
//        interactAction.action.Disable();
//    }

//    void Start()
//    {
//        interactHintUI.SetActive(false);
//        chatUI.SetActive(false);
//    }

//    private void OnInteract(InputAction.CallbackContext context)
//    {
//        if (!playerInRange) return;

//        if (!chatUI.activeSelf)
//        {
//            OpenChat();
//        }
//    }

//    void OpenChat()
//    {
//        chatUI.SetActive(true);
//        interactHintUI.SetActive(false);

//        if (playerInput != null)
//            playerInput.enabled = false;
//    }

//    public void CloseChat()
//    {
//        chatUI.SetActive(false);

//        if (playerInput != null)
//            playerInput.enabled = true;

//        if (playerInRange)
//            interactHintUI.SetActive(true);
//    }

//    void OnTriggerEnter2D(Collider2D other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            playerInRange = true;
//            interactHintUI.SetActive(true);
//        }
//    }

//    void OnTriggerExit2D(Collider2D other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            playerInRange = false;
//            interactHintUI.SetActive(false);
//            CloseChat();
//        }
//    }
//}