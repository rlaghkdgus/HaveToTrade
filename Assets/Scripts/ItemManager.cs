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
    public List<int> itemCountIndex;
    public List<TMP_Text> productTexts;
    public List<Image> productImages;

    // 계산시 저장공간
    private int totalPrice = 0;
    private List<int> randIndex = new List<int>();

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

    public void RandomSetItem(int sortcount)
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
            randIndex.Add(randnum);
            productIndex.Add(randnum);
        }
        randIndex.Clear();
    }
    public void SetUI()
    {
        for(int i = 0; i <productIndex.Count; i++)
        {
            int randCount = Random.Range(1, 3);
            productImages[i].sprite = playerItems[productIndex[i]].image;
            itemCountIndex.Add(randCount);
            productTexts[i].text = "" + itemCountIndex[i];
        }
    }
    public void BuyProduct()
    {
        for(int i = 0; i < productIndex.Count; i++)
        {
            totalPrice += playerItems[productIndex[i]].price * itemCountIndex[i];
        }
        if(Player.Instance.money < totalPrice)
        {
            //불가능 작성
            return;
        }
        Player.Instance.money -= totalPrice;
        for(int i = 0; i < productIndex.Count; i++)
        {
            playerItems[productIndex[i]].counts += itemCountIndex[i]; 
        }
        ListClear();
    }
    public void ListClear()
    {
        productIndex.Clear();
        itemCountIndex.Clear();
    }

}
