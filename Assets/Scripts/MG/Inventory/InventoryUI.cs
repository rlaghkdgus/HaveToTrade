using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private List<ItemSlotUI> S_uiSlots;
    [SerializeField] private List<ItemSlotUI> B_uiSlots;
    [SerializeField] private GameObject S_SlotPrefab;
    [SerializeField] private GameObject B_SlotPrefab;

    [SerializeField] private Transform S_Inven;
    [SerializeField] private Transform B_Inven;

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).CompareTag("S_Inven"))
            {
                S_Inven = transform.GetChild(i);
            }
            else if (transform.GetChild(i).CompareTag("B_Inven"))
            {
                B_Inven = transform.GetChild(i);
            }
        }

        for(int i = 0; i < ItemManager.Instance.playerInventory.inventory.Count; i++)
        {
            GenerateSlot();
        }

        InitUI(true);
    }

    public void InitUI(bool isBuy)
    {
        if (isBuy)
        {
            SetAllIndex();
            Show();
        }
        else
        {
            Show();
            SetAllIndex();
        }
    }

    public void GenerateSlot()
    {
        GameObject S_SlotCopy = Instantiate(S_SlotPrefab, Vector2.zero, Quaternion.identity);
        S_SlotCopy.transform.SetParent(S_Inven.GetChild(0).GetChild(0));
        S_uiSlots.Add(S_SlotCopy.GetComponent<ItemSlotUI>());

        GameObject B_SlotCopy = Instantiate(B_SlotPrefab, Vector2.zero, Quaternion.identity);
        B_SlotCopy.transform.SetParent(B_Inven.GetChild(0).GetChild(0));
        B_uiSlots.Add(B_SlotCopy.GetComponent<ItemSlotUI>());
    }

    public void DeleteSlot(int index)
    {
        Destroy(S_uiSlots[index].gameObject);
        S_uiSlots.RemoveAt(index);
        Destroy(B_uiSlots[index].gameObject);
        B_uiSlots.RemoveAt(index);
    }

    private void SetAllIndex()
    {
        if(S_uiSlots.Count != 0)
        {
            for (int i = 0; i < S_uiSlots.Count; i++)
            {
                S_uiSlots[i].SetIndex(i);
                B_uiSlots[i].SetIndex(i);
            }
        }
    }

    private void Show()
    {
        if(S_uiSlots.Count != 0)
        {
            for (int i = 0; i < S_uiSlots.Count; i++)
            {
                S_uiSlots[i].Set(ItemManager.Instance.playerInventory.inventory[i]);
                B_uiSlots[i].Set(ItemManager.Instance.playerInventory.inventory[i]);
            }
        }
    }
}
