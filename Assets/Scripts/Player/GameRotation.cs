using UnityEngine;

public class GameRotation : MonoBehaviour
{
    [SerializeField] private RotationSettings settings;

    public void UpdateRotation(Vector2 targetDirection, bool isInstant = false)
    {
        if (targetDirection.magnitude < 0.1f) return;

        // Получаем текущий угол и целевой угол
        float currentAngle = transform.parent.eulerAngles.z + 90f; // Компенсация смещения спрайта
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

        // Вычисляем разницу углов (-180 до 180)
        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);
        float absAngleDiff = Mathf.Abs(angleDifference);

        // Рассчитываем скорость поворота на основе разницы углов
        float rotationSpeed = Mathf.Lerp(
            settings.minRotationSpeed,
            settings.maxRotationSpeed,
            Mathf.Pow(absAngleDiff / 180f, settings.accelerationCurve)
        );

        // Если мгновенный поворот (для резких изменений)
        if (isInstant)
        {
            transform.parent.rotation = Quaternion.Euler(0, 0, targetAngle - 90f);
            return;
        }

        // Плавный поворот
        float newAngle = Mathf.MoveTowardsAngle(
            currentAngle,
            targetAngle,
            rotationSpeed * Time.fixedDeltaTime
        );

        transform.parent.rotation = Quaternion.Euler(0, 0, newAngle - 90f);
    }
}