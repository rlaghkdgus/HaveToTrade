using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOut : MonoBehaviour
{
    public void DestroyUI()
    {
        Destroy(transform.parent.gameObject);
    }
}
