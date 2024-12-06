using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Market : MonoBehaviour
{
    public int m_productCountmin;
    public int m_productCountmax;
    public int m_price;

    private void Start()
    {
        Random.Range(m_productCountmin, m_productCountmax);

    }
}
