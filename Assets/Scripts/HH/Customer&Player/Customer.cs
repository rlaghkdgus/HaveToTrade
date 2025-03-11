using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/*
 * 거래 진입 시 고객의 수, 거래 물품의 종류 및 숫자를 정할 공간
 * 손님 행동패턴 요약 : 입장(Start) -> 대기(Idle) -> 아이템매니저에서 가져온 아이템으로 UI세팅-> 흥정(선택)-> 구매 or 판매 -> 퇴장(End) -> 입장, 손님수 만큼 반복
 */
[System.Serializable]
public class customerPrefab
{
    public int customerNum;
    public string customerName;
    public string customerDescription;
    public GameObject[] cusPrefab;
    public float cusBargainChance;
    public int cusProductCount;
}
public class Customer : MonoBehaviour
{
    [Header("손님수, 현재는 시작시 랜덤 지정")]
    public int cusCount;
    [Header("손님 세부 옵션")]
    public float speed; //손님 이동속도
    public float tradeDelay; // 손님 거래 딜레이
    public float rejectDelay; // 손님 거절 후 다음 거래 딜레이
    public float fadeDuration; // 페이드 아웃 지속 시간
    [Header("흥정 세부 옵션")]
    public float initialChance; // 흥정의 초기 확률
    public int bargainPoint; // 흥정확률 단위
    public float bargainChance; // 해당 단위를 넘을때마다 흥정확률이 몇%씩 떨어질지
    [Header("몇개의 종류를 거래할건지")]
    public int tradeSortCount; // 손님이 몇개의 종류를 거래할건지
    [Header("손님 프리팹(손님 별 기능 및 외형),생성 및 퇴장 위치 설정")]
    public List<customerPrefab> cusList;
    public List<Transform> customerTransform;// 생성, 거래위치, 퇴장


    [Header("손님 거래창")]
    public GameObject CustomerUI;
    public GameObject BuyUI;
    public GameObject SellUI;
    public GameObject BargainUI;
    [SerializeField] private TMP_Text cText;
    [SerializeField] private TMP_Text pTxt;
    [SerializeField] private TMP_Text pcTxt; // 플레이어가 들고있는 상품 개수 카운트
    [SerializeField] private TMP_InputField bargainField;
    [SerializeField] private Image pImg;
    [Header("흥정시 On/Off 오브젝트")]
    [SerializeField] private GameObject bargainButton;
    [SerializeField] private GameObject rejectButton;
    [Header("마을 버튼")]
    [SerializeField] GameObject GoTownButton;

    [Header("손님 대화")]
    public TMP_Text talkText; // 대화텍스트
    [SerializeField] Transform textTransform;//대화생성위치
    [SerializeField] float typingDelay;//타이핑 간격
    [Header("거래 후 버튼 관리")]
    [SerializeField] GameObject buttonEdit;
    //static,private 옵저버패턴 변수 등 인스펙터에 안보이는 요소들
    public Data<CustomerState> cState = new Data<CustomerState>();//상태별 이벤트
    private GameObject newCustomer;
    private int bargainValue;
    public static int randcusnum = 0;
    public static TMP_Text productTexts;
    public static Image productImages;
    public static TMP_Text playerCountTexts;// 플레이어가 들고있는 상품 개수 카운트
    public static TMP_Text costText;//가격 텍스트, product랑 price의 앞글자가 겹쳐서..
    public static bool buyOrSell;//참일때구매, 거짓일때판매
    void Start()
    {
        productTexts = pTxt;
        productImages = pImg;
        playerCountTexts = pcTxt;
        costText = cText;
        cusCount = Random.Range(3, 6);
        Player.Instance.RenewMoney();
        //cState.Value = CustomerState.Start;
    }
    private void Awake()//상태변화 구독 위주
    {
        cState.onChange += SetCustomer;
        cState.onChange += CustomerSetItem;
        cState.onChange += CustomerSetUI;
        cState.onChange += BuyItem;
        cState.onChange += SellItem;
        cState.onChange += RejectItem;
        cState.onChange += BargainItem;
        cState.onChange += CustomerExit;
    }
    #region 버튼으로 행동패턴 변화
    public void CustomerStart()
    {
        cState.Value = CustomerState.Start;
    }
    
