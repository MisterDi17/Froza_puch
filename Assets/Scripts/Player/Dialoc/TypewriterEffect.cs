using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private float timeBetweenLetters = 0.05f;
    [SerializeField] private float audioInterval = 0.1f;

    [SerializeField] private AudioClip typingSound;
    [SerializeField] private AudioSource audioSource;

    private bool isTyping = false;
    public bool IsFinished { get; private set; }

    public void Initialize(AudioSource source, AudioClip clip)
    {
        audioSource = source;
        typingSound = clip;
    }

    public void StartTyping(string message, System.Action onComplete = null)
    {
        StopAllCoroutines();
        StartCoroutine(TypeText(message, onComplete, audioSource, typingSound));
    }

    public void StartTyping(string message, System.Action onComplete, AudioSource source, AudioClip clip)
    {
        Initialize(source, clip);
        StartTyping(message, onComplete);
    }

    private IEnumerator TypeText(string message, System.Action onComplete, AudioSource source, AudioClip clip)
    {
        isTyping = true;
        IsFinished = false;

        TMP_Text textComponent = GetComponent<TMP_Text>();
        textComponent.text = "";

        float lastSoundTime = 0f;

        for (int i = 0; i < message.Length; i++)
        {
            textComponent.text += message[i];

            // 🎵 Проигрываем звук только если это НЕ пробел
            if (clip != null && source != null && message[i] != ' ' && message[i] != '.')
            {
                if (Time.time - lastSoundTime >= audioInterval)
                {
                    source.PlayOneShot(clip);
                    lastSoundTime = Time.time;
                }
            }

            yield return new WaitForSeconds(timeBetweenLetters);
        }


        IsFinished = true;
        isTyping = false;
        onComplete?.Invoke();
    }
}
