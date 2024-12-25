using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTest : MonoBehaviour
{
    [SerializeField] private DialogSystem dialog_1;

    private bool TestStart = false;

    private void Update()
    {
        if (TestStart)
        {
            dialog_1.UpdateDialog();
        }
    }

    public void TestDialog()
    {
        TestStart = true;
    }
}
