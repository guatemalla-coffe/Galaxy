using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using TMPro;

public class UIButtonHover : MonoBehaviour
{
    public GameObject popup; // Ссылка на Image (всплывающее окно)
    [SerializeField] private float fadeDuration = 0.3f; // Длительность анимации появления/исчезновения

    private CanvasGroup popupCanvasGroup;
    private Tween currentTween; // Для контроля текущей анимации
    [SerializeField] private GameObject resPrefab;

    private void Awake()
    {
        // Проверяем наличие CanvasGroup на popup
        popupCanvasGroup = popup.GetComponent<CanvasGroup>();
        if (popupCanvasGroup == null)
        {
            popupCanvasGroup = popup.AddComponent<CanvasGroup>();
        }

        // Убираем окно по умолчанию
        popupCanvasGroup.alpha = 0;
        popup.SetActive(false);
    }

    public void OnPointerEnter()
    {
        ShowPopup().Forget();
    }

    public void OnPointerExit()
    {
        HidePopup().Forget();
    }

    private async UniTaskVoid ShowPopup()
    {
        // Если уже идет анимация исчезновения, остановим её
        currentTween?.Kill();

        popup.SetActive(true);
        currentTween = popupCanvasGroup.DOFade(1, fadeDuration).SetEase(Ease.OutQuad);
        await UniTask.Delay((int)(fadeDuration * 1000)); // Ждём завершения анимации
    }

    private async UniTaskVoid HidePopup()
    {
        // Если уже идет анимация появления, остановим её
        currentTween?.Kill();

        currentTween = popupCanvasGroup.DOFade(0, fadeDuration).SetEase(Ease.OutQuad);
        await UniTask.Delay((int)(fadeDuration * 1000)); // Ждём завершения анимации
        popup.SetActive(false); // Только после завершения анимации деактивируем объект
    }

    public void GenerateRes(List<ResourceForBD> data)
    {
        for (int i = 0; i < popup.transform.childCount; i++)
        {
            Destroy(popup.transform.GetChild(i).gameObject);
        }
        foreach (var resource in data)
        {
            GameObject spawnedRes = Instantiate(resPrefab, popup.transform);
            Sprite resourceSprite = Resources.Load<Sprite>($"Icons/{resource.name}");
            if (resourceSprite != null)
            {
                spawnedRes.transform.GetChild(0).GetComponent<Image>().sprite = resourceSprite;
            }

            spawnedRes.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = resource.count.ToString();
        }
    }
}