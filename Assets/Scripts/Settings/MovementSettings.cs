using UnityEngine;

[System.Serializable]
public class MovementSettings
{
    [Header("Speeds")]
    public float walkingSpeed = 5f;
    public float runningSpeed = 10f;

    [Header("Acceleration")]
    public float baseAcceleration = 5f;
    public float runningAccelerationMultiplier = 0.7f;

    [Header("Deceleration")]
    public float baseDeceleration = 8f;
    public float runningDecelerationMultiplier = 0.5f;
    public float reverseDirectionPenalty = 2f;
}