    public void CustomerBuy()
    {
        cState.Value = CustomerState.Buy;
    }
    public void CustomerSell()
    {
        cState.Value = CustomerState.Sell;
    }
    public void CustomerReject()
    {
        cState.Value = CustomerState.Reject;
    }
    public void CustomerBargain()
    {
        cState.Value = CustomerState.Bargain;
    }
    #endregion
    #region 손님 행동패턴
    private void SetCustomer(CustomerState _cState)
    {
        if(_cState == CustomerState.Start)//손님 객체 생성후 이동
        {
            if(cusCount <= 0)
            {
                cusCount = Random.Range(3, 6);
            }
            randcusnum = Random.Range(0,cusList.Count);
            //randcusnum = 1; //테스트용, 주석해야함.
            int randcusprefab = Random.Range(0, cusList[randcusnum].cusPrefab.Length);
            newCustomer = Instantiate(cusList[randcusnum].cusPrefab[randcusprefab], customerTransform[0]);
            CusBargainPointSet(randcusnum);
            StartCoroutine(MoveCustomerToPosition(newCustomer, customerTransform[1].position));
        }
    }
    private void CustomerSetItem(CustomerState _cState)
    {
        if(_cState == CustomerState.ItemSet)//손님대기시 플레이어에게 구매/판매 할 아이템 설정 
        {
            int randsort = Random.Range(1, tradeSortCount+1);
            BuyOrSell();//구매 or 판매 랜덤으로 돌리기
            if (buyOrSell == true)
                ItemManager.Instance.RandomSetItem(randsort);
            else
                ItemManager.Instance.RandomSetItemSell(randsort);
            cState.Value = CustomerState.SetUI;
        }
    }
    private void CustomerSetUI(CustomerState _cState)
    {
        if(_cState == CustomerState.SetUI)//UI로 표현 및 ItemManager의 productCount로 퇴장 판단
        {
            if (buyOrSell == true)
            {
                ItemManager.Instance.SetUI();
                UIon();
                BuyUI.SetActive(true);
            }
            else
            {
                ItemManager.Instance.SetSellUI();
                UIon();
                SellUI.SetActive(true);
            }

        }
    }
    private void BuyItem(CustomerState _cState)
    {
        if(_cState == CustomerState.Buy)//구매
        {
            StartCoroutine(DelayBuy());
        }
    }
    private void SellItem(CustomerState _cState)
    {
        if(_cState == CustomerState.Sell)//판매
        {
            StartCoroutine(DelaySell());
        }
    }
    private void RejectItem(CustomerState _cState)
    {
        if(_cState == CustomerState.Reject)//거절
        {
            StartCoroutine(DelayReject());
        }
    }
    private void BargainItem(CustomerState _cState)
    {
        if(_cState == CustomerState.Bargain)
        {
            CustomerUI.SetActive(false);
            BargainUI.SetActive(true);
        }
    }
    private void CustomerExit(CustomerState _cState)
    {
        if(_cState == CustomerState.End)//종료
        {
            initialChance = 100f;
            StartCoroutine(MoveAndFadeOutCustomer(newCustomer, customerTransform[2].position, fadeDuration)); //퇴장 및 페이드 아웃
            cusCount--;
            StartCoroutine(TradeEnd());
        }
    }
    #endregion
    #region 손님 공통 기능
    public void BuyOrSell()//구매 혹은 판매 신호(랜덤)
    {
        if (ItemManager.Instance.playerInventory.inventory.Count == 0)
        {
            buyOrSell = true;
            return;
        }
        if (Random.value > 0.5)
            buyOrSell = true;
            //buyOrSell = false;
        else
            buyOrSell = false;

    }
    
