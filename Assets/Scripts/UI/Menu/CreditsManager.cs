using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class CreditsManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup creditsCanvasGroup; // CanvasGroup для управления прозрачностью
    [SerializeField] private RectTransform creditsText; // RectTransform текста титров
    [SerializeField] private GameObject[] otherWindows; // Другие окна, которые скрываются

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1f; // Длительность появления/исчезновения
    [SerializeField] private float scrollDuration = 10f; // Длительность прокрутки титров
    [SerializeField] private float startOffset = -1200; // Начальная позиция текста
    [SerializeField] private float endOffset = 1200; // Конечная позиция текста

    private bool isCreditsPlaying = false;
    private Tween currentTween; // Хранит текущую анимацию текста

    private void Start()
    {
        creditsCanvasGroup.alpha = 0;
        creditsCanvasGroup.gameObject.SetActive(false);
        foreach (var window in otherWindows)
        {
            window.SetActive(true);
        }
    }

    public void OnStartCredits()
    {
        if (isCreditsPlaying) return;

        isCreditsPlaying = true;
        creditsCanvasGroup.gameObject.SetActive(true);

        foreach (var window in otherWindows)
        {
            window.SetActive(false);
        }

        PlayCreditsAsync().Forget();
    }

    private async UniTaskVoid PlayCreditsAsync()
    {
        // Сброс позиции текста и текущей анимации
        ResetCredits();

        // Плавное появление титров
        await FadeInAsync();

        // Анимация текста
        currentTween = creditsText.DOAnchorPosY(endOffset, scrollDuration).SetEase(Ease.Linear);
        await currentTween.ToUniTask();

        // Плавное исчезновение титров
        await FadeOutAsync();

        // Восстановление окон
        EndCredits();
    }

    private void Update()
    {
        // Если нажата клавиша Enter, прерываем титры
        if (isCreditsPlaying && Input.GetKeyDown(KeyCode.Return))
        {
            EndCredits();
        }
    }

    private async UniTask FadeInAsync()
    {
        creditsCanvasGroup.alpha = 0;
        await creditsCanvasGroup.DOFade(1, fadeDuration).SetEase(Ease.InOutQuad).ToUniTask();
    }

    private async UniTask FadeOutAsync()
    {
        await creditsCanvasGroup.DOFade(0, fadeDuration).SetEase(Ease.InOutQuad).ToUniTask();
        creditsCanvasGroup.gameObject.SetActive(false);
    }

    private void ResetCredits()
    {
        // Устанавливаем начальную позицию текста
        creditsText.anchoredPosition = new Vector2(0, startOffset);

        // Прерываем текущую анимацию, если она активна
        currentTween?.Kill();
        currentTween = null;
    }

    private void EndCredits()
    {
        if (!isCreditsPlaying) return;

        isCreditsPlaying = false;

        // Прерываем текущую анимацию
        ResetCredits();

        // Прячем титры
        creditsCanvasGroup.gameObject.SetActive(false);
        creditsCanvasGroup.alpha = 0;

        foreach (var window in otherWindows)
        {
            window.SetActive(true);
        }
    }
}
