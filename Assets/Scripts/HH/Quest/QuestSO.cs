using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestInfo
{
    public int id;
    public string questName;
    public ItemSorts sort;
    public string questStuffName;
    public int questInfo;
    public int reward;
    public string questText;
    public bool buyOrSell;
    public VillageType villageType;
    public QuestType questType;
}