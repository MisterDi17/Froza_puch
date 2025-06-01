using System.Collections.Generic;
using UnityEngine;

public class ContainerInteractable : MonoBehaviour
{
    public Inventory containerInventory;
    public bool isOpened = false;
    public bool[] itemsSearched; // чтобы помнить, что обыскано
    public List<InventoryItem> ContainerItems => containerInventory != null ? containerInventory.items : new List<InventoryItem>();


    private void Awake()
    {
        if (containerInventory != null)
            itemsSearched = new bool[containerInventory.items.Count];
    }
}
