using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOpen : MonoBehaviour
{
    public GameObject Map;

    public void MapButton()
    {
        var map = Instantiate<GameObject>(Map);
        map.transform.SetParent(GameObject.FindWithTag("Canvas").transform);
        map.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
}
