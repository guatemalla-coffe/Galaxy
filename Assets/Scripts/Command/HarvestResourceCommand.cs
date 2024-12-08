using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class HarvestResourceCommand : Command
{
    private ResourceObject tResourceObject;

    public HarvestResourceCommand(BearController bear, ResourceObject resource)
    {
        this.bear = bear;
        this.tResourceObject = resource;
        if (resource is TreeResource)
        {
            commandName = "Срубить дерево";
        }
        else if (resource is StoneResource)
        {
            commandName = "Добыть руду";
        }
        else if (resource is FlowerResource)
        {
            commandName = "Собрать цветок";
        }
        else if (resource is BerryResource)
        {
            commandName = "Собрать ягоды";
        }
    }

    public override async Task ExecuteAsync()
    {
        // Создаём команду MoveCommand для подхода к дереву
        var moveCommand = new MoveCommand(bear, tResourceObject.harvestTransform.position, 1f);
        await moveCommand.ExecuteAsync();
        
        bear.currentCommand = this; // ставим команду медведю после выполнение MoveCommand
        await UniTask.Delay(500);

        // Проверяем, был ли медведь отменён или дерево недоступно
        if (bear == null || tResourceObject == null || tResourceObject.IsDepleted())
        {
            Debug.LogWarning("Команда Harvest отменена или цель недоступна.");
            return;
        }

        // Переход в состояние рубки дерева
        bear.SetState(new HarvestState(bear, tResourceObject));
        Debug.Log($"{bear.name} начал рубить дерево {tResourceObject.name}.");

        // Ожидание завершения рубки дерева
        while (!tResourceObject.IsDepleted())
        {
            // Проверяем, если команда была отменена
            if (bear.currentCommand != this)
            {
                Debug.Log("HarvestWoodCommand отменена.");
                return;
            }

            await Task.Yield();
        }

        bear.currentCommand = null;
        Debug.Log($"{bear.name} завершил добычу дерева {tResourceObject.name}. bear.currentCommand != this");
    }

    public override void Cancel()
    {
        if(bear.currentCommand == this) bear.currentCommand = null;
        bear.SetState(new IdleState(bear)); // При отмене возвращаем медведя в Idle состояние
        Debug.Log("HarvestWoodCommand отменена.");
    }
}