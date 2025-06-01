using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatInputHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private ChatManager chatManager; // или кто там у тебя принимает текст

    void Update()
    {
        if (inputField.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Return)) // Enter
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    // Просто вставка новой строки
                    int caretPos = inputField.caretPosition;
                    inputField.text = inputField.text.Insert(caretPos, "\n");
                    inputField.caretPosition = caretPos + 1;
                }
                else
                {
                    // Отправка сообщения
                    SubmitMessage();
                }
            }
        }
    }

    private void SubmitMessage()
    {
        string text = inputField.text.Trim();
        if (string.IsNullOrEmpty(text)) return;

        chatManager.SubmitMessage(text); // передаём текст
        inputField.text = "";
        inputField.ActivateInputField(); // вернуть фокус
    }
}
