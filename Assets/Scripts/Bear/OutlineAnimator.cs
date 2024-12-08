using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class OutlineAnimator : MonoBehaviour
{
    public Material outlineMaterial; // Материал с шейдером
    public float pulseSpeed = 2f; // Скорость анимации
    public float minThickness = 0.05f; // Минимальная толщина обводки
    public float maxThickness = 0.1f; // Максимальная толщина обводки

    private bool isAnimating = false;
    private float currentThickness = 0.05f;
    private float pulseDirection = 1; // Направление изменения толщины

    void Update()
    {
        if (isAnimating && outlineMaterial != null)
        {
            // Рассчитываем изменение толщины
            currentThickness += pulseDirection * pulseSpeed * Time.deltaTime * 0.1f;

            // Проверяем границы и меняем направление
            if (currentThickness >= maxThickness)
            {
                currentThickness = maxThickness;
                pulseDirection = -1;
            }
            else if (currentThickness <= minThickness)
            {
                currentThickness = minThickness;
                pulseDirection = 1;
            }

            // Устанавливаем значение толщины обводки
            outlineMaterial.SetFloat("_OutlineThickness", currentThickness);
            
        }
    }

    public async void StartAnimation()
    {
        isAnimating = true;
        await UniTask.Delay(TimeSpan.FromSeconds(0.25f));
        StopAnimation();
    }

    public void StopAnimation()
    {
        isAnimating = false;
        pulseDirection = 1f;
        currentThickness = minThickness;
        // Сбросить толщину обводки к минимальному значению
        if (outlineMaterial != null)
        {
            outlineMaterial.SetFloat("_OutlineThickness", minThickness);
        }
    }
}