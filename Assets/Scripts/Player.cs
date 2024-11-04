using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Player : Singleton<Player>
{    
    public int money;
    public TMP_Text moneyText;

    public void RenewMoney()
    {
        moneyText.text = "Money :" + money;
    }
}
