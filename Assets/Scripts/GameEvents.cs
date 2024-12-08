using System;

public static class GameEvents
{
    // События для различных действий
    public static event Action OnBuildingMenuOpen;
    public static event Action<int> OnCoinsCollected;

    // Методы для вызова событий
    public static void BuildingMenuOpen()
    {
        OnBuildingMenuOpen?.Invoke();
    }

    public static void CoinsCollected(int amount)
    {
        OnCoinsCollected?.Invoke(amount);
    }
}