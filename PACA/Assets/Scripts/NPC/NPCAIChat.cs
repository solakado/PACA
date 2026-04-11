using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.InputSystem;

public class NPCAIChat : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField inputField;
    public Button sendButton;
    public TextMeshProUGUI chatText;

    [Header("关闭")]
    public NPCInteract npcInteract; // 关联NPC

    [Header("Input")]
    public InputActionReference cancelAction; // ESC

    [Header("API")]
    public string apiKey = "你的key";
    private string apiUrl = "https://api.siliconflow.cn/v1/chat/completions";

    void OnEnable()
    {
        cancelAction.action.Enable();
        cancelAction.action.performed += OnCancel;
    }

    void OnDisable()
    {
        cancelAction.action.performed -= OnCancel;
        cancelAction.action.Disable();
    }

    void Start()
    {
        sendButton.onClick.AddListener(SendQuestion);
    }

    void OnCancel(InputAction.CallbackContext ctx)
    {
        if (gameObject.activeSelf)
        {
            npcInteract.CloseChat();
        }
    }

    public void SendQuestion()
    {
        string question = inputField.text.Trim();
        if (string.IsNullOrEmpty(question)) return;

        chatText.text += "\n<color=yellow>林晚星：</color>" + question;
        inputField.text = "";

        StartCoroutine(RequestSiliconFlow(question));
    }

    IEnumerator RequestSiliconFlow(string question)
    {
        ChatRequest req = new ChatRequest();
        req.model = "deepseek-ai/DeepSeek-V3.2";

        req.messages = new List<ChatMessage>
        {
            new ChatMessage {
                role = "system",
                content = "你是一个游戏里的npc小霞，说话简短、可爱、口语化，不要带上角色描写，回答时完整回答问题，不要回答与问题无关的事情。" +
                "你在此地为了引导女主击溃敌人的阴谋。" +
                "回答一定带上标点符号。" +
                "背景设定：每一座中国古代建筑，都是文明的活化石。飞檐斗拱藏着匠心智慧，青砖黛瓦沉淀民族记忆，是我们不可磨灭的文化根脉。" +
                "一群未来掠夺者，妄图损毁古代建筑、篡改历史，让千年奇迹在时光中湮灭，让未来再无这份文明厚重。" +
                "现代古建筑修复师林晚星，在修复文物时触碰时空古砖，被光晕送往古建筑尚存、危机潜伏的古代。" +
                "她从修复者变为守护者，将穿梭各朝代，守护濒危建筑，对抗篡改历史的敌人。" +
                "时光可逆，文明不可毁。林晚星携现代知识与敬畏，踏上跨越时空的守护之旅——拯救古代建筑，便是拯救文明未来。" +
                "询问操作时：主角操作ad左右移动，空格跳跃，shift冲刺，j攻击。除此以外不要回答" +
                "询问目的，或者自己要干什么时：收集物品为了修复古建筑，前往下一个目的地，继续阻止敌人。"
            },
            new ChatMessage {
                role = "user",
                content = question
            }
        };

        string json = JsonUtility.ToJson(req);

        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(data);
            www.downloadHandler = new DownloadHandlerBuffer();

            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    ChatResponse resp = JsonUtility.FromJson<ChatResponse>(www.downloadHandler.text);
                    string ans = resp.choices[0].message.content;

                    ans = System.Text.RegularExpressions.Regex.Replace(ans, @"[^\u0020-\u9FFF]", "");

                    chatText.text += "\n<color=black>小霞：</color>" + ans;
                }
                catch
                {
                    chatText.text += "\n<color=red>解析失败</color>";
                }
            }
            else
            {
                chatText.text += "\n<color=red>出错：</color>" + www.error;
            }
        }
    }
    
}
[System.Serializable]
public class ChatRequest
{
    public string model;
    public List<ChatMessage> messages;
}

[System.Serializable]
public class ChatMessage
{
    public string role;
    public string content;
}

[System.Serializable]
public class ChatResponse
{
    public List<ChatChoice> choices;
}

[System.Serializable]
public class ChatChoice
{
    public ChatMessage message;
}