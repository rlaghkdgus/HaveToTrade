using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class QuestSystem : Singleton<QuestSystem>
{
    [SerializeField] private QuestTable questTable;
    //public List<TMP_Text> questName = new List<TMP_Text>();
    [Header("퀘스트 버튼")]
    public List<Button> questButtons = new List<Button>();
    [Header("퀘스트 문구")]
    public List<TMP_Text> questDescription = new List<TMP_Text>();
    [Header("퀘스트 번호 확인용")]
    public List<int> questRandIndex = new List<int>();
    [Header("퀘스트UI(on/off용)")]
    public GameObject questUI;
    [Header("수행중인 퀘스트 텍스트")]
    public TMP_Text targetQuestText;
    [Header("현재 퀘스트 진행중인 데이터를 담는 변수들")]
    private int currentIndex;
    public ItemSorts qTargetItem;
    public string qTargetItemName;
    public float questTarget;
    public float questGoal;
    private int questReward;
    private VillageType questVillage;
    public QuestType currentQuestType;
    public bool questBuyOrSell; // 살때 true, 팔때 false;
    [Header("퀘스트 진행 신호")]
    public bool questSign; // 퀘스트 진행중인지 확인
    public bool deliveryCheckSign;
    #region 퀘스트 생성 및 관리
    public void RandomQuest()
    {
        for(int i = 0; i < 3; i++)
        {
            int randnum;
            do
            {
                randnum = Random.Range(0, questTable.quest.Count);
            }while(questRandIndex.Contains(randnum));
            questRandIndex.Add(randnum);
            questDescription[i].text = questTable.quest[randnum].questText;
        }
    }
    public void QuestAccept(int buttonindex)
    {
        currentIndex = questRandIndex[buttonindex];
        qTargetItem = questTable.quest[currentIndex].sort;
        qTargetItemName = questTable.quest[currentIndex].questStuffName;
        questGoal = questTable.quest[currentIndex].questInfo;
        questReward = questTable.quest[currentIndex].reward;
        questBuyOrSell = questTable.quest[currentIndex].buyOrSell;
        currentQuestType = questTable.quest[currentIndex].questType;
        questVillage = questTable.quest[currentIndex].villageType;
        questUI.SetActive(false);
        TextReset();
        questSign = true;
        questRandIndex.Clear();
    }
    public void QuestClear()
    {
        targetQuestText.text = "";
        questTarget = 0;
        questVillage = VillageType.Idle;
        currentQuestType = QuestType.Idle;
        questSign = false;
        deliveryCheckSign = false;
        FameUp();
    }
    public void QuestProgress(pItem ItemCheck, float QuestWeight)
    {
        if (questSign == false || ItemCheck.sort != qTargetItem || Customer.buyOrSell != questBuyOrSell)
            return;
        questTarget += QuestWeight;
        TextReset();
        if (questTarget >= questGoal)
            QuestClear();
    }
    #endregion
    private void Start()
    {
        RandomQuest(); //퀘스트 랜덤 배치       
    }
  
    private void TextReset()
    {
        if (currentQuestType == QuestType.Trade)
            targetQuestText.text = "target:" + qTargetItem.ToString() + "  " + questTarget + " / " + questGoal;
        else if (currentQuestType == QuestType.Delivery)
            targetQuestText.text = "target:" + qTargetItem.ToString() + " destination :" + questVillage.ToString();
        else
            targetQuestText.text = "";
    }
    private void FameUp()
    {
        switch(qTargetItem)
        {
            case ItemSorts.food:
                Player.Instance.foodFame.fame += questReward;
                break;
            case ItemSorts.furniture:
                Player.Instance.furnFame.fame += questReward;
                break;
            case ItemSorts.accesory:
                Player.Instance.accesoryFame.fame += questReward;
                break;
            case ItemSorts.clothes:
                Player.Instance.clothFame.fame += questReward;
                break;
            case ItemSorts.pFood:
                Player.Instance.pFoodFame.fame += questReward;
                break;
        }
    }
    public void DeliveryQuestCheck()
    {
        if (currentQuestType != QuestType.Delivery || TownManager.Instance.curTown.TownType != questVillage)
            //배달 퀘스트가 아닐시 UI 추가가능
            return;
        ItemManager.Instance.playerInventory.CheckItemWeight(qTargetItemName);
        if (deliveryCheckSign == false)
            return;
        else if(deliveryCheckSign == true)
        {
            ItemManager.Instance.playerInventory.QuestItemRemove(qTargetItemName);
            QuestClear();
        }
 

    }
}
