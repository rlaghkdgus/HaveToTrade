using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//사용할 열거형 정리, 주로 옵저버패턴에 사용

public enum ItemSorts
{
    food,
    pFood,
    clothes,
    furniture,
    accesory
}
public enum CustomerState
{
    Idle,
    Start,
    ItemSet,
    SetUI,
    Buy,
    Sell,
    Reject,
    Bargain,
    End,
}

public enum QuestType
{
    Idle,
    Trade,
    Delivery
}
public enum VillageType
{
    Idle,
    GreStar,
    Smokian
}

