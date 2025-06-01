using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HotbarController : MonoBehaviour
{
    public Image[] slotImages;
    public Button[] slotButtons;
    public Image[] slotIcons;
    public HotbarItem[] items;

    public RectTransform hotbarArea; // Сам объект панели
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private int currentSlot = -1;
    private bool hasFocus = false;

    void Start()
    {
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int index = i;
            slotButtons[i].onClick.AddListener(() =>
            {
                SelectSlot(index);
                hasFocus = true;
            });
        }

        LoadIcons();
        UnselectSlot(); // ничего не выбрано по умолчанию
    }

    void Update()
    {
        // Определим клик мышью
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;

            // Если мышь не над UI → сразу сброс фокуса
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                LoseFocus();
            }
            // Если курсор не в зоне hotbar → тоже сброс
            else if (!RectTransformUtility.RectangleContainsScreenPoint(hotbarArea, mousePos))
            {
                LoseFocus();
            }
        }

        // Прокрутка мыши работает ТОЛЬКО если есть фокус
        if (hasFocus && slotImages.Length > 0)
        {
            float scroll = Input.mouseScrollDelta.y;
            if (scroll != 0)
            {
                int direction = scroll > 0 ? -1 : 1;
                int newSlot = (currentSlot + direction + slotImages.Length) % slotImages.Length;
                SelectSlot(newSlot);
            }
        }

        // Клавиши 1–9, 0 — работают всегда
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                SelectSlot(i);
        }

        if (slotImages.Length >= 10 && Input.GetKeyDown(KeyCode.Alpha0))
            SelectSlot(9);
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= slotImages.Length)
            return;

        currentSlot = index;

        for (int i = 0; i < slotImages.Length; i++)
        {
            slotImages[i].color = (i == currentSlot) ? selectedColor : normalColor;
        }

        Debug.Log($"Выбран слот: {index + 1}");
    }

    public void UnselectSlot()
    {
        currentSlot = -1;
        for (int i = 0; i < slotImages.Length; i++)
        {
            slotImages[i].color = normalColor;
        }
    }

    private void LoseFocus()
    {
        hasFocus = false;
        UnselectSlot();
        Debug.Log("Фокус с hotbar снят");
    }

    void LoadIcons()
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (i < items.Length && items[i].icon != null)
            {
                slotIcons[i].sprite = items[i].icon;
                slotIcons[i].enabled = true;
            }
            else
            {
                slotIcons[i].enabled = false;
            }
        }
    }

    public HotbarItem GetSelectedItem()
    {
        if (currentSlot >= 0 && currentSlot < items.Length)
            return items[currentSlot];
        return null;
    }
}
