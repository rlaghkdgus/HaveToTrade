using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TierUp
{
    public int Base;
    public int Food;
    public int pFood;
    public int Clothes;
    public int Furniture;
    public int Accesory;
}

[CreateAssetMenu(fileName = "UpgradeInfo", menuName = "Scriptable Object/UpgradeInfo")]
public class UpgradeInfo : ScriptableObject
{
    public List<TierUp> InfoList;
}