    public void BargainStart()//버튼으로 흥정시작
    {
        StartCoroutine("BargainCycle");
    }
    IEnumerator BargainCycle()//흥정과정
    {
        if (int.TryParse(bargainField.text, out bargainValue))//파싱
        {       
            ItemManager.Instance.SetBargainPrice(initialChance, bargainValue, bargainPoint, bargainChance);
            BargainUI.SetActive(false);
            bargainField.text = "";
            yield return YieldCache.WaitForSeconds(1.0f);
            if (ItemManager.Instance.bargainSuccess == true)//흥정성공시 UI변경
            {
                bargainButton.SetActive(false);
                rejectButton.SetActive(false);
                CustomerUI.SetActive(true);
            }
            else
            {
                CusBargainReject(randcusnum);
                yield return null;
                bargainButton.SetActive(false);
                CustomerUI.SetActive(true);
            }
        }
        else
        {
            Debug.Log("Error");
            yield return null;
            //정수 이외 다른 값일시 돌아가도록
        }
        
        
    }
    IEnumerator MoveCustomerToPosition(GameObject customer, Vector3 targetPosition)//손님 입장
    {
        while (customer.transform.position != targetPosition)
        {
            customer.transform.position = Vector2.MoveTowards(customer.transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        cState.Value = CustomerState.ItemSet;
    }
    IEnumerator MoveAndFadeOutCustomer(GameObject customer, Vector3 targetPosition, float duration)//손님퇴장
    {
        SpriteRenderer spriteRenderer = customer.GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        float elapsed = 0f;

        Vector3 startPosition = customer.transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // 이동 처리
            float t = elapsed / duration;
            customer.transform.position = Vector2.Lerp(startPosition, targetPosition, t);

            // 페이드 아웃 처리
            float alpha = Mathf.Lerp(1f, 0f, t);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null;
        }

        // 마지막 위치와 투명도 설정
        customer.transform.position = targetPosition;
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        Destroy(customer); // 페이드 아웃 후 오브젝트 삭제
    }
    IEnumerator DelayBuy()//구매지연
    {
        ItemManager.Instance.BuyProduct();
        CustomerUI.SetActive(false);
        BuyUI.SetActive(false);
        Player.Instance.RenewMoney();
        //yield return Typing("wow!");
        yield return YieldCache.WaitForSeconds(tradeDelay);
        if (ItemManager.Instance.productCount == ItemManager.Instance.productIndex.Count)
            cState.Value = CustomerState.End;
        else
        cState.Value = CustomerState.SetUI;
    }
    IEnumerator DelaySell()//판매지연
    {
        ItemManager.Instance.SellProduct();
        CustomerUI.SetActive(false);
        SellUI.SetActive(false);
        Player.Instance.RenewMoney();
        yield return YieldCache.WaitForSecondsRealTime(tradeDelay);
        if (ItemManager.Instance.productCount == ItemManager.Instance.productIndex.Count)
            cState.Value = CustomerState.End;
        else
            cState.Value = CustomerState.SetUI;
    }
    IEnumerator TradeEnd()//거래 종료
    {
        ItemManager.Instance.ListClear();
        yield return YieldCache.WaitForSeconds(fadeDuration * 1.5f);
        if (cusCount <= 0)
        {
            cState.Value = CustomerState.Idle;
            buttonEdit.SetActive(true);
        }
        else
        {
            cState.Value = CustomerState.Start;
        }
    }
    IEnumerator DelayReject()//거절 딜레이
    {
        ItemManager.Instance.productCount++;//거절시 다음상품으로 넘김
        CustomerUI.SetActive(false);
        if(buyOrSell == true)
            BuyUI.SetActive(false);
        else
            SellUI.SetActive(false);
        yield return YieldCache.WaitForSeconds(rejectDelay);
        if (ItemManager.Instance.productCount == ItemManager.Instance.productIndex.Count)
            cState.Value = CustomerState.End;
        else
            cState.Value = CustomerState.SetUI;
    }
    #region 손님 대사 효과
    IEnumerator Typing(string talk)
    {
        talkText.text = null;
        for(int i = 0; i< talk.Length; i++)
        {
            talkText.text += talk[i];
            yield return YieldCache.WaitForSeconds(typingDelay);
        }
        talkText.text = null;
        
    }
    #endregion
    private void UIon()// UI 일괄 on
    {
        rejectButton.SetActive(true);
        bargainButton.SetActive(true);
        CustomerUI.SetActive(true);
    }
    #endregion
    #region 손님 개인 기능

    private void CusBargainPointSet(int customnum)
    {
        initialChance += cusList[customnum].cusBargainChance;
    }
    
    private void CusBargainReject(int customnum)
    {
        switch(customnum)
        {
            case 1:
                if(ItemManager.Instance.bargainSuccess == false)
                {
                    StopCoroutine("BargainCycle");
                    cState.Value = CustomerState.End;
                }
                break;
        }
    }
    #endregion
}
