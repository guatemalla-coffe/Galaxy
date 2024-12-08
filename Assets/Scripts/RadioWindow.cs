using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RadioWindow : MonoBehaviour
{
    public Transform lettersLayoutList;
    public Radio radio;

    public Transform chooseLetterWin;
    public Transform awaitForSendingLetterWin;
    public Transform answerWin;
    
    
    public void LoadLetters()
    {
        ClearLettersList();
        foreach (var letter in GameManager.Instance.letters)
        {
            Instantiate(Resources.Load<GameObject>($"Letters/{letter}"), lettersLayoutList);
        }
    }

    private void ClearLettersList()
    {
        for (int i = 0; i < lettersLayoutList.childCount; i++)
        {
            Destroy(lettersLayoutList.GetChild(i).gameObject);
        }
    }

    public void OpenWindow(Radio radioS)
    {
        radio = radioS;
        new RequestsManager().GetShops();
        gameObject.SetActive(true);
        LoadLetters();
    }

    public void CloseWindow()
    {
        gameObject.SetActive(false);
    }
}
