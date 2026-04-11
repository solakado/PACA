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

        chatText.text += "\n<color=yellow>我：</color>" + question;
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
                content = "你是一个游戏里的友善村民小霞，说话简短、可爱、口语化，尽量不要用颜文字，表情包。"
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

                    chatText.text += "\n<color=green>村民：</color>" + ans;
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