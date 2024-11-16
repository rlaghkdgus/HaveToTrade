using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ItemManager : Singleton<ItemManager>
{
    [Header("아이템데이터")]
    [SerializeField] ItemSO itemSO;
    [Header("플레이어 데이터 아이템")]
    public List<pItem> playerItems;
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
    private int currentProductIndex = 0;

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
     if (sortcount > playerItems.Count)//만약 받아온 종류가 플레이어 아이템에 있는 개수보다 많을 경우
     sortcount = playerItems.Count;
     for (int i = 0; i < sortcount; i++)
     {
        int randnum;
                // 새로운 랜덤 번호를 찾을 때까지 반복
        do
        {
           randnum = Random.Range(0, playerItems.Count);
        }
        while (randIndex.Contains(randnum) || playerItems[randnum].counts == 0); // 이미 뽑힌 번호 혹은 플레이어가 물건을 가지고 있지 않은 경우 다시뽑기
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
        currentProductIndex = productIndex[productCount];
        int randCount;
        if(productIndex[productCount] >= playerItems.Count)
            productIndex[productCount] = Random.Range(0, playerItems.Count);
        if (playerItems[currentProductIndex].counts > itemCountLimit)
        {
            randCount = Random.Range(1,itemCountLimit+1);
        }
        else
        {
            randCount = Random.Range(1, playerItems[currentProductIndex].counts + 1);
        }
        PutInfo(randCount);
    }
    #endregion


    #region 구매및 판매 실제 과정
    public void BuyProduct() //구매시
    {
        int currentPrice;

        if (bargainSuccess == true)
            currentPrice = bargainPrice * itemCountIndex[productCount];
        else
           currentPrice = itemSO.items[currentProductIndex].price * itemCountIndex[productCount];

        if (Player.Instance.money < currentPrice)
        {
            //불가능 작성예정
            return;
        }
        Player.Instance.money -= currentPrice; // 물건 비용 지불

        pItem newItem = new pItem(itemSO.items[currentProductIndex]);

        // playerItems 리스트에 같은 아이템이 있는지 확인
        pItem existingItem = playerItems.Find(item => item.stuffName == newItem.stuffName);

        if (existingItem != null)
        {
            // 같은 아이템이 이미 있다면 개수만 증가
            existingItem.counts += itemCountIndex[productCount];
        }
        else
        {
            // 새로운 아이템이라면 리스트에 추가
            newItem.counts = itemCountIndex[productCount];
            playerItems.Add(newItem);
        }
        productCount++;
    }
    public void SellProduct()//상품 판매
    {
        int currentPrice = playerItems[currentProductIndex].price * itemCountIndex[productCount];
        Player.Instance.money += currentPrice;
        playerItems[currentProductIndex].counts -= itemCountIndex[productCount];
        if(playerItems[currentProductIndex].counts <= 0)
            playerItems.RemoveAt(currentProductIndex);
        productCount++;
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
            chancePoint = diff / bargainPoint; //차이와 단위를 나눔
            totalChance = initialChance -(chancePoint * bargainPercent); //초기확률에 계산된 확률단위와 흥정단위당%값을뺀 값을 계산, 실제 흥정 확률
            
            if(totalChance >= randomValue)//결과 확률이 random으로 나온 값보다 높으면 성공, ex) 확률이 30이면 random값으로 31이 나왔을때 실패, 랜덤값이 29이면 성공
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
        else//판매일때
        {

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
        Customer.productImages.sprite = itemSO.items[productIndex[productCount]].image; //상품의 이미지
        Customer.productTexts.text = "" + itemCountIndex[productCount];//개수 텍스트에 반영
        Customer.costText.text = "price : " + itemSO.items[productIndex[productCount]].price;//; 가격 텍스트에 반영
    }
    #endregion

}
