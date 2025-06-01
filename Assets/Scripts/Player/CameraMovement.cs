using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Camera Parameters")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float movingSpeed = 5f;
    [SerializeField] private float aimCameraOffset = 3f;
    [SerializeField] private float maxAimDistance = 10f;
    [SerializeField] private float cameraZPosition = -10f;

    private Camera mainCamera;

    private void Awake()
    {
        if (!playerTransform) playerTransform = GameObject.FindGameObjectWithTag(playerTag)?.transform;
        mainCamera = Camera.main;
        ResetCameraPosition();
    }

    private void Update()
    {
        if (playerTransform == null) return;

        Vector3 targetPosition = GetTargetCameraPosition();
        transform.position = Vector3.Lerp(transform.position, targetPosition, movingSpeed * Time.deltaTime);
    }

    private Vector3 GetTargetCameraPosition()
    {
        if (GameInput.Instance != null && GameInput.Instance.IsAiming() && mainCamera != null)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(
                new Vector3(
                    GameInput.Instance.GetMousePosition().x,
                    GameInput.Instance.GetMousePosition().y,
                    -cameraZPosition
                )
            );

            Vector2 playerPos2D = new Vector2(playerTransform.position.x, playerTransform.position.y);
            Vector2 mousePos2D = new Vector2(mousePosition.x, mousePosition.y);

            Vector2 aimDirection = (mousePos2D - playerPos2D).normalized;
            float distance = Vector2.Distance(mousePos2D, playerPos2D);
            float offsetFactor = Mathf.Clamp(distance / maxAimDistance, 0f, 1f);
            Vector2 cameraOffset = aimDirection * (aimCameraOffset * offsetFactor);

            return new Vector3(
                playerTransform.position.x + cameraOffset.x,
                playerTransform.position.y + cameraOffset.y,
                cameraZPosition
            );
        }
        else
        {
            return new Vector3(
                playerTransform.position.x,
                playerTransform.position.y,
                cameraZPosition
            );
        }
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

    private void OnDrawGizmosSelected()
    {
        // Просто визуализация границ камеры в редакторе
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera != null && mainCamera.orthographic)
        {
            float height = mainCamera.orthographicSize * 2f;
            float width = height * mainCamera.aspect;
            Vector3 size = new Vector3(width, height, 0.1f);

            Gizmos.color = new Color(0, 1, 1, 0.3f);
            Gizmos.DrawWireCube(transform.position, size);
        }
    }
}
