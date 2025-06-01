using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public float maxWeight = 100f;
    public List<InventoryItem> items = new List<InventoryItem>();

    public float CurrentWeight
    {
        get
        {
            float sum = 0f;
            foreach (var it in items)
                sum += it.data.weight * it.quantity;
            return sum;
        }
    }

    public bool CanAdd(ItemData data, int qty)
    {
        float newWeight = data.weight * qty;
        return CurrentWeight + newWeight <= maxWeight;
    }

    public bool AddItem(ItemData data, int qty)
    {
        if (!CanAdd(data, qty)) return false;

        if (data.isStackable)
        {
            // Ищем уже существующий слот
            var existing = items.Find(i => i.data == data);
            if (existing != null)
            {
                int freeSpace = data.maxStackSize - existing.quantity;
                int toAdd = Mathf.Min(freeSpace, qty);
                existing.quantity += toAdd;
                qty -= toAdd;
                if (qty > 0)
                {
                    // Если ещё что-то осталось сверх maxStackSize, создаём новый слот
                    items.Add(new InventoryItem(data, qty));
                }
            }
            else
            {
                int toAdd = Mathf.Min(data.maxStackSize, qty);
                items.Add(new InventoryItem(data, toAdd));
                qty -= toAdd;
                if (qty > 0)
                {
                    items.Add(new InventoryItem(data, qty));
                }
            }
        }
        else
        {
            // Не стакуется, просто добавляем каждый как по одному (qty штук)
            for (int i = 0; i < qty; i++)
            {
                if (!CanAdd(data, 1)) return false;
                items.Add(new InventoryItem(data, 1));
            }
        }
        return true;
    }

    public void RemoveItem(ItemData data, int qty)
    {
        // Удаляем из списка слотов столько предметов, сколько надо
        for (int i = 0; i < qty; i++)
        {
            var slot = items.Find(s => s.data == data);
            if (slot != null)
            {
                slot.quantity--;
                if (slot.quantity <= 0)
                    items.Remove(slot);
            }
            else
                break;
        }
    }

    // Для UI: получить все предметы
    public List<InventoryItem> GetAllItems()
    {
        return items;
    }
}
