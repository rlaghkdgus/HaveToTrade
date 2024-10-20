using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemManager : Singleton<ItemManager>
{
    [Header("아이템데이터")]
    [SerializeField] ItemSO itemSO;
    [Header("플레이어 아이템")]
    public List<pItem> playerItems;
    [Header("데이터 교환시 사용할 리스트(UI포함)")]
    public List<int> productIndex; // 랜덤인덱스 저장공간
    public List<int> itemCountIndex;// 아이템 개수 저장 공간
    public TMP_Text productTexts; // 상품 개수 텍스트
    public Image productImages; // 상품 이미지 리스트

    // 계산시 저장공간
    
    public int productCount = 0;//UI 순서대로 띄우기용
    private List<int> randIndex = new List<int>();// 다른 종류의 아이템을 뽑기위한 리스트

    private void Start()
    {
        SetupItems();
    }
    void SetupItems()
    {
        playerItems = new List<pItem>();
        // ScriptableObject의 데이터를 바탕으로 Item 데이터 생성
        for (int i = 0; i < itemSO.items.Length; i++)
        {
            // ItemData로부터 새로운 Item을 생성
            pItem newItem = new pItem(itemSO.items[i]);

            // 리스트에 추가
            playerItems.Add(newItem);
        }
    }

    public void RandomSetItem(int sortcount)//Customer스크립트에서 sortCount를받아옴
    {
        for (int i = 0; i < sortcount; i++)
        {
            int randnum;
            // 새로운 랜덤 번호를 찾을 때까지 반복
            do
            {
                randnum = Random.Range(0, playerItems.Count);
            }
            while (randIndex.Contains(randnum)); // 이미 뽑힌 번호라면 다시 뽑기
            randIndex.Add(randnum); // 뽑힌번호 저장
            productIndex.Add(randnum); //상품 추가, 리스트의 번호로 추적할 것(추후 수정가능..아마)
        }
        randIndex.Clear();//뽑힌번호 제거
    }
    public void SetUI()
    {
     int randCount = Random.Range(1, 4);//상품의 종류당 얼마나 거래할건지 랜덤으로 설정
     itemCountIndex.Add(randCount); //몇개 살건지 추가
     productImages.sprite = playerItems[productIndex[productCount]].image; //상품의 이미지, 현재 최대 3개로설정
     productTexts.text = "" + itemCountIndex[productCount];//개수 텍스트에 반영
    }
    public void BuyProduct() //구매시
    {
     
        if(Player.Instance.money < playerItems[productIndex[productCount]].price)
        {
            //불가능 작성예정
            return;
        }
        Player.Instance.money -= playerItems[productIndex[productCount]].price; // 물건 비용 지불
        playerItems[productIndex[productCount]].counts += itemCountIndex[productCount]; // 똑같이 productIndex의 값에서 추적 후 개수 삽입 
        productCount++;
        
    }
    public void ListClear()//리스트 제거용
    {
        productIndex.Clear();
        itemCountIndex.Clear();
        productCount = 0;
    }

}
