using UnityEngine;
using UnityEngine.UI;

public class UIActionSliderManager : MonoBehaviour
{
    [SerializeField] private Slider actionSlider;

    public void Show(Transform followTarget, float duration)
    {
        actionSlider.value = 0;
    }

    public void UpdateProgress(float progress)
    {
        actionSlider.value = Mathf.Clamp01(progress);
    }

    public void Hide()
    {
        actionSlider.value = 0;
    }
}
