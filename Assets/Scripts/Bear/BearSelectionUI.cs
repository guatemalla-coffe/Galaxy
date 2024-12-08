using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BearSelectionUI : MonoBehaviour, ISelectionObserver
{
    public TextMeshProUGUI selectedBearNameText;

    void Start()
    {
        BearManager.Instance.SelectBear(null); // Сбросить выделение
    }

    public void OnBearSelected(BearController bear)
    {
        selectedBearNameText.text = bear != null ? $"Selected: {bear.name}" : "No Bear Selected";
    }
}