using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>
{
    [SerializeField] ItemData items;
    public List<Item> playerItems;
    public int money;
    
    private void Buy(Item item)
    {
        if(money < item.price)
        {
            //ºÒ°¡´É UI
            return;
        }
        money -= item.price;
        for(int i = 0; i < playerItems.Count; i++)
        {
            if (playerItems[i] == item)
                playerItems[i].counts += 1;
            else
                playerItems.Add(item);
        }
    }
    private void Sell(Item item)
    {
       
    }

}
