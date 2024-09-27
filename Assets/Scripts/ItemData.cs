using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{  
    public ItemSorts sort;
    public string stuffName;
    public int price;
    public Sprite image;
    public int counts;
}
[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    public Item[] items;
}


