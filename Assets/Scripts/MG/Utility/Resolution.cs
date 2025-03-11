using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class Resolution : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;

    public Toggle fullScreenToggle;

    public GameObject ConfigUI;

    UnityEngine.Resolution[] AllResolutions;
    private bool isFullScreen;
    int SelectedResolution;

    List<UnityEngine.Resolution> SelectedResolutionList = new List<UnityEngine.Resolution>();

    private void Start()
    {
        isFullScreen = true;
        AllResolutions = Screen.resolutions;

        List<string> resolutionStringList = new List<string>();
        string newRes;
        foreach(UnityEngine.Resolution res in AllResolutions)
        {
            float aspectRatio = (float)res.width / res.height;
            if(Mathf.Abs(aspectRatio - 16f / 9f) < 0.01f)
            {
                newRes = res.width.ToString() + " x " + res.height.ToString();
                if (!resolutionStringList.Contains(newRes))
                {
                    resolutionStringList.Add(newRes);
                    SelectedResolutionList.Add(res);
                }
            }
        }

        resolutionDropdown .AddOptions(resolutionStringList);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ConfigUI.SetActive(!ConfigUI.activeSelf);
        }
    }

    public void ChangeResolution()
    {
        SelectedResolution = resolutionDropdown.value;
        Screen.SetResolution(SelectedResolutionList[SelectedResolution].width, SelectedResolutionList[SelectedResolution].height, isFullScreen);
    }

    public void ChangeFullScreen()
    {
        isFullScreen = fullScreenToggle.isOn;
        Screen.SetResolution(SelectedResolutionList[SelectedResolution].width, SelectedResolutionList[SelectedResolution].height, isFullScreen);
    }
}
