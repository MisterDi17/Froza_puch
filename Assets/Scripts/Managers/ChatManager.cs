using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;

public class ChatManager : MonoBehaviour
{

    private Queue<string> dialogQueue = new Queue<string>();
    private bool isDialogPlaying = false;

    [Header("UI")]
    public TMP_InputField inputField;
    public GameObject chatMessagePrefab;
    public Transform messageParent;
    public ScrollRect scrollRect;

    [Header("Dropdowns")]
    public TMP_Dropdown selectActorBX;
    public TMP_Dropdown prawBX;

    [Header("Defaults")]
    public Sprite defaultAvatar;

    [Header("Dialog")]
    public GameObject dialogPrefab; // Префаб с FollowPlayerUI + TypewriterEffect
    public Transform playerTransform; // Игрок, над которым будет диалог
    public static bool IsChatFocused { get; private set; }



    // Список актёров с аватарками — можешь расширить
    [System.Serializable]
    public class ActorData
    {
        public string actorName;
        public Sprite avatar;
    }

    public ActorData[] actors;

    void Start()
    {
        Debug.Log("Создаётся диалоговое окно...");

        if (dialogPrefab == null)
        {
            Debug.LogError("dialogPrefab is NULL");
        }
        else
        {
            Debug.Log("dialogPrefab: " + dialogPrefab.name);
        }

        if (playerTransform == null)
        {
            Debug.LogError("playerTransform is NULL");
        }
        else
        {
            Debug.Log("playerTransform: " + playerTransform.name);
        }

        inputField.onEndEdit.AddListener((text) =>
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SubmitMessage(text);
            }
        });

        AddSystemMessage("Добро пожаловать в чат!");
    }

    private void Update()
    {
        IsChatFocused = inputField != null && inputField.isFocused;

        // Открытие поля ввода по Enter
        if (Input.GetKeyDown(KeyCode.Return) && !inputField.isFocused)
        {
            inputField.ActivateInputField();
        }

        // Закрытие по Escape
        if (Input.GetKeyDown(KeyCode.Escape) && inputField.isFocused)
        {
            inputField.DeactivateInputField();
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void SubmitMessage(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        string senderName = selectActorBX.options[selectActorBX.value].text;
        Sprite senderAvatar = GetActorAvatar(senderName);

        AddChatMessage(senderName, senderAvatar, text);

        // Вставляем этот вызов
        if (senderName == "Plaeyr") // можно сделать имя переменной
        {
            ShowDialogOverPlayer(text);
        }

        inputField.text = "";
        inputField.ActivateInputField();
    }


    private void ShowDialogOverPlayer(string message)
    {
        dialogQueue.Enqueue(message);

        if (!isDialogPlaying)
            StartCoroutine(ProcessDialogQueue());
    }

    private IEnumerator WaitAndDestroyDialog(GameObject dialog, TypewriterEffect typer, string message)
    {
        bool typingDone = false;

        typer.StartTyping(message, () =>
        {
            typingDone = true;
        });

        // Ждём пока завершится печать
        yield return new WaitUntil(() => typingDone);

        // Ждём ещё 5 секунд
        yield return new WaitForSeconds(5f);

        Destroy(dialog);
    }

    private IEnumerator ProcessDialogQueue()
    {
        isDialogPlaying = true;

        while (dialogQueue.Count > 0)
        {
            string message = dialogQueue.Dequeue();

            // Создаём диалог
            GameObject dialogGO = Instantiate(dialogPrefab, UnityEngine.Object.FindFirstObjectByType<Canvas>().transform);
            dialogGO.transform.localPosition = Vector3.zero;

            FollowPlayerUI follow = dialogGO.GetComponent<FollowPlayerUI>();
            if (follow != null)
            {
                follow.player = playerTransform;
                follow.offset = new Vector3(0, 0.75f, 0);
            }

            TypewriterEffect typer = dialogGO.GetComponentInChildren<TypewriterEffect>(true);
            if (typer != null)
            {
                typer.StartTyping(message);
            }

            // Ждём, пока текст полностью напишется
            yield return new WaitUntil(() => typer != null && typer.IsFinished);

            yield return new WaitForSeconds(3f);

            Destroy(dialogGO);

            // Подождать 3 секунды перед следующим сообщением
            yield return new WaitForSeconds(0.1f);
        }

        isDialogPlaying = false;
    }




    public void AddSystemMessage(string text)
    {
        AddChatMessage("Система", defaultAvatar, text, isSystem: true);
    }

    public void AddChatMessage(string senderName, Sprite avatar, string message, bool isSystem = false)
    {
        GameObject go = Instantiate(chatMessagePrefab, messageParent);

        // Поиск компонентов внутри префаба
        var avatarImg = go.transform.Find("NameMesseg/Afatarca").GetComponent<Image>();
        var nameTxt = go.transform.Find("NameMesseg/NameMeseg").GetComponent<TMP_Text>();
        var msgTxt = go.transform.Find("Messeg").GetComponent<TMP_Text>();

        if (avatarImg) avatarImg.sprite = avatar != null ? avatar : defaultAvatar;
        if (nameTxt) nameTxt.text = senderName;
        if (msgTxt) msgTxt.text = isSystem ? $"<i>{message}</i>" : message;

        // Запускаем отложенное обновление скролла
        StartCoroutine(DelayedScrollToBottom());
    }

    private IEnumerator DelayedScrollToBottom()
    {
        yield return null; // кадр 1
        yield return null; // кадр 2
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)messageParent);
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }


    private Sprite GetActorAvatar(string actorName)
    {
        foreach (var actor in actors)
        {
            if (actor.actorName == actorName)
                return actor.avatar;
        }
        return defaultAvatar;
    }
}
