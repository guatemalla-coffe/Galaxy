using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextAnim : MonoBehaviour
{
    private RectTransform _transform;
    private Button _button;
    public GameObject nextButtonSound;

    private void Start()
    {
        _transform = GetComponent<RectTransform>();
        _button = GetComponent<Button>();
        
        EventTrigger.Entry smallButt = new EventTrigger.Entry();
        smallButt.eventID = EventTriggerType.PointerExit;
        smallButt.callback.AddListener((eventData) =>
        {
            _transform.localScale = new Vector3(1f, 1f, 1f); 
            if (nextButtonSound != null)
            {
                _button.transform.GetChild(0).gameObject.SetActive(false);
                GetComponent<TextMeshProUGUI>().color = Color.grey;
            }
        });
        
        EventTrigger.Entry bigButt = new EventTrigger.Entry();
        bigButt.eventID = EventTriggerType.PointerEnter;
        bigButt.callback.AddListener((eventData) =>
        {
            _transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            if (nextButtonSound != null)
            {
                GameObject spawnedSound = Instantiate(nextButtonSound);
                Destroy(spawnedSound, 1f);
                _transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
                GetComponent<TextMeshProUGUI>().color = Color.white;
                _button.transform.GetChild(0).gameObject.SetActive(true);
            }
        });
        
        _button.AddComponent<EventTrigger>();
        _button.GetComponent<EventTrigger>().triggers.Add(bigButt);
        _button.GetComponent<EventTrigger>().triggers.Add(smallButt);
        
        _button.onClick.AddListener(delegate
        {
            _transform.localScale = new Vector3(1f, 1f, 1f); 
            if (nextButtonSound != null)
            {
                _button.transform.GetChild(0).gameObject.SetActive(false);
                GetComponent<TextMeshProUGUI>().color = Color.grey;
            }
        });
    }
    
    
}
