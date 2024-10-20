using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 거래 진입 시 고객의 수, 거래 물품의 종류 및 숫자를 정할 공간
 * 손님 행동패턴 요약 : 입장(Start) -> 대기(Idle) -> 구매 or 판매 -> 퇴장(End) -> 입장, 손님수 만큼 반복
 */
public class Customer : MonoBehaviour
{
    [Header("손님수, 현재는 시작시 랜덤 지정")]
    public int cusCount;
    [Header("손님 세부 옵션")]
    public float speed; //손님 이동속도
    public float buyDelay; // 손님 구매 딜레이
    public List<GameObject> customerPrefab;//손님 프리팹
    public List<Transform> customerTransform;// 생성, 거래위치, 퇴장


    [Header("손님 거래창")]
    public GameObject CustomerUI;
    public Data<CustomerState> cState = new Data<CustomerState>();//상태별 이벤트


    void Start()
    {
        cusCount = Random.Range(3, 5);
        cState.Value = CustomerState.Start;
    }
    private void Awake()
    {
        cState.onChange += SetCustomer;
        cState.onChange += CustomerSetItem;
        cState.onChange += CustomerSetUI;
        cState.onChange += BuyItem;
    }
    #region 버튼으로 행동패턴 변화
    public void CustomerBuy()
    {
        cState.Value = CustomerState.Buy;
    }
    public void CustomerSell()
    {
        cState.Value = CustomerState.Sell;
    }
    #endregion
    #region 손님 행동패턴
    private void SetCustomer(CustomerState _cState)
    {
        if(_cState == CustomerState.Start)//손님 객체 생성후 이동
        {
            int randnum = Random.Range(0,customerPrefab.Count);
            GameObject newCustomer = Instantiate(customerPrefab[randnum], customerTransform[0]);
            StartCoroutine(MoveCustomerToPosition(newCustomer, customerTransform[1].position));
        }
    }
    private void CustomerSetItem(CustomerState _cState)
    {
        if(_cState == CustomerState.ItemSet)//손님대기시 플레이어에게 구매/판매 할 아이템 설정 
        {
            int randsort = Random.Range(1, 4);
            ItemManager.Instance.RandomSetItem(randsort);
            cState.Value = CustomerState.SetUI;
        }
    }
    private void CustomerSetUI(CustomerState _cState)
    {
        if(_cState == CustomerState.SetUI)//UI로 표현 및 ItemManager의 productCount로 퇴장 판단
        {
            if (ItemManager.Instance.productCount == ItemManager.Instance.productIndex.Count)
                cState.Value = CustomerState.End;
            ItemManager.Instance.SetUI();
            CustomerUI.SetActive(true);
        }
    }
    private void BuyItem(CustomerState _cState)
    {
        if(_cState == CustomerState.Buy)//구매
        {
            StartCoroutine(DelayBuy());
            
        }
    }
    private void CustomerExit(CustomerState _cState)
    {
        if(_cState == CustomerState.End)
        {
            ItemManager.Instance.ListClear();
            //페이드아웃 추가
            //이동추가
            cusCount--;
            TradeEnd();
        }
    }
    #endregion
    #region 손님 기능
    IEnumerator MoveCustomerToPosition(GameObject customer, Vector3 targetPosition)//손님 이동 담당
    {
        while (customer.transform.position != targetPosition)
        {
            customer.transform.position = Vector2.MoveTowards(customer.transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        cState.Value = CustomerState.ItemSet;
    }
    IEnumerator DelayBuy()//구매지연
    {
        ItemManager.Instance.BuyProduct();
        CustomerUI.SetActive(false);
        yield return new WaitForSecondsRealtime(buyDelay);
        cState.Value = CustomerState.SetUI;
    }
    private void TradeEnd()
    {
        if(cusCount == 0)
        {
            cState.Value = CustomerState.Idle;
            //종료시 나올 UI및 씬이동
        }
        else
        {
            cState.Value = CustomerState.Start;
        }
    }
    #endregion

}
