using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class BearManager : MonoBehaviour
{
    public static BearManager Instance { get; private set; }
    public LayerMask playerLayerMask;
    public BearController selectedBear; // Текущий выбранный медведь

    private BearController[] bears;
    public List<BearController> bearsList;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        bears = GameObject.FindObjectsByType<BearController>(FindObjectsSortMode.None);
        bearsList = new List<BearController>();
        foreach (var bear in bears)
        {
            bearsList.Add(bear);
        }
    }

    public void SelectBear(BearController bear)
    {
        if (selectedBear == bear)
        {
            // Если медведь уже выбран, ничего не делать
            return;
        }

        if (selectedBear != null)
        {
            selectedBear.Deselect(); // Снимаем выделение
        }

        selectedBear = bear;
        if (selectedBear != null)
        {
            selectedBear.Select(); // Устанавливаем выделение
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 20f, playerLayerMask);
            // Проверяем, если указатель над объектом UI, выходим

            BearController bear;
            foreach (var hit in hits)
            {
                if (hit.transform.gameObject.TryGetComponent<BearController>(out bear))
                {
                    BearManager.Instance.SelectBear(bear);
                    break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SelectNextBear();    
        }
    }

    private void SelectNextBear()
    {
        BearController currentBear = GetSelectedBear();
        int indexCurrent = bearsList.IndexOf(currentBear);
        if (indexCurrent + 1 >= bearsList.Count)
        {
            indexCurrent = 0;
        }
        else
        {
            indexCurrent += 1;
        }

        Vector3 movePosition = bearsList[indexCurrent].transform.position;
        movePosition.z = -10f;
        Camera.main.transform.DOMove(movePosition, 0.5f);
        SelectBear(bearsList[indexCurrent]);
    }
    
    public BearController GetSelectedBear()
    {
        return selectedBear;
    }
}