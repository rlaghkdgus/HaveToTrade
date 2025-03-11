using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScaler : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private SpriteRenderer sr;

    private void Awake()
    {
        cam = Camera.main;

        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Resize();
    }

    public void Resize()
    {
        if (cam == null || sr == null) return;

        float cameraHeight = cam.orthographicSize * 2;
        float cameraWidth = cameraHeight * cam.aspect;

        float spriteHeight = sr.sprite.bounds.size.y;
        float spriteWidth = sr.sprite.bounds.size.x;

        transform.localScale = new Vector3(cameraWidth / spriteWidth, cameraHeight / spriteHeight, 1);
    }
}
