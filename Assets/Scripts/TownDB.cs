using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTownDB", menuName = "DataBase/TownDB")]
public class TownDB : ScriptableObject
{
    public string TownName; // 마을 이름
    public enum Region { Snowy, Forest, Desert} // 지역 구분
    public Region TownRegion; // 마을의 지역 구분 정보

    public GameObject TownPrefab; // 마을 프리팹
}
