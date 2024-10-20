using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//사용할 열거형 정리, 주로 옵저버패턴에 사용
public enum VilageType 
{
    Mountain,
    Beach
}

public enum FameType
{
    food,
    pFood,
    clothes,
    furniture,
    accesory
}

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
    End
}

