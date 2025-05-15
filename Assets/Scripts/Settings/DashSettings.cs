using UnityEngine;

[System.Serializable]
public class DashSettings
{
    [Header("Dash Settings")]
    [Tooltip("Включён ли рывок")]
    public bool isEnabled = true;

    [Tooltip("Максимальная дистанция рывка")]
    public float dashDistance = 5f;

    [Tooltip("Длительность самого рывка")]
    public float dashDuration = 0.2f;

    [Tooltip("Кулдаун перед следующим рывком")]
    public float cooldown = 1f;

    [Tooltip("Кривая скорости в течение рывка")]
    public AnimationCurve speedCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
}
