using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ComicManager : MonoBehaviour
{

    [SerializeField] private GameObject[] images;
    [Range(1f, 15f)][SerializeField] private float timeToChange;
    [SerializeField] private bool destroyOnRestart = false;

    [SerializeField] private bool isTutorial = false;
    private int currentSlide = 1;
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            foreach (var img in images)
            {
                img.SetActive(false);
            }
            if (currentSlide < images.Length)
            {
                images[currentSlide].SetActive(true);
            }
            else
            {
                CloseComic();
            }

            currentSlide++;
        }
    }

    
    public void CloseComic()
    {
        foreach (var img in images)
        {
            img.SetActive(false);
        }

        //images[0].SetActive(true);
        currentSlide = 0;
        Time.timeScale = 1;
        SceneManager.LoadScene(2);
        
        //gameObject.SetActive(false);
    }
}
