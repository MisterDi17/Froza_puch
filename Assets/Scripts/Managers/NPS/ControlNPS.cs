using UnityEngine;

public class ControlNPS : MonoBehaviour
{
    [Header("Настройки")]
    public float stoppingDistance = 0.05f;
    public bool isRunning = false;

    [Header("Ссылки")]
    [SerializeField] private Transform npcTransform;
    [SerializeField] private GameMovement movementController;
    [SerializeField] private GameRotation rotationController;

    private Vector2? _targetPosition;
    private Transform _currentTarget;

    public Vector2? TargetPosition => _targetPosition;

    private void Awake()
    {
        if (npcTransform == null) npcTransform = transform;
        if (movementController == null) movementController = GetComponent<GameMovement>();
        if (rotationController == null) rotationController = GetComponent<GameRotation>();
    }

    public void SetTarget(Transform target)
    {
        _currentTarget = target;
        _targetPosition = target?.position;
    }

    public void SetTargetPosition(Vector2 position)
    {
        _targetPosition = position;
        _currentTarget = null;
    }

    private void Update()
    {
        if (_currentTarget != null)
        {
            _targetPosition = _currentTarget.position;
        }

        if (!_targetPosition.HasValue) return;

        Vector2 direction = (_targetPosition.Value - (Vector2)npcTransform.position).normalized;

        movementController.ProcessMovement(direction, isRunning);
        rotationController?.UpdateRotation(direction);

        if (Vector2.Distance(npcTransform.position, _targetPosition.Value) <= stoppingDistance)
        {
            if (_currentTarget != null && _currentTarget.name.StartsWith("Target_"))
            {
                Destroy(_currentTarget.gameObject);
            }
            _targetPosition = null;
            _currentTarget = null;
        }
    }
}