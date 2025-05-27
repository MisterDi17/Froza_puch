using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textComponent;

    [Header("Текст")]
    [TextArea(3, 10)]
    [SerializeField] private string textToType = "";

    [Header("Настройки")]
    [SerializeField] private float delay = 0.05f;
    [SerializeField] private bool startOnAwake = true;

    [Header("Звук")]
    [SerializeField] private AudioSource typeSound;

    private Coroutine typingCoroutine;
    private bool skipRequested = false;
    public bool IsFinished { get; private set; }

    private void Awake()
    {
        if (string.IsNullOrEmpty(textToType) && textComponent != null)
        {
            textToType = textComponent.text;
        }

        if (startOnAwake && textComponent != null)
        {
            StartTyping(); // Теперь есть такая перегрузка
        }
    }

    // 🔧 Перегрузка без аргументов
    public void StartTyping()
    {
        StartTyping(textToType, null);
    }

    // 🔧 Перегрузка с текстом без колбэка
    public void StartTyping(string text)
    {
        StartTyping(text, null);
    }

    // ✅ Основной метод с колбэком
    public void StartTyping(string text, System.Action onComplete = null)
    {
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>() as TextMeshProUGUI;

        StopAllCoroutines();
        StartCoroutine(TypeRoutine(text, onComplete));
    }


    private IEnumerator TypeRoutine(string text, System.Action onComplete)
    {
        IsFinished = false;
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>() as TextMeshProUGUI;

        textComponent.text = "";

        foreach (char c in text)
        {
            textComponent.text += c;

            // 🔊 Проигрываем звук для каждой буквы
            if (typeSound != null)
            {
                typeSound.Play();
            }

            yield return new WaitForSeconds(delay);
        }

        IsFinished = true;
        onComplete?.Invoke();
    }


    public void Skip()
    {
        skipRequested = true;
    }
}
