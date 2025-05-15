using UnityEngine;

[System.Serializable]
public class RotationSettings
{
    [Header("Base Rotation")]
    public float minRotationSpeed = 50f;    // Минимальная скорость поворота (когда почти смотрим на курсор)
    public float maxRotationSpeed = 300f;   // Максимальная скорость поворота (когда смотрим в противоположную сторону)
    public float accelerationCurve = 1.5f;  // Кривая ускорения поворота
}