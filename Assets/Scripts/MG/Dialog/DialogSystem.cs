using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    public DialogData DialogDB;

    [Header("페이지 하나 관리 방식용 변수")]
    [SerializeField] private int branch; // 리스트에서 선택한 대사 분기 인덱스

    [Header("리스트 선택 방식용 변수")]
    public string seletedDialogName; // 선택한 리스트 이름
    private List<TextData> currentList; // 선택한 리스트를 담을 빈 리스트

    [Header("공용 변수")]
    [SerializeField] private Speaker[] speakers; // 대화에 참여하는 캐릭터 UI 배열

    [SerializeField] private List<DialogData_S> dialogs; // 대화 정보를 담는 배열

    public TextMeshProUGUI Name;
    public TextMeshProUGUI Dialog;
    public Image dialogUI;
    public GameObject DialogBG;

    [SerializeField] private bool isAutoStart = true; // 자동 시작 여부
    [SerializeField] private bool isFirst = true; // 최초 1회만 호출하기 위한 bool 값
    private int currentDialogIndex = -1; // 현재 대사 순번
    private int currentSpeakerIndex = 0; // 현재 화자 speakers 배열 순번

    [Header("타이핑 효과 관련 변수")]
    [SerializeField] private float typingSpeed = 0.1f; // 타이핑 속도
    [SerializeField] private bool isTypingEffect = false; // 타이핑 효과 제어 변수

    private void Awake()
    {
        DialogListLoading();

        Setting();
    }

    public void DialogListLoading() // 리스트 선택 방식
    {
        // 장점 : 액셀 파일 관리가 편함
        // 단점 : 스크립트 조금씩 수정 필요
        dialogs.Clear();

        var field = DialogDB.GetType().GetField(seletedDialogName);
        if (field != null && field.FieldType == typeof(List<TextData>))
        {
            currentList = (List<TextData>)field.GetValue(DialogDB);

            int index = 0;
            for (int i = 0; i < currentList.Count; ++i)
            {
                DialogData_S newDialog = new DialogData_S
                {
                    speakerIndex = currentList[i].SpeakerIndex,
                    name = currentList[i].Name,
                    dialog = currentList[i].Dialog
                };
                dialogs.Add(newDialog);
                index++;
            }
        }
    }

    private void DialogBranchLoading() // 페이지 하나 방식
    {
        // 장점 : 스크립트 수정 필요 없음
        // 단점 : 모든 대화를 액셀 한 시트에서만 관리해야해서 양이 많으면 힘들수도 있음
        /*
        dialogs.Clear();
        int index = 0;
        for(int i = 0; i < DialogDB.TextEX.Count; ++i)
        {
            if (DialogDB.TextEX[i].Branch == branch)
            {
                dialogs[index].name = DialogDB.TextEX[i].Name;
                dialogs[index].dialog = DialogDB.TextEX[i].Dialog;
                index++;
            }
        }*/
    }

    private void Setting()
    {
        for (int i = 0; i < speakers.Length; ++i)
        {
            SetActiveSpeakers(speakers[i], false);

            speakers[i].SpeakerImage.gameObject.SetActive(true);
        }
        SetActiveTextUI(false);
    }

    public bool UpdateDialog()
    {
        if (isFirst == true)
        {
            Setting();

            if (isAutoStart)
            {
                dialogUI.gameObject.SetActive(true);
                SetActiveTextUI(true);
                SetNextDialog();
            }

            isFirst = false;
        }

        while (true)
        {
            if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && DialogBG.activeSelf)
            {
                if (isTypingEffect == true)
                {
                    isTypingEffect = false;

                    StopCoroutine("OnTypingText");
                    Dialog.text = dialogs[currentDialogIndex].dialog;
                    //speakers[currentSpeakerIndex].Dialog.text = dialogs[currentDialogIndex].dialog;
                    //speakers[currentSpeakerIndex].Cursor.SetActive(true);

                    return false;
                }

                if (dialogs.Count > currentDialogIndex + 1)
                {
                    SetNextDialog();
                }
                else
                {/*
                    for (int i = 0; i < speakers.Length; ++i)
                    {
                        SetActiveSpeakers(speakers[i], false);
                        speakers[i].SpeakerImage.gameObject.SetActive(false);
                    }
                    dialogUI.gameObject.SetActive(false);
                    DialogBG.SetActive(false);*/

                    TestInit();
                    return true;
                }
            }
            else if (DialogBG.activeSelf == false)
            {
                DialogBG.SetActive(true);
            }
            return false;
        }
    }

    public void SetActiveFalseUI()
    {
        for (int i = 0; i < speakers.Length; ++i)
        {
            SetActiveSpeakers(speakers[i], false);
            speakers[i].SpeakerImage.gameObject.SetActive(false);
        }
        dialogUI.gameObject.SetActive(false);
        DialogBG.SetActive(false);
    }

    private void TestInit() // 테스트 메서드
    {
        isFirst = true;
        currentDialogIndex = -1;
        currentSpeakerIndex = 0;
    }

    private void SetNextDialog()
    {
        SetActiveSpeakers(speakers[currentSpeakerIndex], false);

        currentDialogIndex++;

        currentSpeakerIndex = dialogs[currentDialogIndex].speakerIndex;

        SetActiveSpeakers(speakers[currentSpeakerIndex], true);

        //speakers[currentSpeakerIndex].Name.text = dialogs[currentDialogIndex].name;
        Name.text = dialogs[currentDialogIndex].name;

        //speakers[currentSpeakerIndex].Dialog.text = dialogs[currentDialogIndex].dialog;
        Dialog.text = dialogs[currentDialogIndex].dialog;

        StartCoroutine("OnTypingText");
    }

    private void SetActiveSpeakers(Speaker speaker, bool isActive)
    {
        //speaker.Name.gameObject.SetActive(isActive);
        //speaker.Dialog.gameObject.SetActive(isActive);

        //speaker.Cursor.SetActive(false);

        Color color = speaker.SpeakerImage.color;
        color.a = isActive == true ? 1 : 0.2f;
        speaker.SpeakerImage.color = color;
    }

    private void SetActiveTextUI(bool isActive)
    {
        Name.gameObject.SetActive(isActive);
        Dialog.gameObject.SetActive(isActive);
    }

    private IEnumerator OnTypingText()
    {
        int index = 0;

        isTypingEffect = true;

        while (index <= dialogs[currentDialogIndex].dialog.Length)
        {
            //speakers[currentSpeakerIndex].Dialog.text = dialogs[currentDialogIndex].dialog.Substring(0, index);
            Dialog.text = dialogs[currentDialogIndex].dialog.Substring(0, index);

            index++;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTypingEffect = false;

        //speakers[currentSpeakerIndex].Cursor.SetActive(true);
    }
}

[System.Serializable]
public struct Speaker
{
    public Image SpeakerImage;
    //public TextMeshProUGUI Name;
    //public TextMeshProUGUI Dialog;
    //public GameObject Cursor;
}

[System.Serializable]
public struct DialogData_S
{
    public int speakerIndex;
    public string name;
    public string dialog;
}