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
    private Queue<(Transform target, string text, AudioClip voice)> localQueue = new();
    private bool isLocalPlaying = false;

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
    private void ShowDialogOverActor(Transform target, string text, AudioClip voice)
    {
        localQueue.Enqueue((target, text, voice));
        if (!isLocalPlaying)
            StartCoroutine(ProcessLocalQueue());
    }
    private IEnumerator ProcessLocalQueue()
    {
        isLocalPlaying = true;

        while (localQueue.Count > 0)
        {
            var (target, text, voice) = localQueue.Dequeue();

            GameObject dialogGO = Instantiate(dialokPrefab, dialogParent);
            RectTransform dialogRect = dialogGO.GetComponent<RectTransform>();
            Vector3 offset = new(0, 0.75f, 0);

            TMP_Text dialogText = dialogRect.GetComponentInChildren<TMP_Text>();
            AudioSource audioSource = dialogGO.GetComponentInChildren<AudioSource>();
            TypewriterEffect typer = dialogText?.GetComponent<TypewriterEffect>();

            bool isDone = false;

            if (typer != null)
            {
                typer.StartTyping(text, () => isDone = true, audioSource, voice);
            }
            else
            {
                if (dialogText) dialogText.text = text;
                isDone = true;
            }

            float timer = 0f;
            float waitDuration = text.Length * 0.05f + 2f;

            while (timer < waitDuration)
            {
                timer += Time.deltaTime;
                UpdateDialogPositionOnce(dialogRect, target, offset);
                yield return null;
            }

            StartCoroutine(DestroyAfterFinish(dialogGO, () => isDone));
        }

        isLocalPlaying = false;
    }
    private void UpdateDialogPositionOnce(RectTransform dialogRect, Transform target, Vector3 offset)
    {
        if (dialogRect == null || target == null || Camera.main == null)
            return;

        Canvas canvas = dialogParent.GetComponentInParent<Canvas>();
        if (canvas == null)
            return;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        Vector3 worldPos = target.position + offset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, Camera.main, out localPoint);

        dialogRect.anchoredPosition = localPoint;
    }
    IEnumerator ShowDialogRoutine(RectTransform dialogRect, Transform target, Vector3 offset, string text)
    {
        TMP_Text dialogText = dialogRect.GetComponentInChildren<TMP_Text>();
        if (dialogText == null)
        {
            Debug.LogError("В prefab диалога нет TMP_Text!");
            yield break;
        }

        yield return StartCoroutine(TypewriterEffect(dialogText, text));

        yield return StartCoroutine(UpdateDialogPosition(dialogRect, target, offset, () => false));
    }
    private IEnumerator TypewriterEffect(TMP_Text textUI, string text)
    {
        textUI.text = "";
        foreach (char c in text)
        {
            textUI.text += c;
            yield return new WaitForSeconds(0.05f);
        }
    }
    private IEnumerator UpdateDialogPosition(RectTransform dialogRect, Transform target, Vector3 offset, System.Func<bool> isDone)
    {
        Canvas canvas = dialogParent.GetComponentInParent<Canvas>();
        if (canvas == null || Camera.main == null)
        {
            Debug.LogError("Не найден Canvas или Main Camera");
            yield break;
        }

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        Vector2 currentPos = dialogRect.anchoredPosition;
        float smoothSpeed = 15f;

        while (!isDone() && target != null)
        {
            Vector3 worldPos = target.position + offset;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            Vector2 targetPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, Camera.main, out targetPos);

            // Рассчитываем новое положение с отставанием
            // Чем выше lagFactor - тем медленнее диалог возвращается к цели, создавая запаздывание
            currentPos = Vector2.Lerp(currentPos, targetPos, Time.deltaTime * smoothSpeed);

            dialogRect.anchoredPosition = currentPos;

            yield return null;
        }
    }
    private IEnumerator DestroyAfterFinish(GameObject go, System.Func<bool> doneCondition)
    {
        yield return new WaitUntil(doneCondition);
        yield return new WaitForSeconds(0f);
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
