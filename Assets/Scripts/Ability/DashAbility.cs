using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DashAbility : MonoBehaviour, IAbility
{
    [SerializeField] private DashSettings settings;
    [SerializeField] private LayerMask obstacleLayers;

    private Rigidbody2D rb;
    private bool isDashing;
    private float cooldownTimer;

    public Vector2 DashDirection { get; private set; }

    public bool IsEnabled
    {
        get => settings.isEnabled;
        set => settings.isEnabled = value;
    }

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    public void Execute(Vector2? direction = null)
    {
        Vector2 dir = (direction.HasValue && direction.Value.magnitude > 0.1f)
            ? direction.Value.normalized
            : GetFacingDirection();
        AttemptDash(dir);
    }

    private void AttemptDash(Vector2 direction)
    {
        if (!settings.isEnabled || isDashing || cooldownTimer > 0f)
            return;

        DashDirection = direction;
        StartCoroutine(PerformDash(direction));
    }

    private IEnumerator PerformDash(Vector2 direction)
    {
        isDashing = true;
        cooldownTimer = settings.cooldown;

        Vector2 startPos = rb.position;
        Vector2 targetPos = startPos + direction * settings.dashDistance;

        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, settings.dashDistance, obstacleLayers);
        if (hit.collider != null)
            targetPos = hit.point - direction * 0.5f;

        float elapsed = 0f;
        while (elapsed < settings.dashDuration)
        {
            float t = elapsed / settings.dashDuration;
            rb.MovePosition(Vector2.Lerp(startPos, targetPos, t));
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPos);
        isDashing = false;
    }

    public Vector2 GetFacingDirection() => transform.up;
}
