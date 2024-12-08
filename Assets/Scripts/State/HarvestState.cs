using System.Threading.Tasks;
using UnityEngine;

public class HarvestState : BearState
{
    private ResourceObject resourceObject;
    private float harvestDuration = 0.5f; // Длительность рубки дерева в секундах
    private bool isHarvesting = false;

    public HarvestState(BearController bear, ResourceObject resource) : base(bear)
    {
        this.resourceObject = resource;
    }

    public override void Enter()
    {
        if (resourceObject == null)
        {
            Debug.LogError("Нет цели для добычи!");
            bear.SetState(new IdleState(bear));
            return;
        }
        
        bear.bearAnimations.SetFacingDirection(resourceObject.transform.position);
        Debug.Log($"{bear.name} начал добычу {resourceObject.name}.");
        StartHarvesting();
    }

    private async void StartHarvesting()
    {
        isHarvesting = true;

        // Анимация начала рубки (опционально)
        //bear.Animator.SetTrigger("Chop");

        // Ожидание завершения рубки
        if(resourceObject is TreeResource) bear.bearAnimations.StartHarvesting();
        else if(resourceObject is FlowerResource || resourceObject is BerryResource) bear.bearAnimations.StartCrafting();
        else bear.bearAnimations.StartMining();
        await Task.Delay((int)(harvestDuration * 1000));
        if(resourceObject == null) Exit();
        if (isHarvesting)
        {
            // Уменьшаем здоровье дерева
            resourceObject.TakeDamage();

            // Проверяем, можно ли продолжить рубку
            if (!resourceObject.IsDepleted())
            {
                Debug.Log($"{bear.name} продолжает добывать ресурс.");
                await Task.Delay((int)(harvestDuration * 1000));
                if(isHarvesting) StartHarvesting();
            }
            else
            {
                if(resourceObject is TreeResource) TaskManager.Instance.CompleteTask(2);
                Debug.Log($"{bear.name} завершил добычу.");
                isHarvesting = false;
                if(!(bear.GetState() is MoveState)) bear.SetState(new IdleState(bear));
            }
        }
        else
        {
            if(resourceObject is TreeResource) TaskManager.Instance.CompleteTask(2);
            Debug.Log($"{bear.name} завершил добычу 2.");
            isHarvesting = false;
            if(!(bear.GetState() is MoveState)) bear.SetState(new IdleState(bear));
        }
    }

    public override void Update()
    {
        if (!isHarvesting)
        {
            if(!(bear.GetState() is MoveState))bear.SetState(new IdleState(bear));
        }
    }

    public override void Exit()
    {
        isHarvesting = false;
        if(resourceObject is TreeResource) bear.bearAnimations.StopHarvesting();
        else if(resourceObject is FlowerResource || resourceObject is BerryResource) bear.bearAnimations.StopCrafting();
        else bear.bearAnimations.StopMining();
        Debug.Log($"{bear.name} прекратил добычу. Выход");
    }

    private void CheckTaskCompletion()
    {
        if(resourceObject is TreeResource) TaskManager.Instance.CompleteTask(2);
    }
}