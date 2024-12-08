using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ScienceCommand : Command
{
    private ScienceTable scienceTable; // Ссылка на верстак
    private GameObject resourceIcon; // Текущий крафтовый объект
    private float craftDuration = 3f; // Длительность крафта в секундах

    public ScienceCommand(BearController bear, ScienceTable scTable)
    {
        this.bear = bear;
        this.scienceTable = scTable;

        // Название команды
        commandName = "Исследовать";
    }

    public override async Task ExecuteAsync()
    {
        
        if (bear.work != "Исследователь")
        {
            UIManager.Instance.ShopTip("Эту задачу может выполнять только исследователь!");
            var moveCommanda = new MoveCommand(bear, scienceTable.craftPosition.position, 1f);
            await moveCommanda.ExecuteAsync();
            Cancel();
            return;
        }
        Debug.Log("ScienceCommandExecuteAsync");
        
        var moveCommand = new MoveCommand(bear, scienceTable.craftPosition.position, 1f);
        await moveCommand.ExecuteAsync();
        
        bear.currentCommand = this;
        
        Debug.Log("Current command is ScienceCommand");
        while (scienceTable.craftList.Count > 0)
        {
            Debug.Log("Inside ScienceCommand while loop");
            scienceTable.StartAnim();
            // Проверяем, есть ли доступные объекты для крафта
            if (scienceTable.craftList == null || !scienceTable.craftList.Any())
            {
                Debug.Log("Нет доступных объектов для изучения!");
                break;
            }

            Debug.Log("1 Current command is ScienceCommand");

            // Берём первый объект для крафта
            resourceIcon = scienceTable.craftList.FirstOrDefault();
            ResourceToUI exploredRes = scienceTable.craftList.FirstOrDefault().GetComponent<ResourceToUI>();
            if (exploredRes == null)
            {
                Debug.Log("Объект для изучения не найден!");
                Cancel();
                break;
            }
            
            bear.SetState(new ScienceState(bear, resourceIcon, craftDuration));
            Debug.Log("5 Current command is ScienceCommand");
            
            
            await Task.Delay((int)(1000));
            Debug.Log("6 Current command is ScienceCommand");

                        
            Debug.Log("Объект для изучения не найден!");
            Cancel();
            
            exploredRes.Explore();
            TaskManager.Instance.CompleteTask(24);
            scienceTable.craftList.Remove(resourceIcon);
            scienceTable.PlaySound();
            
            //UIManager.Instance.scienceTableWindow.UpdateWindow();
            
            break;
            

            // Debug.Log($"{bear.name} завершил изучение {resourceIcon.name}.");
            // scienceTable.StopAnim();
            // Debug.Log("ScienceCommand отменён.");
            // bear.currentCommand = null;
            // bear.SetState(new IdleState(bear)); // Переход в состояние ожидания
            // Cancel();

            
        }

        // Debug.Log($"{bear.name} завершил изучение {resourceIcon.name}.");
        // scienceTable.StopAnim();
        // Debug.Log("ScienceCommand отменён.");
        // bear.currentCommand = null;
        // bear.SetState(new IdleState(bear)); // Переход в состояние ожидания
        // Cancel();
        return;
    }

    public override void Cancel()
    {
        Debug.Log("ScienceCommand отменён.");
        bear.SetState(new IdleState(bear));
        bear.currentCommand = null;
        scienceTable.StopAnim();
    }
}
