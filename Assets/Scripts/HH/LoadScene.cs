using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    public void GoTradeScene()
    {
       SceneManager.LoadScene(1);
    }
    public void GoTownScene()
    {
        SceneManager.LoadScene(0);
    }
}
