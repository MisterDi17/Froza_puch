using UnityEngine;

public class NPSTargetSetter : MonoBehaviour
{
    [SerializeField] private ControlNPS controller;
    [SerializeField] private Transform targetPrefab; // опционально — визуальная метка точки

    private Transform currentTargetInstance;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) // ПКМ
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SetTargetPosition(mousePos);
        }
    }

    public void SetTargetPosition(Vector2 position)
    {
        if (controller == null) return;

        // Создаем или перемещаем метку
        if (targetPrefab != null)
        {
            if (currentTargetInstance == null)
                currentTargetInstance = Instantiate(targetPrefab, position, Quaternion.identity);
            else
                currentTargetInstance.position = position;

            controller.SetTarget(currentTargetInstance);
        }
        else
        {
            // Если метка не задана — создаем временный объект
            GameObject temp = new GameObject("NPS_Target");
            temp.transform.position = position;
            controller.SetTarget(temp.transform);
        }
    }
}
