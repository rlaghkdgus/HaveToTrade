using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
    
    
    

    // 계산시 저장공간
    public int productCount = 0;//상품 순서 분류
    private List<int> randIndex = new List<int>();// 다른 종류의 아이템을 뽑기위한 리스트
    private int bargainPrice = 0; //흥정한 가격
    public bool bargainSuccess = false;
    public int currentProductIndex = 0;
    public pItem buyItem;
    private static ItemManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
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
            while (randIndex.Contains(randnum) || playerInventory.inventory.Count - 1 < randnum); // 이미 뽑힌 번호 혹은 플레이어가 물건을 가지고 있지 않은 경우 다시뽑기
            randIndex.Add(randnum); // 뽑힌번호 저장
            productIndex.Add(randnum); //상품 추가, 리스트의 번호로 추적할 것(추후 수정가능..아마)
        }
        randIndex.Clear();//뽑힌번호 제거
    }
#endregion
    
    #region 거래 UI세팅
    public void SetUI()//구매시 UI
    {
        currentProductIndex = productIndex[productCount];
        int randCount = Random.Range(1, itemCountLimit+1);//상품의 종류당 얼마나 거래할건지 랜덤으로 설정
        PutInfo(randCount);
    }
    public void SetSellUI()//판매시 UI
    {
        int randCount;
        if(productIndex[productCount] >= playerInventory.inventory.Count)
            productIndex[productCount] = Random.Range(0, playerInventory.inventory.Count);
        currentProductIndex = productIndex[productCount];
        if(playerInventory.inventory[currentProductIndex].counts > itemCountLimit)
        {
            randCount = Random.Range(1,itemCountLimit+1);
        }
        else
        {
            randCount = Random.Range(1, playerInventory.inventory[currentProductIndex].counts + 1);
        }
        PutInfo(randCount);
    }
    #endregion
    #region 구매및 판매 실제 과정
    public void BuyProduct() //구매시
    {
        int currentPrice;

        if (bargainSuccess == true)
            currentPrice = bargainPrice * itemCountIndex[productCount] * (int)salePoint(buyItem.sort) / 100;
        else
           currentPrice = itemSO.items[currentProductIndex].price * itemCountIndex[productCount] * (int)salePoint(buyItem.sort) / 100;

        if (Player.Instance.money < currentPrice)
        {
            productCount++;
            BargainClear();
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
                productCount++;
                playerInventory.sortWeight[itemSO.items[currentProductIndex].sort].CurrentWeight -= itemTotalWeight;
                Debug.LogWarning("Cannot add item");
                return;
            }
            playerInventory.PublicCurrentWeight += over;
        }
        
        #endregion

        Player.Instance.money -= currentPrice; // 물건 비용 지불


        // playerItems 리스트에 같은 아이템이 있는지 확인
        pItem existingItem = playerInventory.inventory.Find(item => item.stuffName == buyItem.stuffName);

        if (existingItem != null)
        {
            // 같은 아이템이 이미 있다면 개수만 증가
            existingItem.counts += itemCountIndex[productCount];
            inventoryUI.InitUI(true);
        }
        else
        {
            // 새로운 아이템이라면 리스트에 추가
            buyItem.counts = itemCountIndex[productCount];
            playerInventory.inventory.Add(buyItem);
            inventoryUI.GenerateSlot();
            inventoryUI.InitUI(true);
        }
        if(QuestSystem.Instance.currentQuestType == QuestType.Trade)
            QuestSystem.Instance.QuestProgress(buyItem, itemTotalWeight);
        productCount++;
        buyItem = null;
        BargainClear();
    }
    public void SellProduct()//상품 판매
    {
        int currentPrice;
        pItem itemToRemove = playerInventory.inventory[currentProductIndex];
        if (bargainSuccess == true)
            currentPrice = bargainPrice * itemCountIndex[productCount];
        else
            currentPrice = playerInventory.inventory[currentProductIndex].price * itemCountIndex[productCount];
        
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
        if (QuestSystem.Instance.currentQuestType == QuestType.Trade)
            QuestSystem.Instance.QuestProgress(itemToRemove, itemTotalWeight);
        #endregion
        productCount++;
        BargainClear();
    }

    #endregion
    #region 흥정 과정
    public void SetBargainPrice(float initialChance,int bargainValue,int bargainPoint, float bargainPercent)// 초기확률(%),흥정제시가격, 흥정단위, 흥정 단위당 %순서
    {
        float randomValue = Random.Range(0f, 100f);
        int diff; //차이계산
        int chancePoint; // 지정된확률에 얼마를 곱할건지
        float totalChance; // 최종확률
        if (Customer.buyOrSell== true)//구매일때
        {
            diff = itemSO.items[currentProductIndex].price - bargainValue;//받아온 흥정값과 아이템값 차이를 계산 후
        }
        else//판매일때
        {
           if(bargainValue < 0)//물건팔때는 0원으로 제한...
                bargainValue = 0;
            diff = bargainValue - playerInventory.inventory[currentProductIndex].price; // 판매, 흥정가가 더 높아야 하므로 역으로 계산
        }
        chancePoint = diff / bargainPoint; //차이와 단위를 나눔
        totalChance = initialChance - (chancePoint * bargainPercent); //초기확률에 계산된 확률단위와 흥정단위당%값을뺀 값을 계산, 실제 흥정 확률

        if (totalChance >= randomValue)//결과 확률이 random으로 나온 값보다 높으면 성공, ex) 확률이 30이면 random값으로 31이 나왔을때 실패, 랜덤값이 29이면 성공
        {
            bargainPrice = bargainValue;
            Customer.costText.text = "price : " + bargainPrice;
            bargainSuccess = true;
        }
        else
        {
            bargainSuccess = false;
        }
    }
    #endregion
    #region 초기화 과정 및 코드 단축용
    public void ListClear()//리스트 제거용
    {
        productIndex.Clear();
        itemCountIndex.Clear();
        productCount = 0;
    }
    public void BargainClear()//흥정가격 초기화용
    {
        bargainPrice = 0;
        bargainSuccess = false;
    }
    public void PutInfo(int randCount)//SetUI과정에서 중복되는부분 코드줄이기
    {
        itemCountIndex.Add(randCount); //몇개 살건지 추가
        if (Customer.buyOrSell == true)
        {
            buyItem = new pItem (itemSO.items[currentProductIndex]);
            pItem countItem = playerInventory.inventory.Find(item => item.stuffName == buyItem.stuffName); // 사려고하는 아이템 찾기
            if (countItem != null)//만약 있다면
            {
                Customer.playerCountTexts.text = "" + countItem.counts;
            }
            else //없다면
            {
                Customer.playerCountTexts.text = "" + 0; //플레이어가 들고있는 개수텍스트 에 0 삽입
            }
            Customer.productImages.sprite = itemSO.items[currentProductIndex].image; //상품의 이미지
            Customer.costText.text = "price : " + itemSO.items[currentProductIndex].price;//; 가격 텍스트에 반영
        }
        else
        {
            Customer.playerCountTexts.text = "" + playerInventory.inventory[currentProductIndex].counts;
            Customer.productImages.sprite = playerInventory.inventory[currentProductIndex].image;
            Customer.costText.text = "price : " + playerInventory.inventory[currentProductIndex].price;
        }
        CusProductCountSet();
        Customer.productTexts.text = "" + itemCountIndex[productCount];//개수 텍스트에 반영
    }
    #endregion
    private void CusProductCountSet()
    {
        switch (Customer.randcusnum)
        {
            case 1:
                if (Customer.buyOrSell == true && buyItem.sort == ItemSorts.food)
                {
                    itemCountIndex[productCount] = itemCountIndex[productCount] + 5;
                }
                else if (Customer.buyOrSell == false && playerInventory.inventory[currentProductIndex].sort == ItemSorts.food)//순서문제. 어짜피 buyorsell이 판매로 지정된이상 playerItem에는 무조건 아이템이 하나는 있으므로.
                {
                    itemCountIndex[productCount] = itemCountIndex[productCount] + 5;
                    if (itemCountIndex[productCount] >= playerInventory.inventory[currentProductIndex].counts)
                        itemCountIndex[productCount] = playerInventory.inventory[currentProductIndex].counts;
                }
                break;
        }
    }
    private float salePoint(ItemSorts sorts)
    {
        float salePercent = 100.0f;
      
        switch(sorts)
        {
            case ItemSorts.food:
                salePercent -=Player.Instance.foodFame.sale;
                break;
            case ItemSorts.pFood:
                salePercent -= Player.Instance.pFoodFame.sale;
                break;
            case ItemSorts.accesory:
                salePercent -= Player.Instance.accesoryFame.sale;
                break;
            case ItemSorts.clothes:
                salePercent -= Player.Instance.clothFame.sale;
                break;
            case ItemSorts.furniture:
                salePercent -= Player.Instance.furnFame.sale;
                break;
        }    
        return salePercent;
    }
}
