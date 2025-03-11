using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SortWeight
{
    public float MaxWeight;
    public float CurrentWeight;
}

[CreateAssetMenu(fileName = "Inventory", menuName = "Scriptable Object/Inventory")]
public class InventoryContainer : ScriptableObject
{
    public Dictionary<ItemSorts, SortWeight> sortWeight = new Dictionary<ItemSorts, SortWeight>()
    {
        {ItemSorts.food, new SortWeight() },
        {ItemSorts.pFood, new SortWeight() },
        {ItemSorts.clothes, new SortWeight() },
        {ItemSorts.furniture, new SortWeight() },
        {ItemSorts.accesory, new SortWeight() }
    };

    public float PublicMaxWeight;

    public float PublicCurrentWeight;

    public List<pItem> inventory;

    public UpgradeInfo UpgradeInfo;

    [SerializeField] private int current_Tier = 0;

    public void InitWeight()
    {
        var tier = UpgradeInfo.InfoList[current_Tier];
        ApplyMaxWeight(tier);
        CalCurrentWeight();
    }

    private void ApplyMaxWeight(TierUp tier)
    {
        sortWeight[ItemSorts.food].MaxWeight = tier.Food;
        sortWeight[ItemSorts.pFood].MaxWeight = tier.pFood;
        sortWeight[ItemSorts.clothes].MaxWeight = tier.Clothes;
        sortWeight[ItemSorts.furniture].MaxWeight = tier.Furniture;
        sortWeight[ItemSorts.accesory].MaxWeight = tier.Accesory;
        PublicMaxWeight = tier.Base;
    }

    private void CalCurrentWeight()
    {
        foreach(var sort in sortWeight.Keys)
        {
            sortWeight[sort].CurrentWeight = 0;
        }
        PublicCurrentWeight = 0;

        foreach (var item in inventory)
        {
            float itemTotalWeight = item.weight * item.counts;
            sortWeight[item.sort].CurrentWeight += itemTotalWeight;

            if (sortWeight[item.sort].CurrentWeight > sortWeight[item.sort].MaxWeight)
            {
                float over = sortWeight[item.sort].CurrentWeight - sortWeight[item.sort].MaxWeight;
                if(over > itemTotalWeight)
                {
                    over = itemTotalWeight;
                }
                PublicCurrentWeight += over;
            }
        }
    }

    public void PrintStatus()
    {
        foreach (var sort in sortWeight)
        {
            Debug.Log($"{sort.Key} - Current Weight: {sort.Value.CurrentWeight} / Max Weight: {sort.Value.MaxWeight}");
        }
        Debug.Log($"Shared Current Weight: {PublicCurrentWeight} / Shared Capacity: {PublicMaxWeight}");
    }

    #region 기능 보관용(쓰려면 public 교체 후 사용)
    private void AddItem(pItem newitem)
    {
        float itemTotalWeight = newitem.weight * newitem.counts;

        float expectedWeight = sortWeight[newitem.sort].CurrentWeight + itemTotalWeight;
        float over = expectedWeight - sortWeight[newitem.sort].MaxWeight;
        if(over > 0)
        {
            if(PublicCurrentWeight + over > PublicMaxWeight)
            {
                Debug.LogWarning("Cannot add item");
                return;
            }
            PublicCurrentWeight += over;
        }

        var invenItem = inventory.Find(i => i.stuffName == newitem.stuffName && i.sort == newitem.sort);

        if(invenItem != null)
        {
            invenItem.counts += newitem.counts;
        }
        else
        {
            inventory.Add(newitem);
        }

        sortWeight[newitem.sort].CurrentWeight += itemTotalWeight;
    }

    private void RemoveItem(pItem itemToRemove)
    {
        var item = inventory.Find(i => i.stuffName == itemToRemove.stuffName && i.sort == itemToRemove.sort);
        if(item != null)
        {
            float itemTotalWeight = itemToRemove.weight * itemToRemove.counts;

            item.counts -= itemToRemove.counts;

            if(item.counts <= 0)
            {
                inventory.Remove(item);
            }

            sortWeight[itemToRemove.sort].CurrentWeight += -itemTotalWeight;

            float over = sortWeight[itemToRemove.sort].CurrentWeight - sortWeight[itemToRemove.sort].MaxWeight;
            if(over < 0)
            {
                float RestoreWeight = itemTotalWeight - Mathf.Abs(over);
                PublicCurrentWeight -= RestoreWeight;
            }
        }
    }

    public void CheckItemWeight(string targetName)
    {
        var item = inventory.Find(i => i.stuffName == targetName);
        if(item != null)
        {
            QuestSystem.Instance.deliveryCheckSign = false;
        }
        float itemTotalWeight = item.weight * item.counts;

        if(itemTotalWeight >= QuestSystem.Instance.questGoal)
            QuestSystem.Instance.deliveryCheckSign = true;
        else
            QuestSystem.Instance.deliveryCheckSign = false;

    }

    public void QuestItemRemove(string targetName)
    {
        var item = inventory.Find(i => i.stuffName == targetName);

        float TargetWeight = 0;

        int requiredItems = Mathf.CeilToInt((QuestSystem.Instance.questGoal - TargetWeight) / item.weight);
        int itemsToRemove = Mathf.Min(requiredItems, item.counts);

        item.counts -= itemsToRemove;

        
        if (item.counts <= 0)
        {
            inventory.Remove(item);
        }
    }
    #endregion
}
