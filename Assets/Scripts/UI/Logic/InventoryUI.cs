using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsContainer; // куда класть иконки
    public GameObject itemIconPrefab;

    private Inventory playerInventory;

    private void Start()
    {
        playerInventory = FindFirstObjectByType<Inventory>();
        Refresh();
    }

    public void Refresh()
    {
        // Удаляем всё
        foreach (Transform child in itemsContainer)
            Destroy(child.gameObject);

        // Добавляем заново все предметы
        foreach (var invItem in playerInventory.GetAllItems())
        {
            GameObject go = Instantiate(itemIconPrefab, itemsContainer);
            ItemIcon icon = go.GetComponent<ItemIcon>();
            icon.Setup(invItem.data.icon, invItem.quantity);
            DraggableItem drag = go.AddComponent<DraggableItem>();
            drag.Setup(invItem);
        }
    }
}
