using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class EatCommand : Command
{
    private Table table; // Ссылка на объект Table
    private GameObject targetResource; // Текущий ресурс для поедания
    private float eatDuration = 2f; // Время на поедание ресурса

    public EatCommand(BearController bear, Table table)
    {
        this.bear = bear;
        this.table = table;

        // Название команды
        commandName = "Поесть";
    }

    public override async Task ExecuteAsync()
    {
        var moveCommand = new MoveCommand(bear, table.eatPosition.position, 1f);
        await moveCommand.ExecuteAsync();
        
        bear.currentCommand = this;
        
        Debug.Log("Eat 1");
        // Проверяем, есть ли доступные ресурсы
        if (table.currentFuel == null || !table.currentFuel.Any())
        {
            Debug.LogWarning("Нет доступных ресурсов для поедания!");
            return;
        }
        Debug.Log("Eat 2");

        // Берём первый ресурс из списка
        targetResource = table.currentFuel.FirstOrDefault();
        if (targetResource == null)
        {
            Debug.LogWarning("Ресурс недоступен!");
            return;
        }

        Debug.Log($"{bear.name} начинает есть {targetResource.name}.");

        // Устанавливаем медведя в состояние поедания (опционально)
        bear.SetState(new EatingState(bear, targetResource));

        // Имитация поедания с задержкой
        await Task.Delay((int)(eatDuration * 1000));

        // Проверяем, был ли медведь прерван
        if (bear.currentCommand != this)
        {
            Debug.Log("EatCommand отменён.");
            return;
        }

        bear.Feed(targetResource.GetComponent<ResourceToUI>().feedAmount);
        table.currentFuel.Remove(targetResource);
        Debug.Log($"{bear.name} завершил поедание {targetResource.name}.");
        bear.SetState(new IdleState(bear)); // Переход в состояние ожидания
    }

    public override void Cancel()
    {
        Debug.Log("EatCommand отменён.");
        bear.SetState(new IdleState(bear));
    }
}
