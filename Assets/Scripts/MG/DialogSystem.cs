using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    [SerializeField] private int branch;

    [SerializeField] private DialogData DialogDB;
    
    [SerializeField] private Speaker[] speakers;

    [SerializeField] private bool isAutoStart = true;
    private bool isFirst = true;
    private int currentDialogIndex = -1;
    private int currentSpeakerIndex = 0;

    private void Awake()
    {
        int index = 0;
        for(int i = 0; i < DialogDB.TextEX.Count; ++i)
        {
            if (DialogDB.TextEX[i].Branch == branch)
            {
                
            }
        }

        Set();
    }

    private void Set()
    {
        for(int i = 0; i < speakers.Length; ++i)
        {


            speakers[i].sr.gameObject.SetActive(true);
        }
    }

    public bool UpdateDialog()
    {
        if(isFirst == true)
        {
            Set();

            if (isAutoStart) ;

            isFirst = false;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            
        }

        return false;
    }

    private void SetNextDialog()
    {

    }
}

[System.Serializable]
public struct Speaker
{
    public SpriteRenderer sr;
    public Image DialogUI;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Dialog;
    //public GameObject Cursor;
}