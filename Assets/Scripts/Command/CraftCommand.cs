using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class CraftCommand : Command
{
    private Workbench workbench; // Ссылка на верстак
    private GameObject craftable; // Текущий крафтовый объект
    private float craftDuration = 3f; // Длительность крафта в секундах

    public CraftCommand(BearController bear, Workbench workbench)
    {
        this.bear = bear;
        this.workbench = workbench;

        // Название команды
        commandName = "Крафт";
    }

    public override async Task ExecuteAsync()
    {
        
        Debug.Log("CraftCommandExecuteAsync");
        if (bear.work != "Конструктор" && workbench.type == "Workshop")
        {
            UIManager.Instance.ShopTip("Эту задачу может выполнять только конструктор!");
            var moveCommanda = new MoveCommand(bear, workbench.craftPosition.position, 1f);
            await moveCommanda.ExecuteAsync();
            Cancel();
            return;
        }
        else if (bear.work != "Пасечник" && workbench.type == "HoneySynth")
        {
            UIManager.Instance.ShopTip("Эту задачу может выполнять только пасечник!");
            var moveCommanda = new MoveCommand(bear, workbench.craftPosition.position, 1f);
            await moveCommanda.ExecuteAsync();
            Cancel();
            return;
        }
        
        var moveCommand = new MoveCommand(bear, workbench.craftPosition.position, 1f);
        await moveCommand.ExecuteAsync();
        
        bear.currentCommand = this;
        
        Debug.Log("Current command is CraftCommand");
        while (workbench.craftList.Count > 0)
        {
            workbench.StartAnim();
            // Проверяем, есть ли доступные объекты для крафта
            if (workbench.craftList == null || !workbench.craftList.Any())
            {
                Debug.Log("Нет доступных объектов для крафта!");
                break;
            }

            Debug.Log("1 Current command is CraftCommand");

            // Берём первый объект для крафта
            craftable = workbench.craftList.FirstOrDefault();
            if (craftable == null)
            {
                Debug.Log("Объект для крафта не найден!");
                Cancel();
                break;
            }

            Debug.Log("2 Current command is CraftCommand");

            // Проверяем ресурсы
            foreach (var priceItem in craftable.GetComponent<Craftable>().price)
            {
                Debug.Log("3 Current command is CraftCommand");

                if (!workbench.HasResource(priceItem.name, priceItem.count))
                {
                    Debug.Log($"Не хватает ресурса {priceItem.name} для крафта {craftable.name}.");
                    Cancel();
                    break;
                }
            }

            Debug.Log("4 Current command is CraftCommand");

            //Debug.Log($"{bear.name} начинает крафт {craftable.resource.name}.");

            // Устанавливаем медведя в состояние крафта
            bear.SetState(new CraftState(bear, craftable, craftDuration));
            Debug.Log("5 Current command is CraftCommand");

            // Имитация крафта с задержкой
            await Task.Delay((int)(craftable.GetComponent<Craftable>().craftDuration * 1000));
            Debug.Log("6 Current command is CraftCommand");

            // Проверяем, был ли медведь прерван
            if (bear.currentCommand != this)
            {
                Debug.Log("7 Current command is CraftCommand");

                Debug.Log("CraftCommand отменён.");
                Cancel();
                return;
            }

            // Уменьшаем количество ресурсов
            foreach (var priceItem in craftable.GetComponent<Craftable>().price)
            {
                workbench.UseResource(priceItem.name, priceItem.count);
            }

            workbench.SpawnResource(craftable);
            workbench.craftList.Remove(craftable);
            UIManager.Instance.workshopWindow.UpdateWindow();
        }

        Debug.Log($"{bear.name} завершил крафт {craftable.name}.");
        workbench.StopAnim();
        Debug.Log("CraftCommand отменён.");
        bear.SetState(new IdleState(bear));
        bear.currentCommand = null;
        workbench.StopAnim();
        bear.SetState(new IdleState(bear)); // Переход в состояние ожидания
    }

    public override void Cancel()
    {
        Debug.Log("CraftCommand отменён.");
        bear.target.position = bear.transform.position;
        bear.SetState(new IdleState(bear));
        bear.currentCommand = null;
        workbench.StopAnim();
    }
}
