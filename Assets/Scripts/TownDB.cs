using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTownDB", menuName = "DataBase/TownDB")]
public class TownDB : ScriptableObject
{
    public string TownName; // ���� �̸�
    public enum Region { Snowy, Forest, Desert} // ���� ����
    public Region TownRegion; // ������ ���� ���� ����

    public GameObject TownPrefab; // ���� ������
}
