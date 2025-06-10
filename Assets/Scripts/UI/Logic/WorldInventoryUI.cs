using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldInventoryUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel; // Весь UI-контейнер (Canvas Panel), по умолчанию скрыт
    public Transform itemsContainer; // Контейнер для отображения префабов ItemIcon
    public Button btnTakeEverything;
    public Button btnCollectStacks;
    public Button btnSort;
    public Button btnFilter;

    private ContainerInteractable currentContainer;
    private List<InventoryItem> displayedItems = new List<InventoryItem>();

    private void Start()
    {
        panel.SetActive(false);

        btnTakeEverything.onClick.AddListener(OnTakeEverything);
        btnCollectStacks.onClick.AddListener(OnCollectStacks);
        btnSort.onClick.AddListener(OnSort);
        btnFilter.onClick.AddListener(OnFilter);
    }
    public void OpenContainer(ContainerInteractable container)
    {
        currentContainer = container;
        panel.SetActive(true);
        RefreshUI();
    }
    public void CloseContainer()
    {
        panel.SetActive(false);
        ClearUI();
        currentContainer = null;
    }
    public void AddItemToContainer(InventoryItem ci)
    {
        if (currentContainer == null) return;
        if (ci.isLooted)
        {
            displayedItems.Add(ci);
            CreateItemIcon(ci);
        }
    }
    private void RefreshUI()
    {
        // Очищаем всё, потом заново перебираем container.containerItems и добавляем только те, что isLooted == true
        ClearUI();
        displayedItems.Clear();

        foreach (var ci in currentContainer.ContainerItems)
        {
            if (ci.isLooted)
            {
                displayedItems.Add(ci);
                CreateItemIcon(ci);
            }
        }
    }
    private void ClearUI()
    {
        foreach (Transform child in itemsContainer)
            Destroy(child.gameObject);
    }
    private void CreateItemIcon(InventoryItem ci)
    {
        GameObject iconGO = Instantiate(Resources.Load<GameObject>("ItemIcon"), itemsContainer);
        ItemIcon icon = iconGO.GetComponent<ItemIcon>();
        icon.Setup(ci.data.icon, ci.quantity);
    }
    private void OnTakeEverything()
    {
        // Проходим по displayedItems (которые уже isLooted), и пытаемся добавить их в инвентарь игрока.
        Inventory playerInv = Object.FindFirstObjectByType<Inventory>();
        foreach (var ci in new List<InventoryItem>(displayedItems))
        {
            if (playerInv.CanAdd(ci.data, ci.quantity))
            {
                playerInv.AddItem(ci.data, ci.quantity);
                // Убираем предмет из контейнера (или помечаем как обыскано и забранно)
                currentContainer.ContainerItems.Remove(ci);
                displayedItems.Remove(ci);
                // Удаляем иконку из UI
                // ... (например, ci сам знает свой ItemIcon, или ищем по имени)
            }
            else
            {
                // Если не вместилось — можно показать подсказку «Нет места или перевес» 
                Debug.Log("Не удалось взять всё — нет места или превышен вес");
                break;
            }
        }
        RefreshUI();
    }
    private void OnCollectStacks()
    {
        // Проходим по displayedItems, группируем по itemData, объединяем их, оставляем по одному ContainerItem на каждый тип
        Dictionary<ItemData, InventoryItem> merged = new Dictionary<ItemData, InventoryItem>();
        foreach (var ci in displayedItems)
        {
            if (!merged.ContainsKey(ci.data))
            {
                merged[ci.data] = new InventoryItem(ci.data, ci.quantity, true);
            }
            else
            {
                merged[ci.data].quantity += ci.quantity;
            }
        }
        // Теперь очистим текущий список displayedItems и заполним merged
        displayedItems = new List<InventoryItem>(merged.Values);

        // Заменим данные в currentContainer тоже (если хотим сохранять на будущее)
        currentContainer.ContainerItems.RemoveAll(c => c.isLooted);
        foreach (var kvp in merged)
            currentContainer.ContainerItems.Add(kvp.Value);

        RefreshUI();
    }
    private void OnSort()
    {
        // Например, сортировать по size (Vector2Int) → более крупный первым 
        displayedItems.Sort((a, b) =>
        {
            int sizeA = a.data.height * a.data.width;
            int sizeB = b.data.height * b.data.width;
            return sizeB.CompareTo(sizeA); // больше к мельче
        });
        RefreshUI();
    }
    private void OnFilter()
    {
        // Смоделируем фильтрацию по ценности: группируем теги
        // Например, выводим все виды тегов в Dropdown, но упростим: просто отсортируем по value
        displayedItems.Sort((a, b) => b.data.tags.Count.CompareTo(a.data.tags.Count));
        RefreshUI();
    }
}
