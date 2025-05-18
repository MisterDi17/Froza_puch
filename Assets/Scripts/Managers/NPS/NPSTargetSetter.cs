using UnityEngine;

public class NPSTargetSetter : MonoBehaviour
{
    [SerializeField] private ControlNPS controller;
    [SerializeField] private Sprite targetSprite;

    private SpriteRenderer _targetVisual;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SetTarget(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    public void SetTarget(Vector2 position)
    {
        UpdateTargetVisual(position);
        controller.SetTargetPosition(position);
    }

    private void UpdateTargetVisual(Vector2 position)
    {
        if (targetSprite == null) return;

        if (_targetVisual == null)
        {
            GameObject visual = new GameObject("TargetVisual");
            visual.transform.position = position;
            _targetVisual = visual.AddComponent<SpriteRenderer>();
            _targetVisual.sprite = targetSprite;
            _targetVisual.color = Color.red;
        }
        else
        {
            _targetVisual.transform.position = position;
        }
    }
}