using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI price;
    [SerializeField] private TextMeshProUGUI Weight;

    [SerializeField] private int myIndex;

    public void SetIndex(int index)
    {
        myIndex = index;
    }

    public void Set(pItem slotitem)
    {
        icon.sprite = slotitem.image;

        if (price != null && Weight != null)
        {
            price.text = "Price : " + slotitem.price.ToString();
            Weight.text = "Weight : " + (slotitem.counts * slotitem.weight).ToString();
        }
    }
}
