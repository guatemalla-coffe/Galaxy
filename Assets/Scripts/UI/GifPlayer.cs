using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GifPlayer : MonoBehaviour
{
    [SerializeField] private RawImage gifDisplay; // RawImage для отображения GIF
    [SerializeField] private Texture2D[] frames; // Массив кадров GIF
    [SerializeField] private float frameDelay = 0.1f; // Задержка между кадрами (в секундах)

    private void Start()
    {
        if (frames == null || frames.Length == 0)
        {
            Debug.LogError("Кадры GIF не назначены!");
            return;
        }

        StartCoroutine(PlayGif());
    }

    private IEnumerator PlayGif()
    {
        int frameIndex = 0;

        while (true) // Зацикливаем анимацию
        {
            gifDisplay.texture = frames[frameIndex]; // Устанавливаем текущий кадр
            frameIndex = (frameIndex + 1) % frames.Length; // Переходим к следующему кадру
            yield return new WaitForSeconds(frameDelay); // Ждем указанное время
        }
    }
}