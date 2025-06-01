using UnityEngine;
using UnityEngine.UI;

public class SliderAction : MonoBehaviour
{
    private static SliderAction instance;
    private Slider sliderUI;
    private RectTransform rectTransform;
    private Transform target; // над кем рисуем (игрок или объект)

    public static void Show(Vector3 worldPosition, float duration)
    {
        if (instance == null)
        {
            // Если нет инстанса, спавним в Canvas
            GameObject go = Resources.Load<GameObject>("Prefab/SliderAction");
            instance = go.GetComponent<SliderAction>();
        }

        // Пришлём позицию
        instance.SetTargetPosition(worldPosition);
        instance.sliderUI.maxValue = duration;
        instance.sliderUI.value = 0f;
        instance.gameObject.SetActive(true);
    }

    public static void UpdateProgress(float normalized)
    {
        if (instance == null) return;
        instance.sliderUI.value = Mathf.Lerp(0f, instance.sliderUI.maxValue, normalized);
    }

    public static void Hide()
    {
        if (instance == null) return;
        instance.gameObject.SetActive(false);
    }

    private void Awake()
    {
        sliderUI = GetComponentInChildren<Slider>();
        rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;
        // Если хотим, чтобы слайдер «летел» за игроком или объектом, можно обновлять позицию:
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target != null ? target.position : Vector3.zero);
        rectTransform.position = screenPos + Vector3.up * 50f; // выше головы
    }

    private void SetTargetPosition(Vector3 worldPos)
    {
        // Нам достаточно знать worldPos, чтобы позиционировать слайдер. 
        // Можно превратить в Transform, но для простоты пусть target будет временным пустым GameObject:
        if (target == null)
        {
            GameObject tGO = new GameObject("SliderActionTarget");
            target = tGO.transform;
        }
        target.position = worldPos + Vector3.up * 1.5f; // поднимаем чуть выше
    }
}
