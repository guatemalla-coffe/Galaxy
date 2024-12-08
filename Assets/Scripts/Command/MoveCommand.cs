using Pathfinding;
using System.Threading.Tasks;
using UnityEngine;

public class MoveCommand : Command
{
    public BearController bear;
    private Vector3 targetPosition;
    private float endDistance;

    public MoveCommand(BearController bear, Vector3 target)
    {
        this.bear = bear;
        targetPosition = GetNearestReachablePoint(bear.transform.position, target);
        targetPosition.z = 0;
        endDistance = 1f;  // Стандартное значение, можно изменить
        commandName = "Идти сюда";
    }

    public MoveCommand(BearController bear, Vector3 target, float endDistance)
    {
        this.bear = bear;
        targetPosition = GetNearestReachablePoint(bear.transform.position, target);
        targetPosition.z = 0;
        this.endDistance = endDistance;
        commandName = "Идти сюда";
    }

    public override async Task ExecuteAsync()
    {
        bear.currentCommand = this;
        TaskManager.Instance.CompleteTask(1);
        // Устанавливаем состояние движения
        Debug.Log($"{bear.name} начал движение.");
        bear.SetState(new MoveState(bear, targetPosition, endDistance));

        // Ожидаем, пока медведь достигнет цели или команда не будет отменена
        while (Vector3.Distance(bear.transform.position, targetPosition) > endDistance)
        {
            await Task.Yield(); // Ожидаем следующего кадра
        }
        
        Debug.Log($"{bear.name} завершил движение.");
        Cancel();
    }
    
    public override void Cancel()
    {
        // Если команда отменяется, устанавливаем медведя в Idle состояние
        bear.currentCommand = null;
        bear.SetState(new IdleState(bear));
        Debug.Log("Command MOVE canceled");
    }
    
    private Vector3 GetNearestReachablePoint(Vector3 start, Vector3 target)
    {
        var graph = AstarPath.active.data.gridGraph;

        // Преобразуем стартовую точку в узел
        var startNode = graph.GetNearest(start).node;

        // Получаем список всех достижимых узлов из стартовой точки
        var reachableNodes = PathUtilities.GetReachableNodes(startNode);
        Debug.Log("LENB: "+reachableNodes.Count);

        // Ищем ближайшую достижимую точку к целевой
        GraphNode nearestNode = null;
        float minDistance = float.MaxValue;
        
        foreach (var node in reachableNodes)
        {
            var worldPosition = (Vector3)node.position;
            float distance = Vector3.Distance(worldPosition, target);
            if (distance < minDistance && node.Walkable)
            {
                minDistance = distance;
                nearestNode = node;
            }
        }
        
        if (nearestNode == null)
        {
            // Проверяем, находится ли агент на проходимом узле
            var node = graph.GetNearest(bear.transform.position).node;
            if (!node.Walkable)
            {
                // Перемещаем агента на ближайший проходимый узел
                node = graph.GetNearest(bear.transform.position).node;
                //bear.transform.position = (Vector3)node.position;
                Debug.LogWarning("Agent was placed on a non-walkable node. Repositioning...");
            }
        }
        

        if(nearestNode != null) Debug.Log("NOT NULL " + (Vector3)nearestNode.position); 
        else Debug.Log("NULL " + startNode); 

        return nearestNode != null ? (Vector3)nearestNode.position : start;
    }
}