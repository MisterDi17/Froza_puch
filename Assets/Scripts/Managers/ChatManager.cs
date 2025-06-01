using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class ChatManager : MonoBehaviour
{
    [SerializeField] private GameObject nofilaChatPrefab;
    [SerializeField] private GameObject dialokPrefab;
    [SerializeField] private Transform dialogParent;

    private Queue<(string, ActorCollector.FullActorData)> globalQueue = new Queue<(string, ActorCollector.FullActorData)>();
    private bool isGlobalPlaying = false;

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
    public Sprite defaultProfileArt;

    public ActorCollector actorCollector;

    public static bool IsChatFocused { get; private set; }

    void Start()
    {
        inputField.onEndEdit.AddListener((text) =>
        {
            if (Input.GetKeyDown(KeyCode.Return))
                SubmitMessage(text);
        });

        var actors = actorCollector.GetAllActors();
        Debug.Log($"Всего актёров: {actors.Count}");
        foreach (var a in actors)
            Debug.Log($"Актёр: {a.name}");

        AddSystemMessage("Добро пожаловать в чат!");
        PopulateActorDropdown();
    }
    void Update()
    {
        IsChatFocused = inputField != null && inputField.isFocused;

        if (Input.GetKeyDown(KeyCode.Return) && !inputField.isFocused)
        {
            inputField.ActivateInputField();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && inputField.isFocused)
        {
            inputField.DeactivateInputField();
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    private void PopulateActorDropdown()
    {
        selectActorBX.ClearOptions();
        var actorNames = new List<string>();

        foreach (var actor in actorCollector.GetAllActors())
        {
            if (!string.IsNullOrEmpty(actor.name))
                actorNames.Add(actor.name);
        }

        if (actorNames.Count == 0)
            actorNames.Add("Unknown");

        selectActorBX.AddOptions(actorNames);
    }
    public void SubmitMessage(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        string senderName = selectActorBX.options[selectActorBX.value].text;
        var actorData = actorCollector.GetActorData(senderName);

        if (actorData == null)
        {
            Debug.LogWarning($"Актёр с именем {senderName} не найден!");
            return;
        }

        AddChatMessage(senderName, actorData.avatar, text);

        if (prawBX.options[prawBX.value].text == "Локально")
        {
            var targetActor = actorCollector.GetActor(senderName);
            if (targetActor != null)
                ShowDialogOverActor(targetActor.transform, text, actorData.voice);
        }
        else if (prawBX.options[prawBX.value].text == "Глобально")
        {
            ShowGlobalDialog(text, actorData);
        }

        inputField.text = "";
        inputField.ActivateInputField();
    }
    private void ShowDialogOverActor(Transform target, string message, AudioClip voice)
    {
        if (dialogParent == null)
        {
            Debug.LogError("Dialog Parent не назначен!");
            return;
        }

        GameObject dialog = Instantiate(dialokPrefab, dialogParent);
        dialog.transform.localPosition = Vector3.zero; // по желанию

        var followTarget = dialog.GetComponent<FollowTarget>();
        if (followTarget != null)
        {
            followTarget.target = target;
        }
        else
        {
            Debug.LogError("dialokPrefab не содержит компонент FollowTarget!");
        }

        TMP_Text textComponent = dialog.transform.Find("Fon/DIalocText")?.GetComponent<TMP_Text>();
        AudioSource audioSource = dialog.transform.Find("Fon/DIalocText/Audio")?.GetComponent<AudioSource>();

        if (textComponent != null)
        {
            bool isDone = false;
            var typer = textComponent.GetComponent<TypewriterEffect>();

            if (typer != null)
                typer.StartTyping(message, () => isDone = true, audioSource, voice);
            else
            {
                textComponent.text = message;
                isDone = true;
            }

            StartCoroutine(DestroyAfterFinish(dialog, () => isDone));
        }
    }
    private IEnumerator DestroyAfterFinish(GameObject go, System.Func<bool> doneCondition)
    {
        yield return new WaitUntil(doneCondition);
        yield return new WaitForSeconds(2f);
        Destroy(go);
    }
    private void ShowGlobalDialog(string message, ActorCollector.FullActorData actor)
    {
        globalQueue.Enqueue((message, actor));
        if (!isGlobalPlaying)
            StartCoroutine(ProcessGlobalQueue());
    }
    private IEnumerator ProcessGlobalQueue()
    {
        isGlobalPlaying = true;

        while (globalQueue.Count > 0)
        {
            var (message, actor) = globalQueue.Dequeue();

            if (dialogParent == null)
            {
                Debug.LogError("Dialog Parent не назначен!");
                yield break;
            }

            GameObject go = Instantiate(nofilaChatPrefab, dialogParent);
            go.transform.localPosition = Vector3.zero; // опционально, если нужен сброс позиции
            var nameText = go.transform.Find("Panel/NameProf/NameActer")?.GetComponent<TMP_Text>();
            var image = go.transform.Find("Panel/NameProf/Image")?.GetComponent<Image>();
            var textProf = go.transform.Find("Panel/TextProf")?.GetComponent<TMP_Text>();
            var audioSource = go.transform.Find("Panel/TextProf/Audio")?.GetComponent<AudioSource>();
            var art = go.transform.Find("Art")?.GetComponent<Image>();

            if (nameText) nameText.text = actor.name;
            if (image) image.sprite = actor.avatar;
            if (art) art.sprite = actor.poster;

            if (textProf != null)
            {
                bool isDone = false;
                var typer = textProf.GetComponent<TypewriterEffect>();
                if (typer != null)
                    typer.StartTyping(message, () => isDone = true, audioSource, actor.voice);
                else
                {
                    textProf.text = message;
                    isDone = true;
                }

                yield return new WaitUntil(() => isDone);
            }

            yield return new WaitForSeconds(2f);
            Destroy(go);
        }

        isGlobalPlaying = false;
    }
    public void AddSystemMessage(string text)
    {
        AddChatMessage("Система", defaultAvatar, text, isSystem: true);
    }
    public void AddChatMessage(string senderName, Sprite avatar, string message, bool isSystem = false)
    {
        GameObject go = Instantiate(chatMessagePrefab, messageParent);
        var avatarImg = go.transform.Find("NameMesseg/Afatarca").GetComponent<Image>();
        var nameTxt = go.transform.Find("NameMesseg/NameMeseg/Name").GetComponent<TMP_Text>();
        var msgTxt = go.transform.Find("Messeg").GetComponent<TMP_Text>();

        if (avatarImg) avatarImg.sprite = avatar ?? defaultAvatar;
        if (nameTxt) nameTxt.text = senderName;
        if (msgTxt) msgTxt.text = isSystem ? $"<i>{message}</i>" : message;

        StartCoroutine(DelayedScrollToBottom());
    }
    private IEnumerator DelayedScrollToBottom()
    {
        yield return null;
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)messageParent);
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
