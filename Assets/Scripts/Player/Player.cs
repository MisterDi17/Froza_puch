using UnityEngine;

[RequireComponent(typeof(AbilityManager))]
public class Player : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private PlayerSettings settings;

    private void Awake()
    {
        if (!settings.Movement)
            settings.SetMovement(GetComponentInChildren<GameMovement>());
        if (!settings.Rotation)
            settings.SetRotation(GetComponentInChildren<GameRotation>());
        if (!settings.Animator)
            settings.SetAnimator(GetComponentInChildren<Animator>());
    }
    private void FixedUpdate()
    {
        if (ChatManager.IsChatFocused)
            return;

        var input = GameInput.Instance;
        if (input == null) return;

        bool isAiming = input.IsAiming();
        Vector2 moveDir = input.GetMovementVector();
        bool isRunning = input.IsRunning();

        settings.Movement.SetSprinting(isRunning);

        if (isAiming)
        {
            Vector2 mouseWorld = Camera.main != null
                ? (Vector2)Camera.main.ScreenToWorldPoint(input.GetMousePosition())
                : Vector2.zero;
            Vector2 lookDir = (mouseWorld - (Vector2)transform.position).normalized;
            settings.Rotation.UpdateRotation(lookDir);
            settings.Movement.ProcessMovement(moveDir * settings.AimSlowdown, false);
        }
        else
        {
            if (moveDir.magnitude > settings.MouseDeadzone)
                settings.Rotation.UpdateRotation(moveDir);
            settings.Movement.ProcessMovement(moveDir, isRunning);
        }

        if (settings.Animator != null && settings.Animator.runtimeAnimatorController != null)
        {
            settings.Animator.SetBool("isMoving", settings.Movement.IsMoving);
            settings.Animator.SetBool("isRunning", settings.Movement.CurrentSpeedPercent > 0.5f);
        }
    }
}
