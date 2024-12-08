using UnityEngine;

public class CraftState : BearState
{
    private GameObject craftable;
    private float craftDuration;

    public CraftState(BearController bear, GameObject craftable, float craftDuration) : base(bear)
    {
        this.craftable = craftable;
        this.craftDuration = craftDuration;
    }

    public override void Enter()
    {
        Debug.Log($"{bear.name} начал крафт {craftable.name}.");
        bear.bearAnimations.StartCrafting();
        
    }

    public override void Update()
    {
        // Здесь можно добавить дополнительные проверки
    }

    public override void Exit()
    {
        bear.bearAnimations.StopCrafting();
        Debug.Log($"{bear.name} завершил крафт {craftable.name}.");
    }
}