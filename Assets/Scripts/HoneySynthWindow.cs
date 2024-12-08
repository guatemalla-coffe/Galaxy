using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoneySynthWindow : MonoBehaviour
{
    public Image honeySynthIcon;
    public Image fillRect_Progress;
    public TextMeshProUGUI textProgress;
    
    public HoneySynth honeySynth;
    
    
    private void Awake()
    {
        UpdateWindow();
    }

    private void FixedUpdate()
    {
        if (honeySynth != null)
        {
            UpdateWindow();
        }
    }

    public void UpdateWindow()
    {
        honeySynthIcon.sprite = honeySynth.spriteRenderer.sprite;
    }

    public void OpenWindow(HoneySynth honeySynthS)
    {
        honeySynth = honeySynthS;
        gameObject.SetActive(true);
        UpdateWindow();
    }

    public void CloseWindow()
    {
        HoneySynth[] furs = GameObject.FindObjectsByType<HoneySynth>(FindObjectsSortMode.None);
        foreach (var fur in furs)
        {
            fur.Deselect();
        }  
        gameObject.SetActive(false);
    }
}
