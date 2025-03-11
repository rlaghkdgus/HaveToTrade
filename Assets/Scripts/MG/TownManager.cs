using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownManager : MonoBehaviour
{
    private static TownManager instance;

    [SerializeField] public TownDB curTown; // ���� ���� ����
    [SerializeField] private TownDB nextTown; // ���� ���� ����

    [SerializeField] private GameObject TownClone; // ���� ����
    public GameObject ButtonGroup;
    private Travel travel;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        travel = GetComponent<Travel>();
        TownGenerate();
    }

    public static TownManager Instance
    {
        get
        {
            if(instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    private void OnEnable() // Town ��ư �̺�Ʈ ����
    {
        Town.OnTownSelected += HandleTownSelected;
    }

    private void OnDisable() // ����
    {
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

    public void TownGenerate()
    {
        // ���� ���� ���� ����
        TownClone = Instantiate<GameObject>(curTown.TownPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        Debug.Log("Ÿ�� ���� �Ϸ�");
    }

    public void UpdateTown()
    {
        curTown = nextTown;
        nextTown = null;
        TownClone = GameObject.FindGameObjectWithTag("Town");
        var MapButton = GameObject.FindWithTag("Canvas").transform.Find("ButtonGroup").transform.Find("OpenMap");
        MapButton.gameObject.SetActive(true);
        Debug.Log("������Ʈ �Ϸ�");
        ButtonGroup.SetActive(true);
        
    }
    
}
