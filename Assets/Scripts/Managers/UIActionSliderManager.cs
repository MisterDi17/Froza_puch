using UnityEngine;
using UnityEngine.UI;

public class UIActionSliderManager : MonoBehaviour
{
    [SerializeField] private Slider actionSlider;

    public void Show(Vector3 worldPosition, float duration)
    {
        gameObject.SetActive(true);
        transform.position = Camera.main.WorldToScreenPoint(worldPosition);
        actionSlider.value = 0;
    }

    public void UpdateProgress(float progress)
    {
        actionSlider.value = Mathf.Clamp01(progress);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
