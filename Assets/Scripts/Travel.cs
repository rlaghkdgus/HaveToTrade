using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Travel : MonoBehaviour
{
    [SerializeField] private float interval; // 배경 재생성 간격
    [SerializeField] private float speed_F; // 맨 앞 레이어 속도
    [SerializeField] private float speed_M; // 중간 레이어 속도
    [SerializeField] private float speed_B; // 맨 뒤 레이어 속도

    [SerializeField] private int NextIndex = 0; // 길 생성을 멈추고 다음 마을을 생성할 타이밍을 재는 변수

    [SerializeField] private List<GameObject> ForwardList = new List<GameObject>(); // 맨 앞 레이어 리스트
    [SerializeField] private List<GameObject> MiddleList = new List<GameObject>(); // 중간 레이어 리스트
    [SerializeField] private List<GameObject> BackList = new List<GameObject>(); // 맨 뒤 레이어 리스트
    private int index_F = 1; // 재생성 위치 지정을 위한 index
    private int index_M = 1;
    private int index_B = 1;

    public GameObject Map; // 맵 프리팹
    public GameObject[] Road; // 길 프리팹 배열

    [SerializeField] private bool OnMove = false;
    [SerializeField] private GameObject curTownClone;
    [SerializeField] private GameObject nextTown;
    public GameObject nextTownClone;

    public event Action MoveCompleted;

    public void MapOpen() // 맵 여는 기능
    {
        // 맵 프리팹 동적생성
        var map = Instantiate<GameObject>(Map, new Vector3(0, 0, 0), Quaternion.identity);
        map.transform.SetParent(this.transform);
    }

    public void CombinationRoad()
    {
        // 길 배경 조합 랜덤 선택
    }

    private void CopyRoad(GameObject clone, int type) // forward = 0, middle = 1, back = 2
    {
        // 길 동적 생성
        var curRoad = Instantiate<GameObject>(clone, new Vector3(0, 0, 0), Quaternion.identity);
        var nextRoad = Instantiate<GameObject>(clone, new Vector3(0, 0, 0), Quaternion.identity);
        // 레이어 위치에 따라 구분
        switch (type)
        {
            case 0:
                ForwardList.Add(curRoad);
                ForwardList.Add(nextRoad);
                break;
            case 1:
                MiddleList.Add(curRoad);
                MiddleList.Add(nextRoad);
                break;
            case 2:
                BackList.Add(curRoad);
                BackList.Add(nextRoad);
                break;
        }
        SortBG(type);
    }

    public void LoadRoad(GameObject curTown, GameObject nextTown)
    {
        CopyRoad(Road[2], 2);
        CopyRoad(Road[1], 1);
        CopyRoad(Road[0], 0);
        curTownClone = curTown;
        this.nextTown = nextTown;
        OnMove = true;
    }

    private void InitRoad()
    {
        ForwardList.Clear();
        MiddleList.Clear();
        BackList.Clear();

        GameObject[] RoadObj = GameObject.FindGameObjectsWithTag("Road");
        foreach(GameObject obj in RoadObj)
        {
            Destroy(obj);
        }
        NextIndex = 0;
        curTownClone = null;
        nextTown = null;
        nextTownClone = null;
    }

    private void SortBG(int type)
    {
        List<GameObject> RoadList = null;
        switch (type)
        {
            case 0:
                RoadList = ForwardList;
                break;
            case 1:
                RoadList = MiddleList;
                break;
            case 2:
                RoadList = BackList;
                break;
        }

        for (int i = 0; i < RoadList.Count; i++)
        {
            var curBG = RoadList[i];
            curBG.transform.localPosition = Vector3.right * interval * i;
        }
    }

    private void Update()
    {
        if(OnMove)
            MoveBackGround();
    }

    private void MoveBackGround()
    {
        if (curTownClone != null)
        {
            curTownClone.transform.localPosition += Vector3.left * Time.deltaTime * speed_F;
            if (curTownClone.transform.localPosition.x <= -interval)
            {
                // 범위 벗어날 시 파괴
                Destroy(curTownClone);
            }
        }

        for (int i = 0; i < ForwardList.Count; i++)
        {
            ForwardList[i].transform.localPosition += Vector3.left * Time.deltaTime * speed_F;
            if(NextIndex < 2)
            {
                if (ForwardList[i].transform.localPosition.x <= -interval)
                {
                    ForwardList[i].transform.localPosition = new Vector3(ForwardList[index_F].transform.localPosition.x + interval, 0f, 0f);
                    index_F = (index_F + 1) % ForwardList.Count;
                    NextIndex++;
                }
            }
            else if(nextTownClone == null)
            {
                nextTownClone = Instantiate<GameObject>(nextTown, new Vector3(ForwardList[index_F].transform.localPosition.x + interval, 0f, 0f), Quaternion.identity);
            }
        }

        for (int i = 0; i < MiddleList.Count; i++)
        {
            MiddleList[i].transform.localPosition += Vector3.left * Time.deltaTime * speed_M;
            if (MiddleList[i].transform.localPosition.x <= -interval)
            {
                MiddleList[i].transform.localPosition = new Vector3(MiddleList[index_M].transform.localPosition.x + interval, 0f, 0f);
                index_M = (index_M + 1) % MiddleList.Count;
            }
        }

        for (int i = 0; i < BackList.Count; i++)
        {
            BackList[i].transform.localPosition += Vector3.left * Time.deltaTime * speed_B;
            if (BackList[i].transform.localPosition.x <= -interval)
            {
                BackList[i].transform.localPosition = new Vector3(BackList[index_B].transform.localPosition.x + interval, 0f, 0f);
                index_B = (index_B + 1) % BackList.Count;
            }
        }

        if (nextTownClone != null)
        {
            nextTownClone.transform.localPosition += Vector3.left * Time.deltaTime * speed_F;
            if (nextTownClone.transform.localPosition.x <= 0)
            {
                OnMove = false;
                InitRoad();

                MoveCompleted?.Invoke();
            }
        }
    }
}
