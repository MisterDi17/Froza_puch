using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;             // Название предмета
    public Sprite icon;                 // Иконка
    public int width = 1;               // Ширина в слотах
    public int height = 1;              // Высота в слотах
    public float weight = 1f;           // Вес одного предмета
    public int maxStack = 1;            // Максимальное количество в стаке (1 — значит уникальный)
    public ItemRarity rarity;           // Редкость
    public List<string> tags;           // Теги: "weapon", "food", "valuable" и т.п.

    [TextArea]
    public string description;          // Описание
    public bool isStackable => maxStack > 1;
    public int maxStackSize => maxStack;
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
