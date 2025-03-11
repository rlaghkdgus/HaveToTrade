using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownManager : MonoBehaviour
{
    private static TownManager instance;

    [SerializeField] public TownDB curTown; // 현재 마을 정보
    [SerializeField] private TownDB nextTown; // 다음 마을 정보

    public GameObject TownClone; // 현재 마을
    public GameObject ButtonGroup;
    private Travel travel;
    private TownViewChanger changer;

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
        changer = GetComponent<TownViewChanger>();
        TownGenerate();
        changer.ButtonUIUpdate(curTown.TownPrefabs);
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

    private void OnEnable() // Town 버튼 이벤트 구독
    {
        Town.OnTownSelected += HandleTownSelected;
    }

    private void OnDisable() // 해제
    {
        Town.OnTownSelected -= HandleTownSelected;
    }

    private void HandleTownSelected(TownDB town) // 맵에서 마을 버튼을 누르면 이동을 실행
    {
        if(curTown != town)
        {
            // 다음 마을 정보를 Town에서 받아옴
            nextTown = town;
            // Map 제거
            var Map = GameObject.FindWithTag("Map");
            Destroy(Map);
            // 길 생성 실행
            travel.LoadRoad(TownClone, nextTown.TownPrefabs[0], nextTown);
        }
        else
        {
            Debug.Log("현재 마을과 동일한 목적지");
        }
    }

    public void TownGenerate()
    {
        // 현재 마을 동적 생성
        TownClone = Instantiate<GameObject>(curTown.TownPrefabs[changer.currentIndex], new Vector3(0, 0, 0), Quaternion.identity);
        Debug.Log("타운 생성 완료");
    }

    public void UpdateTown()
    {
        curTown = nextTown;
        nextTown = null;
        TownClone = GameObject.FindGameObjectWithTag("Town");
        var MapButton = GameObject.FindWithTag("Canvas").transform.Find("ButtonGroup").transform.Find("OpenMap");
        MapButton.gameObject.SetActive(true);
        Debug.Log("업데이트 완료");
        changer.currentIndex = 0;
        changer.ButtonUIUpdate(curTown.TownPrefabs);
        ButtonGroup.SetActive(true);
    }
    
    public void ViewChangeButtonAction(int index)
    {
        changer.ViewChange(index, curTown.TownPrefabs);
    }
}
