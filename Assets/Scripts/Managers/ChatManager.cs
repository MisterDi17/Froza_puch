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

    private Queue<(string, FullActorData)> globalQueue = new Queue<(string, FullActorData)>();
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
    public bool isChek;
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
        PopulateActorDropdown();
        if(isChek == true) StartCoroutine(TutorialSequence());
    }
    private IEnumerator TutorialSequence()
    {
        yield return ShowGlobal("Добро пожаловать, Мой дорогой друг.", "Наставник");
        yield return new WaitForSeconds(1.5f);

        yield return ShowGlobal("Ты находишься в тренировочной среде. Сейчас мы активируем основные протоколы.", "Наставник");
        yield return new WaitForSeconds(2f);

        yield return ShowLocal("Готов. Диагностика завершена. Запуск обучения.", "Модуль 3g 2.0.3");

        // Обучение движению
        yield return ShowGlobal("Первый протокол — перемещение.", "Наставник");
        yield return ShowGlobal("Используй клавиши W, A, S, D для передвижения.", "Наставник");
        yield return new WaitForSeconds(2.5f);

        yield return ShowGlobal("Осмотрись на территории. Когда будешь готов — продолжим.", "Наставник");
        yield return new WaitForSeconds(3f);

        // Взаимодействие
        yield return ShowGlobal("Перед тобой находится объект взаимодействия.", "Наставник");
        yield return ShowGlobal("Чтобы взаимодействовать, подойди к объекту и нажми E.", "Наставник");
        yield return ShowGlobal("Для расширенного выбора действий зажми E и удерживай — появится список.", "Наставник");
        yield return new WaitForSeconds(3f);

        yield return ShowLocal("Команда взаимодействия получена. Проверяю функциональность...", "Модуль 3g 2.0.3");

        // Ctrl: красться и осматриваться
        yield return ShowGlobal("Следующий модуль — осмотр и скрытность.", "Наставник");
        yield return ShowGlobal("Зажми Ctrl, чтобы присесть и двигаться тише.", "Наставник");
        yield return ShowGlobal("Также удержание Ctrl позволяет свободно осматривать территорию.", "Наставник");
        yield return new WaitForSeconds(3f);

        // Shift: рывок и способности
        yield return ShowGlobal("Активируем модуль рывка.", "Наставник");
        yield return ShowGlobal("Удерживай Shift, чтобы совершить рывок в выбранном направлении.", "Наставник");
        yield return ShowGlobal("Ты можешь отключить рывок в панели способностей, если он тебе мешает.", "Наставник");
        yield return new WaitForSeconds(3.5f);

        yield return ShowLocal("Рывок активен. Панель способностей доступна.", "Модуль 3g 2.0.3");

        // Система чата
        yield return ShowGlobal("Теперь о системе общения.", "Наставник");
        yield return ShowGlobal("В нижней части чата ты можешь выбрать актёра и режим сообщения.", "Наставник");
        yield return ShowGlobal("Режимы включают глобальные сообщения и локальные, отображаемые над головой персонажа.", "Наставник");
        yield return new WaitForSeconds(3f);

        yield return ShowLocal("Выбор контекста общения завершён. Передаю привет, игрок.", "Модуль 3g 2.0.3");

        // Время
        yield return ShowGlobal("И последнее — управление временем.", "Наставник");
        yield return ShowGlobal("В правом нижнем углу ты увидишь панель времени.", "Наставник");
        yield return ShowGlobal("С её помощью ты можешь ускорять или замедлять ход событий.", "Наставник");
        yield return new WaitForSeconds(3f);

        // Завершение
        yield return ShowGlobal("Обучение завершено.", "Наставник");
        yield return ShowGlobal("Можешь приступать к исследованию системы. Удачи, Модуль 3g 2.0.3.", "Наставник");

        yield return ShowLocal("Миссия принята. Переход в активный режим.", "Модуль 3g 2.0.3");
    }
    private IEnumerator ShowGlobal(string message, string actorName)
    {
        var actor = actorCollector.GetActorData(actorName);
        if (actor == null)
        {
            Debug.LogWarning($"[ChatManager] Не найден актёр: {actorName}");
            yield break;
        }

        ShowGlobalDialog(message, actor);
        float waitTime = message.Length * 0.05f + 1.5f;
        yield return new WaitForSeconds(waitTime);
    }
    private IEnumerator ShowLocal(string message, string actorName)
    {
        var actor = actorCollector.GetActor(actorName);
        var actorData = actorCollector.GetActorData(actorName);

        if (actor == null || actorData == null)
        {
            Debug.LogWarning($"[ChatManager] Не найден актёр: {actorName}");
            yield break;
        }

        ShowDialogOverActor(actor.transform, message, actorData.voice);
        float waitTime = message.Length * 0.05f + 1.5f;
        yield return new WaitForSeconds(waitTime);
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
    private void ShowGlobalDialog(string message, FullActorData actor)
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
