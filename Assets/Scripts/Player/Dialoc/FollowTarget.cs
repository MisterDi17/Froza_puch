using UnityEngine;
using UnityEngine.UI;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(99, -2f, 0);

    private RectTransform rectTransform;
    private Canvas canvas;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void LateUpdate()
    {
        if (target == null || canvas == null || Camera.main == null) return;

        Vector3 worldPosition = target.position + offset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            Camera.main,
            out localPoint
        );

        rectTransform.localPosition = localPoint;
    }
}
