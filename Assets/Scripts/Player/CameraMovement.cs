using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float movingSpeed = 5f;
    [SerializeField] private float aimCameraOffset = 3f;
    [SerializeField] private float maxAimDistance = 10f;
    [SerializeField] private float cameraZPosition = -10f; // Вынесем Z-позицию как параметр

    private void Awake()
    {
        if (!playerTransform)
        {
            playerTransform = GameObject.FindGameObjectWithTag(playerTag)?.transform;
        }

        ResetCameraPosition();
    }

    private void Update()
    {
        if (GameInput.Instance != null && GameInput.Instance.IsAiming())
        {
            UpdateAimCamera();
        }
        else
        {
            UpdateNormalCamera();
        }
    }

    private void UpdateNormalCamera()
    {
        if (playerTransform == null) return;

        Vector3 target = new Vector3(
            playerTransform.position.x,
            playerTransform.position.y,
            cameraZPosition
        );

        transform.position = Vector3.Lerp(transform.position, target, movingSpeed * Time.deltaTime);
    }

    private void UpdateAimCamera()
    {
        if (playerTransform == null || Camera.main == null) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(
            new Vector3(
                GameInput.Instance.GetMousePosition().x,
                GameInput.Instance.GetMousePosition().y,
                -cameraZPosition // Компенсируем Z-позицию камеры
            )
        );

        Vector3 playerPosition = playerTransform.position;
        Vector2 aimDirection = (mousePosition - playerPosition).normalized;

        // Рассчитываем смещение камеры
        float distance = Vector2.Distance(mousePosition, playerPosition);
        float offsetFactor = Mathf.Clamp(distance / maxAimDistance, 0f, 1f);
        Vector2 cameraOffset = aimDirection * (aimCameraOffset * offsetFactor);

        Vector3 target = new Vector3(
            playerPosition.x + cameraOffset.x,
            playerPosition.y + cameraOffset.y,
            cameraZPosition
        );

        transform.position = Vector3.Lerp(transform.position, target, movingSpeed * Time.deltaTime);
    }

    private void ResetCameraPosition()
    {
        if (playerTransform)
        {
            transform.position = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y,
                cameraZPosition
            );
        }
    }
}