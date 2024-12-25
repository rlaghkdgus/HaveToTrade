using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownManager : MonoBehaviour
{
    private static TownManager instance;

    [SerializeField] private TownDB curTown; // 현재 마을 정보
    [SerializeField] private TownDB nextTown; // 다음 마을 정보

    [SerializeField] private GameObject TownClone; // 현재 마을
    public GameObject GoTradeButton;
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
            travel.LoadRoad(TownClone, nextTown.TownPrefab);
        }
        else
        {
            Debug.Log("현재 마을과 동일한 목적지");
        }
    }

    public void TownGenerate()
    {
        // 현재 마을 동적 생성
        TownClone = Instantiate<GameObject>(curTown.TownPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        Debug.Log("타운 생성 완료");
    }

    public void UpdateTown()
    {
        curTown = nextTown;
        nextTown = null;
        TownClone = GameObject.FindGameObjectWithTag("Town");
        var MapButton = GameObject.FindWithTag("Canvas").transform.Find("OpenMap");
        MapButton.gameObject.SetActive(true);
        Debug.Log("업데이트 완료");
        GoTradeButton.SetActive(true);
        
    }
    
}
