using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Answer_Shuttle : MonoBehaviour
{ 
    public TMP_InputField inputField;
    public GameObject wrongAnswerText;

    public string messageName;
    public string planName;
    public string correctAnswer;
    
    public void Check()
    {
        string ifText = inputField.text.ToLower();
        if (ifText == correctAnswer)
        {
            GameManager.Instance.letters.Remove(messageName);
            UIManager.Instance.radioWindow.radio.SpawnResource(Resources.Load<GameObject>($"Resources/{planName}"));
            UIManager.Instance.radioWindow.chooseLetterWin.gameObject.SetActive(true);
            UIManager.Instance.radioWindow.answerWin.gameObject.SetActive(false);
            UIManager.Instance.radioWindow.LoadLetters();
            TaskManager.Instance.CompleteTask(22);
            Destroy(gameObject);
        }
        else
        {
            wrongAnswerText.SetActive(true);
        }
    }
}
