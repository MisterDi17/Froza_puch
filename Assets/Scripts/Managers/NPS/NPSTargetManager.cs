using System.Collections.Generic;
using UnityEngine;

public class NPSTargetManager : MonoBehaviour
{
    [SerializeField] private ControlNPS controller;
    [SerializeField] private Sprite targetSprite;
    [SerializeField] private float stopDistance = 0.2f;
    [SerializeField] private bool showPath = true;
    [SerializeField] private bool isPingPongPatrol = true;

    private bool goingForward = true; // Направление движения
    private List<Vector3> pathPoints = new();
    private LineRenderer lineRenderer;
    private GameObject spriteContainer;

    private int currentTargetIndex = 0;

    private void Awake()
    {
        spriteContainer = new GameObject("PathSprites");

        // Line Renderer для визуализации пути
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 0;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                AddPathPoint(mousePos);
            else
                SetSingleTarget(mousePos);
        }

        // Проверка: дошли ли до текущей цели
        if (pathPoints.Count > 0 && controller.Target != null)
        {
            float distance = Vector2.Distance(controller.transform.position, pathPoints[currentTargetIndex]);
            if (distance <= stopDistance)
            {
                RemovePoint(currentTargetIndex);
            }
        }


        UpdateLineRenderer();
    }

    private void SetSingleTarget(Vector2 position)
    {
        ClearPath();
        AddPathPoint(position);
    }

    private void AddPathPoint(Vector2 position)
    {
        pathPoints.Add(position);
        CreateSpriteMarker(position);

        if (controller != null && pathPoints.Count > 0)
        {
            // Если патруль туда-сюда, корректируем поведение
            if (isPingPongPatrol)
            {
                // Если патруль туда-обратно, сбрасываем направление на первую точку
                currentTargetIndex = 0;
                goingForward = true;
            }
            controller.SetTarget(CreateDummyTransform(pathPoints[0]));
        }
    }


    private void RemovePoint(int index)
    {
        if (pathPoints.Count == 0) return;

        pathPoints.RemoveAt(index);

        // Удаляем визуальный маркер
        if (spriteContainer.transform.childCount > index)
            Destroy(spriteContainer.transform.GetChild(index).gameObject);

        if (pathPoints.Count == 0)
        {
            controller.SetTarget(null);
            return;
        }

        // Логика для патруля туда-обратно
        if (isPingPongPatrol)
        {
            if (goingForward && currentTargetIndex >= pathPoints.Count)
            {
                goingForward = false;
                currentTargetIndex = pathPoints.Count - 1;
            }
            else if (!goingForward && currentTargetIndex < 0)
            {
                goingForward = true;
                currentTargetIndex = 0;
            }
            else
            {
                currentTargetIndex += goingForward ? 1 : -1;
            }
        }
        else
        {
            currentTargetIndex = Mathf.Min(currentTargetIndex, pathPoints.Count - 1);
        }

        controller.SetTarget(CreateDummyTransform(pathPoints[currentTargetIndex]));
    }




    private void ClearPath()
    {
        pathPoints.Clear();
        foreach (Transform child in spriteContainer.transform)
            Destroy(child.gameObject);

        controller.SetTarget(null);
    }

    private void CreateSpriteMarker(Vector2 position)
    {
        if (targetSprite == null) return;

        GameObject marker = new GameObject("PathMarker");
        marker.transform.position = position;
        marker.transform.parent = spriteContainer.transform;

        var sr = marker.AddComponent<SpriteRenderer>();
        sr.sprite = targetSprite;
        sr.sortingOrder = 10;
    }

    private Transform CreateDummyTransform(Vector2 pos)
    {
        GameObject dummy = new GameObject("TargetPoint");
        dummy.transform.position = pos;
        Destroy(dummy, 1f); // удаляем после 1 сек (или оставь если хочешь)
        return dummy.transform;
    }

    private void UpdateLineRenderer()
    {
        if (!showPath)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        lineRenderer.positionCount = pathPoints.Count;
        for (int i = 0; i < pathPoints.Count; i++)
        {
            lineRenderer.SetPosition(i, pathPoints[i]);
        }
    }

    public void SetPathVisible(bool visible)
    {
        showPath = visible;
    }
}
