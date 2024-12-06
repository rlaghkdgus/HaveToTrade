using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditorInternal.Profiling.Memory.Experimental;

public class ItemManager : Singleton<ItemManager>
{
    [Header("아이템데이터")]
    [SerializeField] ItemSO itemSO;
    [Header("플레이어 데이터 아이템")]
    //public List<pItem> playerItems;
    public InventoryContainer playerInventory;
    public InventoryUI inventoryUI;
    [Header("품목별 개수 제한")]
    public int itemCountLimit;// 품목별 개수 제한
    [Header("데이터 교환시 사용할 리스트(UI포함)")]
    public List<int> productIndex; // 랜덤인덱스 저장공간
    public List<int> itemCountIndex;// 아이템 개수 저장 공간
    public TMP_Text productTexts; // 상품 개수 텍스트
    public Image productImages; // 상품 이미지 리스트
    

    // 계산시 저장공간
    public int productCount = 0;//상품 순서 분류
    private List<int> randIndex = new List<int>();// 다른 종류의 아이템을 뽑기위한 리스트

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        playerInventory.InitWeight();
    }
    #region 거래전 아이템 세팅
    public void RandomSetItem(int sortcount)//Customer스크립트에서 sortCount를받아옴
    {
        for (int i = 0; i < sortcount; i++)
        {
            int randnum;
            // 새로운 랜덤 번호를 찾을 때까지 반복
            do
            {
                randnum = Random.Range(0, itemSO.items.Length);
            }
            while (randIndex.Contains(randnum)); // 이미 뽑힌 번호라면 다시 뽑기
            randIndex.Add(randnum); // 뽑힌번호 저장
            productIndex.Add(randnum); //상품 추가, 리스트의 번호로 추적할 것(추후 수정가능..아마)
        }
        randIndex.Clear();//뽑힌번호 제거
    }
    public void RandomSetItemSell(int sortcount)//판매시 랜덤 아이템 설정
    {
        if(sortcount > playerInventory.inventory.Count)//만약 받아온 종류가 플레이어 아이템에 있는 개수보다 많을 경우
            sortcount = playerInventory.inventory.Count;
        for(int i = 0; i< sortcount; i++)
        {
            int randnum;
            // 새로운 랜덤 번호를 찾을 때까지 반복
            do
            {
                randnum = Random.Range(0, playerInventory.inventory.Count);
            }
            while (randIndex.Contains(randnum) || playerInventory.inventory[randnum].counts == 0); // 이미 뽑힌 번호 혹은 플레이어가 물건을 가지고 있지 않은 경우 다시뽑기
            randIndex.Add(randnum); // 뽑힌번호 저장
            productIndex.Add(randnum); //상품 추가, 리스트의 번호로 추적할 것(추후 수정가능..아마)
        }
        randIndex.Clear();//뽑힌번호 제거
    }
    #endregion

    #region 거래 UI세팅
    public void SetUI()//구매시 UI
    {
     int randCount = Random.Range(1, itemCountLimit+1);//상품의 종류당 얼마나 거래할건지 랜덤으로 설정
     itemCountIndex.Add(randCount); //몇개 살건지 추가
     productImages.sprite = itemSO.items[productIndex[productCount]].image; //상품의 이미지
     productTexts.text = "" + itemCountIndex[productCount];//개수 텍스트에 반영
    }
    public void SetSellUI()//판매시 UI
    {
        int randCount;
        if(productIndex[productCount] >= playerInventory.inventory.Count)
            productIndex[productCount] = Random.Range(0, playerInventory.inventory.Count);
        int currentProductIndex = productIndex[productCount];
        if(playerInventory.inventory[currentProductIndex].counts > itemCountLimit)
        {
            randCount = Random.Range(1,itemCountLimit+1);
        }
        else
        {
            randCount = Random.Range(1, playerInventory.inventory[currentProductIndex].counts + 1);
        }
        itemCountIndex.Add(randCount); //몇개 팔건지 추가
        productImages.sprite = playerInventory.inventory[currentProductIndex].image;
        productTexts.text = "" + itemCountIndex[productCount];//개수 텍스트에 반영
    }
    #endregion


    #region 구매및 판매 실제 과정
    public void BuyProduct() //구매시
    {
        int currentProductIndex = productIndex[productCount];
        int currentPrice = itemSO.items[currentProductIndex].price * itemCountIndex[productCount];
        if (Player.Instance.money < currentPrice)
        {
            //불가능 작성예정
            return;
        }

        #region 무게 계산 부분
        float itemTotalWeight = itemSO.items[currentProductIndex].weight * itemCountIndex[productCount];

        playerInventory.sortWeight[itemSO.items[currentProductIndex].sort].CurrentWeight += itemTotalWeight;
        if(playerInventory.sortWeight[itemSO.items[currentProductIndex].sort].CurrentWeight > playerInventory.sortWeight[itemSO.items[currentProductIndex].sort].MaxWeight)
        {
            float over = playerInventory.sortWeight[itemSO.items[currentProductIndex].sort].CurrentWeight - playerInventory.sortWeight[itemSO.items[currentProductIndex].sort].MaxWeight;
            over = Mathf.Min(over, itemTotalWeight);

            if (playerInventory.PublicCurrentWeight + over > playerInventory.PublicMaxWeight)
            {
                playerInventory.sortWeight[itemSO.items[currentProductIndex].sort].CurrentWeight -= itemTotalWeight;
                Debug.LogWarning("Cannot add item");
                return;
            }
            playerInventory.PublicCurrentWeight += over;
        }
        #endregion

        Player.Instance.money -= currentPrice; // 물건 비용 지불

        pItem newItem = new pItem(itemSO.items[currentProductIndex]);

        // playerItems 리스트에 같은 아이템이 있는지 확인
        pItem existingItem = playerInventory.inventory.Find(item => item.stuffName == newItem.stuffName);

        if (existingItem != null)
        {
            // 같은 아이템이 이미 있다면 개수만 증가
            existingItem.counts += itemCountIndex[productCount];
            inventoryUI.InitUI(true);
        }
        else
        {
            // 새로운 아이템이라면 리스트에 추가
            newItem.counts = itemCountIndex[productCount];
            playerInventory.inventory.Add(newItem);
            inventoryUI.GenerateSlot();
            inventoryUI.InitUI(true);
        }
        productCount++;
    }
    public void SellProduct()//상품 판매
    {
        int currentProductIndex = productIndex[productCount];
        pItem itemToRemove = playerInventory.inventory[currentProductIndex];
        int currentPrice = itemToRemove.price * itemCountIndex[productCount];

        Player.Instance.money += currentPrice;
        playerInventory.inventory[currentProductIndex].counts -= itemCountIndex[productCount];
        if (playerInventory.inventory[currentProductIndex].counts <= 0)
        {
            playerInventory.inventory.RemoveAt(currentProductIndex);
            inventoryUI.DeleteSlot(currentProductIndex);
        }

        inventoryUI.InitUI(false);

        #region 무게 계산 부분
        float itemTotalWeight = itemToRemove.weight * itemCountIndex[productCount];
        if (playerInventory.sortWeight[itemToRemove.sort].CurrentWeight <= playerInventory.sortWeight[itemToRemove.sort].MaxWeight)
        {
            playerInventory.sortWeight[itemToRemove.sort].CurrentWeight -= itemTotalWeight;
        }
        else
        {
            if(playerInventory.sortWeight[itemToRemove.sort].CurrentWeight - itemTotalWeight >= playerInventory.sortWeight[itemToRemove.sort].MaxWeight)
            {
                playerInventory.sortWeight[itemToRemove.sort].CurrentWeight -= itemTotalWeight;
                playerInventory.PublicCurrentWeight -= itemTotalWeight;
            }
            else
            {
                playerInventory.sortWeight[itemToRemove.sort].CurrentWeight -= itemTotalWeight;
                float margin = playerInventory.sortWeight[itemToRemove.sort].MaxWeight - playerInventory.sortWeight[itemToRemove.sort].CurrentWeight;
                float over = itemTotalWeight - margin;
                playerInventory.PublicCurrentWeight -= over;
            }
        }

        #endregion
        productCount++;
    }

    #endregion
    public void ListClear()//리스트 제거용
    {
        productIndex.Clear();
        itemCountIndex.Clear();
        productCount = 0;
    }

}
