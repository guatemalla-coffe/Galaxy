using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    public Transform tutorialsParent;
    public List<GameObject> tutorials;
    
    public int currentActiveIndex = 0;  
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            tutorials = new List<GameObject>();
            for (int i = 0; i < tutorialsParent.childCount; i++)
            {
                tutorials.Add(tutorialsParent.GetChild(i).gameObject);
            }
        }
    }

    private void Start()
    {
    }

    public void Show(int index)
    {
        Time.timeScale = 0.0000001f;
        tutorials[index].SetActive(true);
        currentActiveIndex = index;
    }

    public void Close()
    {
        foreach (var tutor in tutorials)
        {
            tutor.gameObject.SetActive(false);
        }
        Time.timeScale = 1f;
    }
}
