using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Player : Singleton<Player>
{
    [SerializeField] ItemData dItems;
    public List<Item> playerItems;
    public int money;
    public Image pImage;
    public TMP_Text priceText;
    
    /*
     * 현재 코드의 문제점 : 어떤 상황에서 UI및 아이템을 세팅할것인가
     * 현재 작동 방식 : item을 셋팅 -> money가 부족하면 바로 리턴 -> UI셋팅 -> 돈 제거 후 리스트삽입
     */
    public void SetUI(Item item)
    {
        pImage.sprite = item.image;
        priceText.text = "" + item.price;
    }
    public void Buy()
    {
        Item item = dItems.items[Random.Range(0, dItems.items.Length)];
        if(money < item.price)
        {
            //불가능 UI
            return;
        }
        SetUI(item);
        money -= item.price;
        for(int i = 0; i < playerItems.Count; i++)
        {
            if (playerItems[i].stuffName == item.stuffName)
            {
                playerItems[i].counts += 1;
                return;
            }
        }
        playerItems.Add(item);
        return;
    }
    public void Sell()
    {
       
    }

}
