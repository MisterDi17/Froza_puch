[System.Serializable]
public class InventoryItem
{
    public ItemData data;
    public int quantity;
    public bool isLooted; // Добавлено

    public InventoryItem(ItemData data, int qty, bool isLooted = false)
    {
        this.data = data;
        this.quantity = qty;
        this.isLooted = isLooted;
    }
}
