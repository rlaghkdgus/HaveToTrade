using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGscroll : MonoBehaviour
{
    [SerializeField] private Renderer rd;
    public float speed;

    private void Awake()
    {
        rd = GetComponent<Renderer>();
    }

    private void Update()
    {
        float movespeed = Time.deltaTime * speed;
        rd.material.mainTextureOffset += Vector2.right * movespeed;
    }
}
