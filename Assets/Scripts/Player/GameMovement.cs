using UnityEngine;

public class GameMovement : MonoBehaviour
{
    [SerializeField] private MovementSettings settings;

    private Rigidbody2D rb;
    private Vector2 currentVelocity;
    private bool isSprinting;
    public Vector2 CurrentVelocity => currentVelocity;
    public bool IsMoving => currentVelocity.magnitude > 0.1f;
    public float CurrentSpeedPercent => currentVelocity.magnitude / settings.runningSpeed;
    public MovementSettings Settings => settings;

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
    }
    public void SetSprinting(bool sprinting)
    {
        isSprinting = sprinting;
    }

    public void ProcessMovement(Vector2 inputDirection, bool isRunning)
    {
        float targetSpeed = isSprinting ? settings.runningSpeed : settings.walkingSpeed;
        float acceleration = settings.baseAcceleration * (isRunning ? settings.runningAccelerationMultiplier : 1f);
        float deceleration = settings.baseDeceleration * (isRunning ? settings.runningDecelerationMultiplier : 1f);

        if (inputDirection.magnitude > 0.1f)
        {
            float dotProduct = Vector2.Dot(inputDirection.normalized, currentVelocity.normalized);
            float effectiveAcceleration = dotProduct < -0.7f ?
                deceleration * settings.reverseDirectionPenalty :
                acceleration;

            currentVelocity = Vector2.Lerp(
                currentVelocity,
                inputDirection.normalized * targetSpeed,
                effectiveAcceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            currentVelocity = Vector2.Lerp(
                currentVelocity,
                Vector2.zero,
                deceleration * Time.fixedDeltaTime
            );
        }

        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }
}