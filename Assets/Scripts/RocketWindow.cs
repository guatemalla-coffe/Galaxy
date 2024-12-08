using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RocketWindow : MonoBehaviour
{
    public Transform lettersLayoutList;
    public Rocket rocket;
    public GameObject buttonPrefab;

    public Transform onWorkWindow;
    public Transform chooseItemWindow;
    public GameObject notEnoughRes;
    public GameObject cannotLoad;
    
    public async void LoadItems()
    {
        ClearLettersList();
        await new RequestsManager().GetShops();
        if (GameManager.Instance.shopsAcitve)
        {
            
            cannotLoad.SetActive(false);

            foreach (var item in GameManager.Instance.GetShopByName("Rocket").Resources)
            {
                GameObject butt = Instantiate(buttonPrefab, lettersLayoutList);
                butt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.Name; // Тут локализация
                butt.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Icons/{item.Name}");
                butt.GetComponent<Button>().onClick.AddListener(delegate
                {
                    if (UIManager.Instance.GetResourceIconByName("EHoney").GetCount() >= 1)
                    {
                        notEnoughRes.SetActive(false);
                        UIManager.Instance.GetResourceIconByName("EHoney").IncreaseAmount(-1);
                        rocket.StartWork(item.Name, 5);
                    }
                    else
                    {
                        notEnoughRes.SetActive(true);
                    }
                });
            }
        }
        else{
            cannotLoad.SetActive(true);
        }
    }

    private void ClearLettersList()
    {
        for (int i = 0; i < lettersLayoutList.childCount; i++)
        {
            Destroy(lettersLayoutList.GetChild(i).gameObject);
        }
    }

    public async void OpenWindow(Rocket rocketS)
    {
        rocket = rocketS;
        gameObject.SetActive(true);
        await new RequestsManager().GetShops();
        if (rocket.onWork)
        {
            onWorkWindow.gameObject.SetActive(true);
            chooseItemWindow.gameObject.SetActive(false);
        }
        else
        {
            chooseItemWindow.gameObject.SetActive(true);
            onWorkWindow.gameObject.SetActive(false);
            LoadItems();
        }
    }

    public void CloseWindow()
    {
        gameObject.SetActive(false);
        notEnoughRes.SetActive(false);
    }
}
