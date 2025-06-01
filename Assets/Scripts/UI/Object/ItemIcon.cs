using UnityEngine;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
    public Image icon;
    public Text amountText;

    public void Setup(Sprite sprite, int amount)
    {
        icon.sprite = sprite;
        amountText.text = amount > 1 ? amount.ToString() : "";
    }
}
