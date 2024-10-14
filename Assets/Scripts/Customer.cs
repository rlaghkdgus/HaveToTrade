using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * 거래 진입 시 고객의 수, 거래 물품의 종류 및 숫자를 정할 공간
 * 
 */
public class Customer : MonoBehaviour
{
    public int cusCount; // 손님 수
    public float speed; //손님 이동속도
    public List<GameObject> customerPrefab;//손님 프리팹
    public List<Transform> customerTransform;// 생성, 거래위치, 퇴장
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
        cState.onChange += BuyItem;
    }
    public void CustomerBuy()
    {
        cState.Value = CustomerState.Buy;
    }
    public void CustomerSell()
    {
        cState.Value = CustomerState.Sell;
    }

    private void SetCustomer(CustomerState _cState)
    {
        if(_cState == CustomerState.Start)
        {
            int randnum = Random.Range(0,customerPrefab.Count);
            GameObject newCustomer = Instantiate(customerPrefab[randnum], customerTransform[0]);
            StartCoroutine(MoveCustomerToPosition(newCustomer, customerTransform[1].position));
        }
    }

    IEnumerator MoveCustomerToPosition(GameObject customer, Vector3 targetPosition)//손님 이동 담당
    {
        while (customer.transform.position != targetPosition)
        {
            customer.transform.position = Vector2.MoveTowards(customer.transform.position, targetPosition, speed * Time.deltaTime);
            yield return null; 
        }
        cState.Value = CustomerState.Idle; 
    }
    private void CustomerSetItem(CustomerState _cState)
    {
        if(_cState == CustomerState.Idle)
        {
            int randsort = Random.Range(1, 4);
            ItemManager.Instance.RandomSetItem(randsort);
            ItemManager.Instance.SetUI();
        }
    }
    private void BuyItem(CustomerState _cState)
    {
        if(_cState == CustomerState.Buy)
        {
            ItemManager.Instance.BuyProduct();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
