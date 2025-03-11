using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTownDB", menuName = "DataBase/TownDB")]
public class TownDB : ScriptableObject
{
    public string TownName; // 마을 이름
    public VillageType TownType;

    public GameObject TownPrefab; // 마을 프리팹
}
