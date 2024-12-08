using System.Collections.Generic;
using UnityEngine;

// public class CommandQueue
// {
//     public Queue<Command> commandQueue = new Queue<Command>();
//     private bool isExecuting = false;
//     public Command currentCommand = null;
//
//     public void AddCommand(Command command)
//     {
//         // Если текущая команда существует, отменяем её
//         if (currentCommand != null)
//         {
//             Debug.Log("Queue > 0: Cancelling current command.");
//             currentCommand.Cancel();  // Отменяем текущую команду
//             currentCommand = null;  // Обнуляем текущую команду
//         }
//
//         // Добавляем команду в очередь
//         commandQueue.Enqueue(command);
//         
//         // Если сейчас ничего не выполняется, начинаем выполнение очереди
//         TryExecuteNext();
//     }
//
//     private async void TryExecuteNext()
//     {
//         if (isExecuting || commandQueue.Count == 0)
//         {
//             Debug.Log($"isExecuting {isExecuting}, commandQueue.Count {commandQueue.Count} => RETURN");
//             return;
//         }
//
//         // Берём команду из очереди
//         Command command = commandQueue.Dequeue();
//         currentCommand = command;  // Устанавливаем текущую команду
//
//         isExecuting = true;
//
//         // Выполнение команды асинхронно
//         await command.ExecuteAsync();
//
//         // После завершения выполнения
//         isExecuting = false;
//         currentCommand = null; // Сбрасываем текущую команду
//
//         // Пытаемся выполнить следующую команду, если она есть
//         TryExecuteNext();
//     }
// }