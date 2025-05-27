using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldTimeManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI timeDisplay;
    public TextMeshProUGUI timeSpeedText;
    public Slider timeSpeedSlider;

    [Header("Настройки времени")]
    public float timeMultiplier = 1f;
    [SerializeField] private float powerFactor = 6.643856f; // чтобы 2.0 = 100x
    private float currentSeconds = 0f;

    void Start()
    {
        timeSpeedSlider.minValue = 0f;
        timeSpeedSlider.maxValue = 2f;
        timeSpeedSlider.value = 1f;

        timeSpeedSlider.onValueChanged.AddListener(OnTimeSpeedChanged);
        UpdateTimeSpeedDisplay();
    }

    void Update()
    {
        if (timeMultiplier > 0f)
        {
            currentSeconds += Time.deltaTime * timeMultiplier;
            UpdateTimeDisplay();
        }
    }

    void OnTimeSpeedChanged(float value)
    {
        if (value <= 1f)
        {
            timeMultiplier = value;
        }
        else
        {
            // Чтобы при value = 2 получить 100
            float maxValue = 2f;
            float maxMultiplier = 100f;

            // Считаем нормализованное значение от 1 до 2 (0–1 интервал)
            float normalized = (value - 1f) / (maxValue - 1f);

            // Теперь растим от 1x до 100x с экспонентой по 10 в степени
            timeMultiplier = Mathf.Lerp(1f, maxMultiplier, Mathf.Pow(normalized, 1.5f));
        }

        timeMultiplier = Mathf.Clamp(timeMultiplier, 0f, 100f); // защита
        UpdateTimeSpeedDisplay();
    }

    void UpdateTimeSpeedDisplay()
    {
        timeSpeedText.text = $"{timeMultiplier:0.#}x";
    }

    void UpdateTimeDisplay()
    {
        int totalSeconds = Mathf.FloorToInt(currentSeconds);

        int seconds = totalSeconds % 60;
        int minutes = (totalSeconds / 60) % 60;
        int hours = (totalSeconds / 3600) % 24;
        int days = (totalSeconds / 86400) % 365;
        int years = totalSeconds / (86400 * 365);

        timeDisplay.text = $"{years:D4}:{days:D3}:{hours:D2}:{minutes:D2}:{seconds:D2}";
    }
}
