using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIClickOutsideHandler : MonoBehaviour
{
    public GameObject contentPanel; // Твоя ContentPanel
    public GameObject uiCreater;    // Объект UICreater
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUI(uiCreater))
            {
                StartCoroutine(CloseContentPanelWithFocusFix());
            }
            else
            {
                contentPanel.SetActive(true);
            }
        }
    }

    IEnumerator CloseContentPanelWithFocusFix()
    {
        // подождать конец кадра, чтобы дать системе завершить текущее событие
        yield return new WaitForEndOfFrame();

        // сбрасываем выбранный UI объект
        EventSystem.current.SetSelectedGameObject(null);

        // деактивируем все поля ввода
        foreach (var input in contentPanel.GetComponentsInChildren<InputField>())
            input.DeactivateInputField();

        // и только потом скрываем
        contentPanel.SetActive(false);
    }


    bool IsPointerOverUI(GameObject targetUI)
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Input.mousePosition;

        var results = new System.Collections.Generic.List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == targetUI || result.gameObject.transform.IsChildOf(targetUI.transform))
                return true;
        }

        return false;
    }
}
