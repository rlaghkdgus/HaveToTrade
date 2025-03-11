using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/*
 * �ŷ� ���� �� ������ ��, �ŷ� ��ǰ�� ���� �� ���ڸ� ���� ����
 * �մ� �ൿ���� ��� : ����(Start) -> ���(Idle) -> �����۸Ŵ������� ������ ���������� UI����-> ����(����)-> ���� or �Ǹ� -> ����(End) -> ����, �մԼ� ��ŭ �ݺ�
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
    [Header("�մԼ�, ����� ���۽� ���� ����")]
    public int cusCount;
    [Header("�մ� ���� �ɼ�")]
    public float speed; //�մ� �̵��ӵ�
    public float tradeDelay; // �մ� �ŷ� ������
    public float rejectDelay; // �մ� ���� �� ���� �ŷ� ������
    public float fadeDuration; // ���̵� �ƿ� ���� �ð�
    [Header("���� ���� �ɼ�")]
    public float initialChance; // ������ �ʱ� Ȯ��
    public int bargainPoint; // ����Ȯ�� ����
    public float bargainChance; // �ش� ������ ���������� ����Ȯ���� ��%�� ��������
    [Header("��� ������ �ŷ��Ұ���")]
    public int tradeSortCount; // �մ��� ��� ������ �ŷ��Ұ���
    [Header("�մ� ������(�մ� �� ��� �� ����),���� �� ���� ��ġ ����")]
    public List<customerPrefab> cusList;
    public List<Transform> customerTransform;// ����, �ŷ���ġ, ����


    [Header("�մ� �ŷ�â")]
    public GameObject CustomerUI;
    public GameObject BuyUI;
    public GameObject SellUI;
    public GameObject BargainUI;
    [SerializeField] private TMP_Text cText;
    [SerializeField] private TMP_Text pTxt;
    [SerializeField] private TMP_Text pcTxt; // �÷��̾ ����ִ� ��ǰ ���� ī��Ʈ
    [SerializeField] private TMP_InputField bargainField;
    [SerializeField] private Image pImg;
    [Header("������ On/Off ������Ʈ")]
    [SerializeField] private GameObject bargainButton;
    [SerializeField] private GameObject rejectButton;
    [Header("���� ��ư")]
    [SerializeField] GameObject ButtonGroup;

    [Header("�մ� ��ȭ")]
    public TMP_Text talkText; // ��ȭ�ؽ�Ʈ
    [SerializeField] Transform textTransform;//��ȭ������ġ
    [SerializeField] float typingDelay;//Ÿ���� ����
    [Header("�ŷ� �� ��ư ����")]
    [SerializeField] GameObject buttonEdit;
    //static,private ���������� ���� �� �ν����Ϳ� �Ⱥ��̴� ��ҵ�
    public Data<CustomerState> cState = new Data<CustomerState>();//���º� �̺�Ʈ
    private GameObject newCustomer;
    private int bargainValue;
    public static int randcusnum = 0;
    public static TMP_Text productTexts;
    public static Image productImages;
    public static TMP_Text playerCountTexts;// �÷��̾ ����ִ� ��ǰ ���� ī��Ʈ
    public static TMP_Text costText;//���� �ؽ�Ʈ, product�� price�� �ձ��ڰ� ���ļ�..
    public static bool buyOrSell;//���϶�����, �����϶��Ǹ�
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
    private void Awake()//���º�ȭ ���� ����
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
    #region ��ư���� �ൿ���� ��ȭ
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
    #region �մ� �ൿ����
    private void SetCustomer(CustomerState _cState)
    {
        if(_cState == CustomerState.Start)//�մ� ��ü ������ �̵�
        {
            if(cusCount <= 0)
            {
                cusCount = Random.Range(3, 6);
            }
            randcusnum = Random.Range(0,cusList.Count);
            //randcusnum = 1; //�׽�Ʈ��, �ּ��ؾ���.
            int randcusprefab = Random.Range(0, cusList[randcusnum].cusPrefab.Length);
            newCustomer = Instantiate(cusList[randcusnum].cusPrefab[randcusprefab], customerTransform[0]);
            CusBargainPointSet(randcusnum);
            StartCoroutine(MoveCustomerToPosition(newCustomer, customerTransform[1].position));
        }
    }
    private void CustomerSetItem(CustomerState _cState)
    {
        if(_cState == CustomerState.ItemSet)//�մԴ��� �÷��̾�� ����/�Ǹ� �� ������ ���� 
        {
            int randsort = Random.Range(1, tradeSortCount+1);
            BuyOrSell();//���� or �Ǹ� �������� ������
            if (buyOrSell == true)
                ItemManager.Instance.RandomSetItem(randsort);
            else
                ItemManager.Instance.RandomSetItemSell(randsort);
            cState.Value = CustomerState.SetUI;
        }
    }
    private void CustomerSetUI(CustomerState _cState)
    {
        if(_cState == CustomerState.SetUI)//UI�� ǥ�� �� ItemManager�� productCount�� ���� �Ǵ�
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
        if(_cState == CustomerState.Buy)//����
        {
            StartCoroutine(DelayBuy());
        }
    }
    private void SellItem(CustomerState _cState)
    {
        if(_cState == CustomerState.Sell)//�Ǹ�
        {
            StartCoroutine(DelaySell());
        }
    }
    private void RejectItem(CustomerState _cState)
    {
        if(_cState == CustomerState.Reject)//����
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
        if(_cState == CustomerState.End)//����
        {
            initialChance = 100f;
            StartCoroutine(MoveAndFadeOutCustomer(newCustomer, customerTransform[2].position, fadeDuration)); //���� �� ���̵� �ƿ�
            cusCount--;
            StartCoroutine(TradeEnd());
        }
    }
    #endregion
    #region �մ� ���� ���
    public void BuyOrSell()//���� Ȥ�� �Ǹ� ��ȣ(����)
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
    
    public void BargainStart()//��ư���� ��������
    {
        StartCoroutine("BargainCycle");
    }
    IEnumerator BargainCycle()//��������
    {
        if (int.TryParse(bargainField.text, out bargainValue))//�Ľ�
        {       
            ItemManager.Instance.SetBargainPrice(initialChance, bargainValue, bargainPoint, bargainChance);
            BargainUI.SetActive(false);
            bargainField.text = "";
            yield return YieldCache.WaitForSeconds(1.0f);
            if (ItemManager.Instance.bargainSuccess == true)//���������� UI����
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
            //���� �̿� �ٸ� ���Ͻ� ���ư�����
        }
        
        
    }
    IEnumerator MoveCustomerToPosition(GameObject customer, Vector3 targetPosition)//�մ� ����
    {
        while (customer.transform.position != targetPosition)
        {
            customer.transform.position = Vector2.MoveTowards(customer.transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        cState.Value = CustomerState.ItemSet;
    }
    IEnumerator MoveAndFadeOutCustomer(GameObject customer, Vector3 targetPosition, float duration)//�մ�����
    {
        SpriteRenderer spriteRenderer = customer.GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        float elapsed = 0f;

        Vector3 startPosition = customer.transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // �̵� ó��
            float t = elapsed / duration;
            customer.transform.position = Vector2.Lerp(startPosition, targetPosition, t);

            // ���̵� �ƿ� ó��
            float alpha = Mathf.Lerp(1f, 0f, t);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null;
        }

        // ������ ��ġ�� ������ ����
        customer.transform.position = targetPosition;
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        Destroy(customer); // ���̵� �ƿ� �� ������Ʈ ����
    }
    IEnumerator DelayBuy()//��������
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
    IEnumerator DelaySell()//�Ǹ�����
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
    IEnumerator TradeEnd()//�ŷ� ����
    {
        ItemManager.Instance.ListClear();
        yield return YieldCache.WaitForSeconds(fadeDuration * 1.5f);
        if (cusCount <= 0)
        {
            cState.Value = CustomerState.Idle;
            ButtonGroup.SetActive(true);
            buttonEdit.SetActive(true);
        }
        else
        {
            cState.Value = CustomerState.Start;
        }
    }
    IEnumerator DelayReject()//���� ������
    {
        ItemManager.Instance.productCount++;//������ ������ǰ���� �ѱ�
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
    #region �մ� ��� ȿ��
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
    private void UIon()// UI �ϰ� on
    {
        rejectButton.SetActive(true);
        bargainButton.SetActive(true);
        CustomerUI.SetActive(true);
    }
    #endregion
    #region �մ� ���� ���

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
