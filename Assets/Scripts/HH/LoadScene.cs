using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    private static LoadScene instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Button SceneChangeButton = GameObject.FindWithTag("Canvas").transform.Find("ChangeScene").GetComponent<Button>();

        if(SceneChangeButton != null)
        {
            if(SceneManager.GetActiveScene().buildIndex == 0)
            {
                SceneChangeButton.onClick.AddListener(() => ChangeScene(1));
                TownManager.Instance.GoTradeButton = SceneChangeButton.gameObject;
                TownManager.Instance.TownGenerate();
            }
            else if(SceneManager.GetActiveScene().buildIndex == 1)
            {
                SceneChangeButton.onClick.AddListener(() => ChangeScene(0));
            }
        }

        Debug.Log("Scene 이동 이벤트 추가 완료");
    }

    public void ChangeScene(int SceneIndex)
    {
       SceneManager.LoadScene(SceneIndex);
    }
}
