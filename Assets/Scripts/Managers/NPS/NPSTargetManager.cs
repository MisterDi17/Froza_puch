using System.Collections.Generic;
using UnityEngine;

public class NPSTargetManager : MonoBehaviour
{
    [System.Serializable]
    public class RoutePoint
    {
        public string pointName;
        public Vector2 position;
        public SpriteRenderer marker;
    }

    [Header("Настройки")]
    public List<RoutePoint> routePoints = new();
    public float stopDistance = 0.1f;
    public bool loopPatrol = true;
    public Sprite pointSprite;

    private ControlNPS _npcController;
    private LineRenderer _lineRenderer;
    private int _currentPointIndex = 0;
    private bool _movingForward = true;

    private void Awake()
    {
        _npcController = transform.root.GetComponentInChildren<ControlNPS>();
        Debug.Log($"ControlNPS найден? {_npcController != null}");
        InitializeLineRenderer();
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetKey(KeyCode.LeftShift))
                AddRoutePoint(mousePos);
            else
                SetSingleTarget(mousePos);
        }

        UpdatePathVisual();
    }

    private void AddRoutePoint(Vector2 position)
    {
        var newPoint = new RoutePoint
        {
            pointName = $"Точка {routePoints.Count + 1}",
            position = position,
            marker = CreateVisualMarker(position)
        };

        routePoints.Add(newPoint);

        if (routePoints.Count == 1)
            SetCurrentTarget(0);
    }

    private SpriteRenderer CreateVisualMarker(Vector2 position)
    {
        GameObject marker = new GameObject("PathMarker");
        marker.transform.position = position;
        var sr = marker.AddComponent<SpriteRenderer>();
        sr.sprite = pointSprite;
        sr.color = Color.green;
        return sr;
    }

    private void SetCurrentTarget(int index)
    {
        _npcController.SetTargetPosition(routePoints[index].position);
        _currentPointIndex = index;
    }

    private void UpdatePathVisual()
    {
        if (routePoints.Count < 2)
        {
            _lineRenderer.positionCount = 0;
            return;
        }

        _lineRenderer.positionCount = routePoints.Count;
        for (int i = 0; i < routePoints.Count; i++)
        {
            _lineRenderer.SetPosition(i, routePoints[i].position);
        }
    }

    private void InitializeLineRenderer()
    {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = Color.green;
        _lineRenderer.endColor = Color.red;
    }

    private void SetSingleTarget(Vector2 position)
    {
        ClearPath();
        AddRoutePoint(position);
    }

    private void ClearPath()
    {
        foreach (var point in routePoints)
        {
            if (point.marker != null)
                Destroy(point.marker.gameObject);
        }
        routePoints.Clear();
    }
}