using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOpen : MonoBehaviour
{
    public GameObject Map;

    public void MapButton()
    {
        var map = Instantiate<GameObject>(Map, new Vector3(0, 0, 0), Quaternion.identity);
        map.transform.SetParent(GameObject.FindWithTag("Canvas").transform);
    }
}
