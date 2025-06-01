using NUnit.Framework.Interfaces;

[System.Serializable]
public class ContainerItem
{
    public ItemData itemData; // ссылка на ScriptableObject с данными предмета
    public int quantity;      // сколько таких предметов
    public bool isLooted;     // флаг, если уже подобрал игрок (для запоминания)
}
