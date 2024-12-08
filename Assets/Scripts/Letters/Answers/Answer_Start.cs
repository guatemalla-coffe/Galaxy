using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Answer_Start : MonoBehaviour
{
    public void DoOnClick()
    {
        GameManager.Instance.letters.Remove("Letter_0");
        GameManager.Instance.letters.Add("Letter_Engine");
        GameManager.Instance.letters.Add("Letter_Body");
        GameManager.Instance.letters.Add("Letter_Wings");
        GameManager.Instance.letters.Add("Letter_FuelTank");
        GameManager.Instance.letters.Add("Letter_ControlPanel");
        UIManager.Instance.radioWindow.chooseLetterWin.gameObject.SetActive(true);
        UIManager.Instance.radioWindow.answerWin.gameObject.SetActive(false);
        UIManager.Instance.radioWindow.LoadLetters();
        Destroy(gameObject);
    }
}
