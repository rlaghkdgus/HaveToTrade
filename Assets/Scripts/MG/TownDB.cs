using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTownDB", menuName = "DataBase/TownDB")]
public class TownDB : ScriptableObject
{
    public string TownName; // 마을 이름
    public VillageType TownType;

    public List<GameObject> TownPrefabs; // 마을 프리팹

    public List<GameObject> RoadPrefabs_F; // 앞
    public List<GameObject> RoadPrefabs_M; // 중간
    public List<GameObject> RoadPrefabs_B; // 뒤
}
