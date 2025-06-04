using UnityEngine;
using UnityEngine.UI;

public class UIActionSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private Transform targetToFollow;
    private Camera cam;

    public void Show(Transform target, float duration)
    {
        cam = Camera.main;
        targetToFollow = target;
        slider.value = 0;
        gameObject.SetActive(true);
    }

    public void UpdateProgress(float progress)
    {
        slider.value = Mathf.Clamp01(progress);
    }

    public void Hide()
    {
        Destroy(gameObject); // уничтожаем сам объект
    }

    private void Update()
    {
        if (targetToFollow != null && cam != null)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(targetToFollow.position + Vector3.up * 1.5f);

            // Получаем ссылку на родительский Canvas
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null) return;

            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            RectTransform rt = GetComponent<RectTransform>();

            Vector2 localPos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, cam, out localPos))
            {
                rt.localPosition = localPos;
            }
        }
    }

}
