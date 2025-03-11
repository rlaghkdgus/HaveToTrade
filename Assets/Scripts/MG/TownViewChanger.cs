using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownViewChanger : MonoBehaviour
{
    [SerializeField] private float FadeTime = 1f;
    public int currentIndex = 0;
    [SerializeField] private GameObject FadeUI;

    [SerializeField] private GameObject Change_R;
    [SerializeField] private GameObject Change_L;

    public void ViewChange(int index, List<GameObject> SingleTownList)
    {
        if(currentIndex + index >= 0 && currentIndex + index <= SingleTownList.Count)
        {
            StartCoroutine(ChangeTownViewPrefab(index, SingleTownList));
        }
    }

    public void ButtonUIUpdate(List<GameObject> SingleTownList)
    {
        if (currentIndex == 0)
        {
            Change_L.SetActive(false);
            Change_R.SetActive(true);
        }
        else if (currentIndex == SingleTownList.Count - 1)
        {
            Change_L.SetActive(true);
            Change_R.SetActive(false);
        }
        else if (currentIndex > 0 && currentIndex < SingleTownList.Count - 1)
        {
            Change_L.SetActive(true);
            Change_R.SetActive(true);
        }
    }

    IEnumerator ChangeTownViewPrefab(int index, List<GameObject> SingleTownList)
    {
        GameObject fade = Instantiate(FadeUI);
        yield return new WaitForSeconds(FadeTime);
        Destroy(TownManager.Instance.TownClone);
        currentIndex += index;
        TownManager.Instance.TownClone = Instantiate(SingleTownList[currentIndex]);
        ButtonUIUpdate(SingleTownList);
    }
}
