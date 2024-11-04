using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownManager : MonoBehaviour
{
    [SerializeField] private TownDB curTown; // ���� ���� ����
    [SerializeField] private TownDB nextTown; // ���� ���� ����

    [SerializeField] private GameObject TownClone; // ���� ����

    private Travel travel;

    private void Awake()
    {
        travel = GetComponent<Travel>();
        // ���� ���� �� ���� ���� ���� ����
        TownClone = Instantiate<GameObject>(curTown.TownPrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    private void OnEnable() // Town ��ư �̺�Ʈ ����
    {
        travel.MoveCompleted += UpdateTown;
        Town.OnTownSelected += HandleTownSelected;
    }

    private void OnDisable() // ����
    {
        travel.MoveCompleted -= UpdateTown;
        Town.OnTownSelected -= HandleTownSelected;
    }

    private void HandleTownSelected(TownDB town) // �ʿ��� ���� ��ư�� ������ �̵��� ����
    {
        if(curTown != town)
        {
            // ���� ���� ������ Town���� �޾ƿ�
            nextTown = town;
            // Map ����
            var Map = GameObject.FindWithTag("Map");
            Destroy(Map);
            // �� ���� ����
            travel.LoadRoad(TownClone, nextTown.TownPrefab);
        }
        else
        {
            Debug.Log("���� ������ ������ ������");
        }
    }

    private void UpdateTown()
    {
        curTown = nextTown;
        nextTown = null;
        TownClone = GameObject.FindGameObjectWithTag("Town");
        var MapButton = GameObject.FindWithTag("Canvas").transform.Find("OpenMap");
        MapButton.gameObject.SetActive(true);
        Debug.Log("������Ʈ �Ϸ�");
    }
}
