using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HotbarSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public int slotIndex;
    public Image iconImage;
    public InventoryItem assignedItem;

    public void OnDrop(PointerEventData eventData)
    {
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
            iconImage.color = new Color(0, 0, 0, 0);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (assignedItem != null)
        {
            Debug.Log($"Используем {assignedItem.data.itemName} из хотбара {slotIndex}");
        }
    }
}
