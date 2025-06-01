using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HotbarSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public int slotIndex; // 0–9 (0 соответствует клавише 0, остальные 1–9)
    public Image iconImage;
    public InventoryItem assignedItem;

    // При падении (Drop) из инвентаря сюда
    public void OnDrop(PointerEventData eventData)
    {
        // Предположим, что в eventData.pointerDrag находится UI-элемент ItemIcon из инвентаря, 
        // у которого есть скрипт DraggableItem, который хранит ссылку на InventoryItem.
        var drag = eventData.pointerDrag;
        if (drag == null) return;
        var draggable = drag.GetComponent<DraggableItem>();
        if (draggable == null) return;

        InventoryItem item = draggable.GetInventoryItem();
        AssignItem(item);
    }

    private void AssignItem(InventoryItem item)
    {
        assignedItem = item;
        if (item != null)
        {
            iconImage.sprite = item.data.icon;
            iconImage.color = Color.white;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.color = new Color(0, 0, 0, 0); // прозрачный
        }
    }

    // Дополнительно: при клике на слот (например, хотим использовать предмет)
    public void OnPointerClick(PointerEventData eventData)
    {
        if (assignedItem != null)
        {
            // Используем предмет (вызываем у InventoryItem/ItemData соответствующую логику)
            Debug.Log($"Используем {assignedItem.data.itemName} из хотбара {slotIndex}");
            // Например, если это еда — съесть, если оружие — выстрелить, и т.д.
        }
    }
}
