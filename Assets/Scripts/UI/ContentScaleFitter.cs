using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class ContentScaleFitter : MonoBehaviour
{
    public RectTransform target; // что измеряем (например, Text)
    public Vector2 baseSize = new Vector2(100, 100); // размер, при котором scale = 1
    public bool scaleX = true;
    public bool scaleY = true;

    private void Update()
    {
        if (!target) return;

        Vector2 contentSize = target.sizeDelta;

        float scaleXFactor = contentSize.x / baseSize.x;
        float scaleYFactor = contentSize.y / baseSize.y;

        Vector3 newScale = transform.localScale;

        if (scaleX) newScale.x = scaleXFactor;
        if (scaleY) newScale.y = scaleYFactor;

        transform.localScale = newScale;
    }
}
